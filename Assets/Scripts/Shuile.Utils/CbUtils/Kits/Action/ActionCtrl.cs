using DG.Tweening.Core;

namespace CbUtils.ActionKit
{
    public interface IAction
    {
        void Start();
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

    /// <summary>
    /// control timing sequence logic
    /// </summary>
    public class ActionCtrl: CSharpLazySingletons<ActionCtrl>
    {
        public static DelayAction Delay(float durationInSeconds)
        {
            var delay = new DelayAction
            {
                delayDuration = durationInSeconds
            };
            return delay;
        }
    }
}
