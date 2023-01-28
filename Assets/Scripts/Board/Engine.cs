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
        uint uintMove = (uint) move[0];
        uintMove |= (uint)move[1] << 8;
        uintMove |= (uint)move[2] << 16;

        board.playManual(uintMove);
    }

    public int[] PlayAuto(int recursionDepth = 1)
    {
        uint uintMove = board.playDepth(recursionDepth);
        int[] move = new int[3];
        move[0] = (int)uintMove & 0b_1111;
        move[1] = (int)uintMove >> 8 & 0b_1111;
        move[2] = (int)uintMove >> 16;

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