namespace FSM
{
    public class State<T>
    {
        public string Name { get; private set; }
        public T Id { get; private set; }

        public delegate void SimpleDelegate();

        public SimpleDelegate OnEnter;
        public SimpleDelegate OnExit;
        public SimpleDelegate OnUpdate;
        public SimpleDelegate OnFixedUpdate;

        public State(T id) => Id = id;
        public State(T id, string name) : this(id) => Name = name;
        public State(T id,
            SimpleDelegate onEnter,
            SimpleDelegate onExit = null,
            SimpleDelegate onUpdate = null,
            SimpleDelegate onFixedUpdate = null) : this(id)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnFixedUpdate = onFixedUpdate;
        }
        public State(T id,
            string name,
            SimpleDelegate onEnter,
            SimpleDelegate onExit = null,
            SimpleDelegate onUpdate = null,
            SimpleDelegate onFixedUpdate = null) : this(id, name)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnFixedUpdate = onFixedUpdate;
        }

        virtual public void Enter() => OnEnter?.Invoke();
        virtual public void Exit() => OnExit?.Invoke();
        virtual public void Update() => OnUpdate?.Invoke();
        virtual public void FixedUpdate() => OnFixedUpdate?.Invoke();
    }
}