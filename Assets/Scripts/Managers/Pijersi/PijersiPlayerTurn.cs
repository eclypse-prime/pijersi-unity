public partial class Pijersi
{
    private void OnEnterPlayerTurn()
    {
        cameraMovement.position = currentTeamId == 0 ? CameraMovement.PositionType.White : CameraMovement.PositionType.Black;
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
