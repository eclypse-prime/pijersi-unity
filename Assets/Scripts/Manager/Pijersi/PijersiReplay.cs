public partial class Pijersi
{
    public void OnEnterReplay() {}

    public void OnExitReplay() { }

    public void OnUpdateReplay()
    {
        if (replayState != ReplayState.Play) return;

        Next(false);
    }
}
