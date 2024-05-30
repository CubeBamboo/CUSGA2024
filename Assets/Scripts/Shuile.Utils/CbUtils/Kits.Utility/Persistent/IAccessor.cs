using Cysharp.Threading.Tasks;

namespace Shuile.Persistent
{
    public interface IAccessor<T> where T : PersistentData<T>, new()
    {
        /// <summary>
        /// 是否支持使用<see cref="SaveAsync{TProp}(string, TProp)"/>
        /// </summary>
        public bool IsRandomRWSupported { get; }

        /// <summary>
        /// 异步加载
        /// </summary>
        public UniTask<T> LoadAsync();

        /// <summary>
        /// 异步保存整个数据模型
        /// </summary>
        public UniTask SaveAsync(T data);

        /// <summary>
        /// 异步保存单条数据，需要<see cref="IsRandomRWSupported"/>返回true
        /// </summary>
        public UniTask SaveAsync<TProp>(string path, TProp propertyValue);
    }
}