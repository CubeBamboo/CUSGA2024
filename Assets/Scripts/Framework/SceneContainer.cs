using UnityEngine;

namespace Shuile.Framework
{
    [DefaultExecutionOrder(-500)]
    public abstract class SceneContainer : MonoContainer
    {
        public static SceneContainer Instance { get; private set; }
        public static RuntimeContext GlobalSceneExtraContext { get; set; }

        public override void Awake()
        {
            base.Awake();

            if (Instance != null)
            {
                Debug.LogWarning("Multiple SceneContainer detected.");
            }
            Instance = this;
        }

        protected override void OnInitContainer()
        {
            if(GlobalSceneExtraContext != null) Context.AddParent(GlobalSceneExtraContext);
            base.OnInitContainer();
        }
    }
}
