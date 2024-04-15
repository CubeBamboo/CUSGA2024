using Shuile.Network.Packets;

using System.Collections.Generic;

namespace Shuile.Network
{
    public interface IHost
    {
        /// <summary>
        /// ��ǰ�����ӵ����н�ɫ��Id
        /// </summary>
        public IReadOnlyList<int> Actors { get; }

        /// <summary>
        /// ���ӵ��˷����õ��������ַ���
        /// </summary>
        public string ConnectionString { get; }
        
        /// <summary>
        /// �Ƿ���Լ��뷿��
        /// </summary>
        public bool CanJoin { get; set; }

        /// <summary>
        /// ���͸�ĳ����ɫ
        /// </summary>
        /// <typeparam name="T">���ݰ�����</typeparam>
        /// <param name="actorId">��ɫ��id</param>
        /// <param name="packet">���ݰ�</param>
        public void SendTo<T>(int actorId, T packet) where T : Packet, new();

        /// <summary>
        /// �������ݰ�����Unity���̵߳���
        /// </summary>
        public void PollEvents();
    }
}
