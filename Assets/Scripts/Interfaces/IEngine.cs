interface IEngine
{
    public void PlayManual(int[] move);
    public int[] PlayAuto(int recursionDepth);
    public void SetState(int[] colours, int[] tops, int[] bottoms);
    public int Evaluate();
    public bool CheckWin();
}