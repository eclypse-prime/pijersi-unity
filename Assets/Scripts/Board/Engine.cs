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
        PijersiEngine.IntVector vectorMove = new PijersiEngine.IntVector();
        for (int i = 0; i < 3; i++)
        {
            vectorMove.Add(move[i]);
        }
        board.playManual(vectorMove);
    }

    public int[] PlayAuto(int recursionDepth = 1)
    {
        PijersiEngine.IntVector vectorMove = board.playAlphaBeta(recursionDepth);
        int[] move = new int[3];
        for (int i = 0; i < 3; i++)
        {
            move[i] = vectorMove[i];
        }

        return move;
    }

    public void SetState(byte[] newState)
    {
        PijersiEngine.byteArray arrayState = new PijersiEngine.byteArray(45);
        for (int k = 0; k < 45; k++)
        {
            arrayState.setitem(k, newState[k]);
        }
        board.setState(arrayState.cast());
    }

    public void SetPlayer(byte colour)
    {
        board.currentPlayer = colour;
    }

    public float Evaluate()
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