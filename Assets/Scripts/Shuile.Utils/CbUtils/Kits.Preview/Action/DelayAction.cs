namespace CbUtils.ActionKit
{
    /*public class DelayAction : IAction
    {
        public struct DelayActionData
        {
            public float delayDuration;
            public System.Action onComplete;
            public GameObject linkedGameobject;
        }
        DelayActionData data;

        public DelayAction SetDuration(float duration)
        {
            data.delayDuration = duration;
            return this;
        }
        public DelayAction SetOnComplete(System.Action action)
        {
            data.onComplete += action;
            return this;
        }
        public DelayAction SetKillWhenGameObjectDestroy(GameObject go)
        {
            data.linkedGameobject = go;
            return this;
        }

        /// <summary> life time will be linked to gameObject </summary>
        public void Start(GameObject gameObject)
        {
            if(gameObject != null) data.linkedGameobject = gameObject;
            DotweenActionCtrlExecutor.Instance.Execute(data);
        }
        public void Start() => this.Start(null);
    }*/
}
