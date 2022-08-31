interface IEngine
{
    public void PlayManual(int[] move);
    public int[] PlayAuto(int recursionDepth);
    public void SetState(byte[] newState);
    public void SetPlayer(int colour);
    public float Evaluate();
    public bool CheckWin();
}