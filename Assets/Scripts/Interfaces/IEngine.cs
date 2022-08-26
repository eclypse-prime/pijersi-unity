interface IEngine
{
    public void PlayManual(int[] move);
    public int[] PlayAuto(int recursionDepth);
    public void SetState(byte[] newState);
    public int Evaluate();
    public bool CheckWin();
}