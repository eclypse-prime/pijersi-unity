public partial class Pijersi
{
    private void OnEnterNext()
    {
        int turnId   = save.turns.Count - 1;
        int actionId = save.turns[turnId].actions.Count;

        Save.Turn turn = replaySave.turns[turnId];
        selectedCell = turn.cells[actionId];
        pointedCell  = turn.cells[actionId + 1];

        switch (turn.actions[actionId])
        {
            case ActionType.Move:
            case ActionType.StackMove:
            case ActionType.Attack:
            case ActionType.StackAttack:
                SM.ChangeState(State.Move);
                break;
            case ActionType.Stack:
                SM.ChangeState(State.Stack);
                break;
            case ActionType.Unstack:
            case ActionType.UnstackAttack:
                SM.ChangeState(State.Unstack);
                break;
            case ActionType.None:
                break;
            default:
                break;
        }

        bool isReplayActionEnd = replayAt.Item1 == turnId && replayAt.Item2 == save.turns[turnId].actions.Count - 1;
        if (isReplayActionEnd)
        {
            bool isReplayEnd = replayAt.Item1 == replaySave.turns.Count - 1 && replayAt.Item2 == replaySave.turns[turnId].actions.Count - 1;
            if (isReplayEnd)
            {
                replaySave = null;
                replayState = ReplayState.None;
            }
            replayAt = (-1, -1);
        }
    }
}
