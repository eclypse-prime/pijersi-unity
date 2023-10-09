public partial class Pijersi
{
    private void OnEnterPlayerTurn()
    {
        UpdateCameraPosition();
    }

    private void OnUpdatePlayerTurn()
    {
        if (!CheckPointedCell()) return;

        if ((mainAction.WasPressedThisFrame() || secondaryAction.WasPressedThisFrame()) && pointedCell.pieces[0]?.team == currentTeamId)
        {
            SM.ChangeState(State.Selection);
            return;
        }

        if (pointedCell != lastPointedCell)
            animation.UpdateHighlight(pointedCell);
    }
}
