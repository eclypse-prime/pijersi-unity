using System.Collections.Generic;

namespace FSM
{
    public class StateMachine<T>
    {
        protected Dictionary<T, State<T>> states;
        public State<T> CurrentState { get; protected set; }

        public StateMachine() => states = new Dictionary<T, State<T>>();

        public void Add(State<T> state) => states.Add(state.Id, state);
        public void Add(T stateId, State<T> state) => states.Add(stateId, state);

        public State<T> GetState(T stateId) => states.ContainsKey(stateId) ? states[stateId] : null;

        public void ChangeState(T stateId)
        {
            State<T> state = states[stateId];
            ChangeState(state);
        }

        public void ChangeState(State<T> state)
        {
            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();
        }

        public void Update() => CurrentState?.Update();
        public void FixedUpdate() => CurrentState?.FixedUpdate();
    }
}