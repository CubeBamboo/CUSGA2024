using Cysharp.Threading.Tasks;

namespace Shuile.Network
{
    public interface IMultiPlayerService
    {
        /// <summary>
        /// �Ƿ������ӵ�����
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// ��ʼ������ʹ�ñ�Ĺ���ǰ�������
        /// </summary>
        public UniTask Init();

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="initDescription">����Ԥ������Ϣ</param>
        /// <returns>�����������</returns>
        /// <exception cref="ConnectionStringFormatInvalidException">�����ַ�����ʽ��Ч</exception>
        public UniTask<IHost> CreateRoom(string initDescription = null);
        
        /// <summary>
        /// ���뷿��
        /// </summary>
        /// <param name="connectionString">ͨ��<see cref="IHost.ConnectionString"/>��ȡ</param>
        /// <returns>���������Ϊ�ͻ���</returns>
        /// <exception cref="ConnectionStringFormatInvalidException">�����ַ�����ʽ��Ч</exception>
        public UniTask<IRoom> JoinRoomAsync(string connectionString);
    }
}

// TapSDK�У����Ӿ�����TapServerֻ��һ�����ӣ����õ�����Ϣֻ��ActorId�������˵�Id��Ψһ���ŵ�
// LiteNetLib�У��������Ը����ͻ��ˣ�����ȷ������˭����

// TapSDK�У���MasterClient���Ա�ת�ƣ�LiteNetLib����Ҫ�ֶ�ʵ�ִ˹��ܣ�������������ˣ�

// MasterClient��Ҫ��������Ϸ�߼���