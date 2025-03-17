using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shuile;
using Shuile.Framework;
using Shuile.Gameplay.Character;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.Examples.NetChat
{
    public class NetGameplayManager : MonoContainer
    {
        private NetManager _client;
        private NetManager _serverManager; // can be null
        private int? _clientIdInServer;

        private NetPeer _serverPeer;

        private bool _isServer;

        private int _updateTick = 0;

        public Player SelfPlayer { get; set; }

        private Dictionary<int, GameObject> _otherPlayers = new Dictionary<int, GameObject>();

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            base.LoadFromParentContext(context);

            var gamePlayScene = (GamePlayScene)ContainerExtensions.FindSceneContainer();
            if (gamePlayScene.TryGetPlayer(out var player))
            {
                SelfPlayer = player;
            }
            else
            {
                Debug.LogError("Player not found");
            }

            // server
            _isServer = context.Get<NetChatConfig>().IsServer;
            if (_isServer)
            {
                InitializeServer();
            }

            // client
            {
                var ip = context.Get<NetChatConfig>().ServerIp;
                InitializeClient(ip);
            }
        }

        private void InitializeClient(string ip)
        {
            var listener = new EventBasedNetListener();
            _client = new NetManager(listener);
            _client.Start();
            _serverPeer = _client.Connect(ip, 7777, "AceTaffy");
            Debug.Log($"[Client]: try connect to server: {_serverPeer.Address}");

            // listener.PeerConnectedEvent += peer =>
            // {
            //     _serverPeer = peer;
            //     Debug.Log($"[Client]: try connect to server: {_serverPeer.Address}");
            // };

            listener.NetworkReceiveEvent += (peer, reader, channelNumber, deliveryMethod) =>
            {
                var type = (PacketServerGameplayType)reader.GetByte();
                if (type == PacketServerGameplayType.PlayerState)
                {
                    var json = reader.GetString();
                    var obj = JsonConvert.DeserializeObject<PlayerState>(json);
                    // 更新代理Player位置
                    if (_otherPlayers.TryGetValue(obj.PlayerId, out var p))
                    {
                        p.transform.position = new Vector3(obj.PositionX, obj.PositionY, p.transform.position.z);
                    }

                    Debug.Log($"[Client]: Message-PlayerState received: {json}");
                }
                else if (type == PacketServerGameplayType.PlayerJoin)
                {
                    var id = reader.GetInt();
                    if (_clientIdInServer != null && _clientIdInServer != id) // 不是自己
                    {
                        // 创建角色
                        _otherPlayers.Add(id, CreatePlayerRenderer());
                        Debug.Log($"[Client]: PlayerJoin received: {id}");
                    }
                }
                else if (type == PacketServerGameplayType.ClientConnected)
                {
                    var jObject = JObject.Parse(reader.GetString());
                    _clientIdInServer = jObject["clientId"].Value<int>();
                    var existingIds = jObject["existingIds"].Values<int>().ToArray();

                    // 创建角色
                    foreach (var existingId in existingIds.Where(x => x != _clientIdInServer))
                    {
                        _otherPlayers.Add(existingId, CreatePlayerRenderer());
                        Debug.Log($"[Client]: Init existing players: {string.Join(',', existingIds)}");
                    }
                }
            };
        }

        private void InitializeServer()
        {
            var listener = new EventBasedNetListener();
            _serverManager = new NetManager(listener);
            _serverManager.Start(7777);
            _serverManager.UpdateTime = 15;
            _serverManager.DisconnectTimeout = 5000;

            listener.ConnectionRequestEvent += request =>
            {
                request.AcceptIfKey("AceTaffy");
                Debug.Log($"[Server]: get connection request: {request.RemoteEndPoint} - Key: {request.Data.PeekString()}");
            };

            listener.PeerConnectedEvent += peer =>
            {
                Debug.Log($"[Server]: Peer connected: {peer.Address}");

                // 发clientId 和 已有Player
                var writer = new NetDataWriter();
                writer.Put((byte)PacketServerGameplayType.ClientConnected);
                var existingIds = _serverManager.Select(x => x.Id).ToArray();
                var jObject = new JObject { ["clientId"] = peer.Id, ["existingIds"] = new JArray(existingIds) };
                writer.Put(jObject.ToString());
                peer.Send(writer, DeliveryMethod.ReliableOrdered);

                // 广播新Player加入
                var writer2 = new NetDataWriter();
                writer2.Put((byte)PacketServerGameplayType.PlayerJoin);
                writer2.Put(peer.Id);
                _serverManager.SendToAll(writer2, DeliveryMethod.ReliableUnordered);
            };

            listener.NetworkReceiveEvent += (peer, reader, channelNumber, deliveryMethod) =>
            {
                // dispatch
                var type = (PacketClientGameplayType)reader.GetByte();
                if (type == PacketClientGameplayType.PlayerState)
                {
                    var json = reader.GetString();

                    var writer = new NetDataWriter();
                    writer.Put((byte)PacketServerGameplayType.PlayerState);
                    writer.Put(json);
                    _serverManager.SendToAll(writer, DeliveryMethod.ReliableUnordered);
                    Debug.Log($"[Server]: Message received: {json}");
                }
            };
        }

        private GameObject CreatePlayerRenderer()
        {
            var playerPrefab = GameApplication.BuiltInData.GetFromPrefabArray("PlayerRenderer");
            var inst = Instantiate(playerPrefab);
            if (inst.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            return inst;
        }

        private void Update()
        {
            if (_isServer)
            {
                _serverManager.PollEvents();
            }
            _client.PollEvents();

            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                Debug.Log("V pressed");
                var playerPrefab = GameApplication.BuiltInData.GetFromPrefabArray("PlayerRenderer");
                var inst = Instantiate(playerPrefab);
                _otherPlayers.Add(_otherPlayers.Count, inst);
            }
        }

        private void FixedUpdate()
        {
            if (_clientIdInServer == null || _serverPeer == null)
            {
                return;
            }

            _updateTick++;
            if (_updateTick % 5 == 0)
            {
                SendStateToServer(new PlayerState()
                {
                    PositionX = SelfPlayer.transform.position.x,
                    PositionY = SelfPlayer.transform.position.y,
                    PlayerId = _clientIdInServer.Value
                });
            }
        }

        public void SendStateToServer(PlayerState data)
        {
            if (_serverPeer == null)
            {
                Debug.LogError("Not connected to server");
                return;
            }

            var writer = new NetDataWriter();
            writer.Put((byte)PacketClientGameplayType.PlayerState);
            writer.Put(JsonConvert.SerializeObject(data));
            _serverPeer.Send(writer, DeliveryMethod.ReliableUnordered);
        }
    }

    public struct PlayerState
    {
        public float PositionX;
        public float PositionY;
        public bool Attack;

        public int PlayerId; // Server侧的PeerId
    }

    public enum PacketServerGameplayType : byte
    {
        PlayerJoin,
        PlayerState,
        ClientConnected
    }

    public enum PacketClientGameplayType : byte
    {
        PlayerState
    }
}
