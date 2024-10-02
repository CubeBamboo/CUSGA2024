using UnityEngine;

namespace Shuile.Framework
{
    [DefaultExecutionOrder(-500)]
    public abstract class SceneContainer : MonoContainer
    {
        public static SceneContainer Instance { get; private set; }

        public override void Awake()
        {
            if (IsInit) return;
            base.Awake();

            if (Instance != null)
            {
                Debug.LogWarning("Multiple SceneContainer detected.");
            }
            Instance = this;
        }

        public void MakeSureAwake()
        {
            if (!IsInit)
            {
                Awake();
            }
        }
    }
}
