namespace CbUtils.Editor
{
    public class StateCheckDebugProperty
    {
        private enum State
        {
            ReadyCheck,
            CheckEnd
        }

        private State _state = State.CheckEnd;
        System.Func<bool> _check;

        public StateCheckDebugProperty(System.Func<bool> check)
        {
            _check = check;
        }

        public bool Check
        {
            get
            {
                if (!_check())
                    _state = State.ReadyCheck;
                return _state == State.ReadyCheck && _check();
            }
        }
    }
}
