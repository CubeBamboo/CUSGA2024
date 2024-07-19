using CbUtils.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CbUtils
{
    // inspired by qframework
    public interface IState
    {
        void Enter();
        void Exit();

        void FixedUpdate();
        void Update();

        void OnGUI();

        // judge whether to change state,
        // for example you want to switch to A State only when current state is B State
        bool Condition();

        void Custom(); // simple

        // use it anywhere you want, maybe can use string for label in the future
        void Custom(string label);
    }

    // container for state code
    /// <typeparam name="TStateId"> recommend to use Enum </typeparam>
    public class FSM<TStateId>
    {
        private IState _currentState;
        private TStateId _currentStateId;

        private TStateId _previousStateId;

        public Dictionary<TStateId, IState> States { get; } = new();

        public FSM() { }

        public FSM(TStateId stateId)
        {
            StartState(stateId);
        }

        public TStateId CurrentStateId => _currentStateId;

        /// <summary>
        ///     (old state, new state)
        /// </summary>
        public event Action<TStateId, TStateId> OnStateChanged = (_, _) => { }; // maybe for debug

        // recommend to using by class BaseState, to register states
        public void AddState(TStateId id, IState state)
        {
            States.Add(id, state);
        }

        private IState InnerCreateState()
        {
            return new EmptyState();
        }


        public void SwitchState(TStateId id, bool callLifeTimeEvent = true)
        {
            if (_currentStateId.Equals(id))
            {
                return;
            }

            if (!States.ContainsKey(id))
            {
                States[id] = InnerCreateState();
            }

            if (_currentState == null)
            {
                throw new Exception("FSM: Current State is null, maybe you forgot to call function FSM.StartState()");
            }

            if (!_currentState.Condition())
            {
                return;
            }

            if (callLifeTimeEvent)
            {
                _currentState.Exit();
            }

            _previousStateId = _currentStateId;
            _currentStateId = id;
            _currentState = States[id];
            OnStateChanged(_previousStateId, _currentStateId);
            if (callLifeTimeEvent)
            {
                _currentState.Enter();
            }
        }

        public void StartState(TStateId id)
        {
            _currentStateId = id;
            _currentState = States[id];
            _currentState.Enter();
        }

        public void Clear()
        {
            _currentStateId = default;
            _currentState = null;
            States.Clear();
        }

        // using by EventState, set it by chaining
        public EventState NewEventState(TStateId id)
        {
            if (States.ContainsKey(id))
            {
                return States[id] as EventState;
            }

            var state = new EventState();
            States[id] = state;
            return state;
        }

        // all LifeTime callback
        public void Enter()
        {
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState.Update();
        }

        public void FixedUpdate()
        {
            _currentState.FixedUpdate();
        }

        public void Exit()
        {
            _currentState.Exit();
        }

        public void OnGUI()
        {
            _currentState.OnGUI();
        }

        public void Custom()
        {
            _currentState.Custom();
        }

        public void Custom(string label = "")
        {
            _currentState.Custom(label);
        }
    }

    public abstract class BaseState<TState, TTarget> : IState
    {
        protected FSM<TState> fsm;

        private readonly Dictionary<string, Action> onCustomDict = new();
        protected TTarget target;

        public BaseState(FSM<TState> fsm, TTarget target)
        {
            this.fsm = fsm;
            this.target = target;
            InitCustom();
        }

        public virtual bool Condition() { return true; } // it's virtual

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void OnGUI() { }
        public virtual void Custom() { }

        public void Custom(string label)
        {
            onCustomDict.TryGetValue(label, out var action);
            action?.Invoke();
        }

        /// <summary>
        ///     use <seealso cref="AddCustom(string, System.Action)" /> in this method"/>
        /// </summary>
        public virtual void InitCustom() { }

        protected void AddCustom(string label, Action action)
        {
            if (onCustomDict.ContainsKey(label))
            {
                onCustomDict[label] = action;
            }
            else
            {
                onCustomDict.Add(label, action);
            }
        }
    }

    public class EmptyState : IState
    {
        public void Enter() { }
        public void Exit() { }
        public void FixedUpdate() { }
        public void Update() { }
        public void OnGUI() { }

        public bool Condition()
        {
            return true;
        }

        public void Custom() { }
        public void Custom(string label) { }
    }

    /// <summary>
    ///     state based on event
    /// </summary>
    public class EventState : IState
    {
        private Func<bool> onCondition;
        private Action onCustom;
        private readonly Dictionary<string, Action> onCustomDict = new();
        private Action onEnter;
        private Action onExit;
        private Action onFixedUpdate;
        private Action onGUI;
        private Action onUpdate;

        // IState Part
        public void Enter()
        {
            onEnter?.Invoke();
        }

        public void Exit()
        {
            onExit?.Invoke();
        }

        public void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        public void Update()
        {
            onUpdate?.Invoke();
        }

        public void OnGUI()
        {
            onGUI?.Invoke();
        }

        public bool Condition()
        {
            return onCondition == null || onCondition.Invoke();
        }

        public void Custom()
        {
            onCustom?.Invoke();
        }

        public void Custom(string label = "")
        {
            onCustomDict.TryGetValue(label, out var action);
            action?.Invoke();
        }

        public EventState OnEnter(Action action)
        {
            onEnter = action;
            return this;
        }

        public EventState OnExit(Action action)
        {
            onExit = action;
            return this;
        }

        public EventState OnFixedUpdate(Action action)
        {
            onFixedUpdate = action;
            return this;
        }

        public EventState OnUpdate(Action action)
        {
            onUpdate = action;
            return this;
        }

        public EventState OnGUI(Action action)
        {
            onGUI = action;
            return this;
        }

        public EventState OnCondition(Func<bool> action)
        {
            onCondition = action;
            return this;
        }

        public EventState OnCustom(Action action)
        {
            onCustom = action;
            return this;
        }

        public EventState OnCustom(Action action, string label)
        {
            if (onCustomDict.ContainsKey(label))
            {
                onCustomDict[label] = action;
            }
            else
            {
                onCustomDict.Add(label, action);
            }

            return this;
        }
    }
}
