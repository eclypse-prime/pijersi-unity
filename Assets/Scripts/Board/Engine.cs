class Engine : IEngine
{
    private PijersiEngine.Board board;

    public Engine()
    {
        board = new PijersiEngine.Board();
        board.init();
    }

    public void PlayManual(int[] move)
    {
        PijersiEngine.IntVector vectorMove = new PijersiEngine.IntVector(6);
        for (int i = 0; i < 6; i++)
        {
            vectorMove[i] = move[i];
        }
        board.playManual(vectorMove);
    }

    public int[] PlayAuto(int recursionDepth = 1)
    {
        PijersiEngine.IntVector vectorMove = board.playAuto(recursionDepth);
        int[] move = new int[6];
        for (int i = 0; i < 6; i++)
        {
            move[i] = vectorMove[i];
        }

        return move;
    }

    public void SetState(int[] colours, int[] tops, int[] bottoms)
    {
        PijersiEngine.intArray arrayColours = new PijersiEngine.intArray(45);
        PijersiEngine.intArray arrayTops = new PijersiEngine.intArray(45);
        PijersiEngine.intArray arrayBottoms = new PijersiEngine.intArray(45);
        for (int i = 0; i < 45; i++)
        {
            arrayColours.setitem(i, colours[i]);
            arrayTops.setitem(i, tops[i]);
            arrayBottoms.setitem(i, bottoms[i]);
        }
        board.setState(arrayColours.cast(), arrayTops.cast(), arrayBottoms.cast());
    }

    public int Evaluate()
    {
        return board.evaluate();
    }

    public bool CheckWin()
    {
        return board.checkWin();
    }

    public override string ToString()
    {
        return board.toString();
    }
}