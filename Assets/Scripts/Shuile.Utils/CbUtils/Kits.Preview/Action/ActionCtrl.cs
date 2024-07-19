using System;

namespace CbUtils.ActionKit
{
    public interface IAction
    {
        void Start();
    }

    /// <summary>
    ///     control timing sequence logic
    /// </summary>
    [Obsolete("use TimingCtrl instead")]
    public class ActionCtrl : CSharpLazySingletons<ActionCtrl>
    {
        public static OldDelayAction Delay(float durationInSeconds)
        {
            var delay = new OldDelayAction { delayDuration = durationInSeconds };
            return delay;
        }
    }


    /*public class Sequence : IAction
    {
        private DG.Tweening.Sequence sequence;
        private List<IAction> actions = new List<IAction>();

        public void AppendCallback(System.Action action)
        {
            sequence.AppendCallback(() => action());
        }
        public void AppendInterval(float seconds)
        {
            sequence.AppendInterval(seconds);
        }

        public void Start(GameObject gameObject)
        {
            sequence = DOTween.Sequence();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }*/
}
