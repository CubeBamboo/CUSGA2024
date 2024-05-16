using Cysharp.Threading.Tasks;

namespace Shuile.Persistent
{
    public interface IAccessor<T> where T : PersistentData<T>, new()
    {
        /// <summary>
        /// �Ƿ�֧��ʹ��<see cref="SaveAsync{TProp}(string, TProp)"/>
        /// </summary>
        public bool IsRandomRWSupported { get; }

        /// <summary>
        /// �첽����
        /// </summary>
        public UniTask<T> LoadAsync();

        /// <summary>
        /// �첽������������ģ��
        /// </summary>
        public UniTask SaveAsync(T data);

        /// <summary>
        /// �첽���浥�����ݣ���Ҫ<see cref="IsRandomRWSupported"/>����true
        /// </summary>
        public UniTask SaveAsync<TProp>(string path, TProp propertyValue);
    }
}