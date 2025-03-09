using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shuile.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Scenes.Examples.NetChat
{
    public class NetChatConfig
    {
        public string ServerIp { get; set; }
        public bool IsServer { get; set; }
    }

    public class NetChatUIManager : MonoContainer
    {
        // ui
        [SerializeField] private Button sendButton;
        [SerializeField] private TMP_InputField messageInput;
        [SerializeField] private TMP_InputField nameInput;

        [SerializeField] private GameObject chatTemplate;

        // client
        private ChatClient _chatClient;
        private ChatServer _chatServer; // can be null
        private bool _isServer;

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            base.LoadFromParentContext(context);

            // server
            _isServer = context.Get<NetChatConfig>().IsServer;
            if (_isServer)
            {
                _chatServer = new ChatServer();
            }

            // ip
            var ip = context.Get<NetChatConfig>().ServerIp;
            _chatClient = new ChatClient();
            _chatClient.Connect(ip);

            _chatClient.OnChatReceived += AddToChatUI;
        }

        private void Start()
        {
            sendButton.onClick.AddListener(() =>
            {
                // 自己更新
                var msg = messageInput.text;
                var nm = nameInput.text;
                // AddToChatUI(nm, msg);

                // 发包网络更新
                _chatClient.SendChatToServer(nm, msg);
            });
        }

        private void Update()
        {
            if (_isServer)
            {
                _chatServer?.PollEvents();
            }

            _chatClient?.PollEvents();
        }

        private void OnDestroy()
        {
            _chatClient?.Dispose();
            _chatServer?.Dispose();
        }

        // update ui
        public void AddToChatUI(string senderName, string message)
        {
            if (string.IsNullOrEmpty(senderName) || string.IsNullOrEmpty(message))
            {
                return;
            }

            var instance = Object.Instantiate(chatTemplate, chatTemplate.transform.parent);
            instance.transform.SetAsLastSibling();
            instance.name = "ChatCell";
            instance.GetComponentInChildren<TextMeshProUGUI>().text = $"{senderName}: {message}";
            instance.SetActive(true);
        }
    }

    public class ChatClient : IDisposable
    {
        private NetManager _netManager;
        private NetPeer _server;

        public event Action<string, string> OnChatReceived;

        public void PollEvents()
        {
            _netManager?.PollEvents();
        }

        public void Connect(string address)
        {
            var listener = new EventBasedNetListener();
            _netManager = new NetManager(listener);
            _netManager.Start();
            _server = _netManager.Connect(address, 7777, "AceTaffy");

            Debug.Log($"[Client]: initialized the client, trying to connect to server: {address}");

            listener.PeerConnectedEvent += peer =>
            {
                _server = peer;
                Debug.Log($"[Client]: Connected to server: {_server.Address}");
            };

            listener.NetworkReceiveEvent += (peer, reader, channelNumber, deliveryMethod) =>
            {
                var json = reader.GetString();
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                var name = obj["name"].ToString();
                var message = obj["message"].ToString();

                Debug.Log($"[Client]: Message received: {name}: {message}");
                OnChatReceived?.Invoke(name, message);
            };
        }

        public void SendChatToServer(string name, string message)
        {
            if (_server == null)
            {
                Debug.LogError("Not connected to server");
                return;
            }

            // todo: 多数据包区分
            var writer = new NetDataWriter();
            writer.Put(JsonConvert.SerializeObject(new JObject { ["name"] = name, ["message"] = message }));
            _server.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void Dispose()
        {
            _netManager?.Stop();
        }
    }

    public class ChatServer : IDisposable
    {
        private NetManager _server;

        private List<NetPeer> _clients = new List<NetPeer>();

        public ChatServer()
        {
            var listener = new EventBasedNetListener();
            _server = new NetManager(listener);

            _server.Start(7777);
            _server.UpdateTime = 15;
            _server.DisconnectTimeout = 5000;

            Debug.Log("[Server]: Server started at port 7777");

            listener.ConnectionRequestEvent += request =>
            {
                request.AcceptIfKey("AceTaffy");
                Debug.Log($"[Server]: get connection request: {request.RemoteEndPoint} - {request.Data.PeekString()}");
            };

            listener.PeerConnectedEvent += peer =>
            {
                Debug.Log($"[Server]: Peer connected: {peer.Address}");
                _clients.Add(peer);
            };

            listener.NetworkReceiveEvent += (peer, reader, channelNumber, deliveryMethod) =>
            {
                // dispatch
                var json = reader.GetString();
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                var name = obj["name"].ToString();
                var message = obj["message"].ToString();

                Debug.Log($"[Server]: Message received: {name}: {message}");

                var writer = new NetDataWriter();
                writer.Put(JsonConvert.SerializeObject(new JObject { ["name"] = name, ["message"] = message }));
                _server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            };
        }

        public void PollEvents()
        {
            _server?.PollEvents();
        }

        public void Dispose()
        {
            _server?.Stop();
        }
    }
}
