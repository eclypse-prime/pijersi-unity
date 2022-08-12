interface IEngine
{
    public void PlayManual(int[] move);
    public int[] PlayAuto();
    public void SetState(int[] colours, int[] tops, int[] bottoms);
}