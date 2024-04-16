using System.Collections;

namespace CbUtils
{
    public class MonoActionCtrlExecutor : MonoSingletons<MonoActionCtrlExecutor>
    {
        protected override void OnAwake()
            => SetDontDestroyOnLoad();

        public void ExecuteCoroutine(IEnumerator coroutine)
            => StartCoroutine(coroutine);
    }
}
