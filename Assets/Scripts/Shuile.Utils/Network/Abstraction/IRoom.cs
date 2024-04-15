using Cysharp.Threading.Tasks;

using Shuile.Network.Packets;

using System.Collections.Generic;

namespace Shuile.Network
{
    public interface IRoom
    {
        /// <summary>
        /// ��ǰ�����ӵ����н�ɫ��Id
        /// </summary>
        public IReadOnlyList<int> Actors { get; }

        /// <summary>
        /// �Ƿ��Ѿ����뷿��
        /// </summary>
        public bool IsJoined { get; }
        
        /// <summary>
        /// �������ݰ������������뵽�����Ͷ��а��ˣ�
        /// </summary>
        /// <typeparam name="T">���ݰ�����</typeparam>
        /// <param name="packet">���ݰ�</param>
        public void SendToHost<T>(T packet) where T : Packet, new();

        /// <summary>
        /// ��ͼ���뷿�䣨����Ѿ����������·�����
        /// </summary>
        public UniTask Join();
    }
}
