public partial class Pijersi
{
    private void OnUpdateReplay()
    {
        if (replayState != ReplayState.Play) return;

        Next(false);
    }
}
