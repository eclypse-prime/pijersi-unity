using System;
using System.Collections.Generic;

namespace FSM
{
    public class StateMachine<T>
    {
        protected Dictionary<T, State<T>> states;
        public State<T> currentState { get; protected set; }

        public StateMachine()
        {
            states = new Dictionary<T, State<T>>();
        }

        public void Add(State<T> state)
        {
            states.Add(state.id, state);
        }

        public void Add(T stateId, State<T> state)
        {
            states.Add(stateId, state);
        }

        public State<T> GetState(T stateId)
        {
            return states.ContainsKey(stateId) ? states[stateId] : null;
        }

        public void ChangeState(T stateId)
        {
            State<T> state = states[stateId];
            ChangeState(state);
        }

        public void ChangeState(State<T> state)
        {
            currentState?.Exit();
            currentState = state;
            currentState?.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }
    }
}