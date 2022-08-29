interface IEngine
{
    public void PlayManual(int[] move);
    public int[] PlayAuto(int recursionDepth);
    public void SetState(byte[] newState);
    public float Evaluate();
    public bool CheckWin();
}