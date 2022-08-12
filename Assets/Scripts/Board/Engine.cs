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
        PijersiEngine.intArray arrayMove = new PijersiEngine.intArray(6);
        for (int i = 0; i < 6; i++)
        {
            arrayMove.setitem(i, move[i]);
        }
        board.playManual(arrayMove.cast());
    }

    public int[] PlayAuto()
    {
        PijersiEngine.SWIGTYPE_p_int pMove = board.playAuto();
        PijersiEngine.intArray arrayMove = PijersiEngine.intArray.frompointer(pMove);
        int[] move = new int[6];
        for (int i = 0; i < 6; i++)
        {
            move[i] = arrayMove.getitem(i);
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

    public string toString()
    {
        return board.toString();
    }
}