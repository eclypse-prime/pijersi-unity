public enum ActionType
{
    None,
    Move,
    StackMove,
    Stack,
    Unstack,
    Attack,
    StackAttack,
    UnstackAttack
}

public static class ActionTypeExtensions
{
    public static bool IsStack(this ActionType action) =>
        action == ActionType.StackMove || action == ActionType.StackAttack;

    public static bool IsAttack(this ActionType action) =>
        action == ActionType.Attack || action == ActionType.StackAttack || action == ActionType.UnstackAttack;

    public static bool IsStackUnstack(this ActionType action) =>
        action == ActionType.Stack || action == ActionType.Unstack || action == ActionType.UnstackAttack;
}