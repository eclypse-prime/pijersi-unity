public partial class Pijersi
{
    private void OnUpdatePlayAuto()
    {
        selectedCell = aiActionCells[0];
        pointedCell  = aiActionCells[1];

        SM.ChangeState(aiActionStates[0]);

        if (aiActionStates.Length < 2) return;

        // removes current action
        aiActionCells  = new Cell[] { aiActionCells[1], aiActionCells[2] };
        aiActionStates = new State[] { aiActionStates[1] };
    }
}
