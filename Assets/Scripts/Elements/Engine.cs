class Engine : IEngine
{
    private PijersiEngine.Board board;

    public Engine()
    {
        board = new PijersiEngine.Board();
        board.init();
    }

    /// <summary>
    /// Play a manual turn to the engine.
    /// </summary>
    /// <remarks>
    /// The turn need to be legal.
    /// </remarks>
    /// <param name="move">Index array of cells used in the turn.</param>
    public void PlayManual(int[] move)
    {
        uint uintMove = (uint) move[0];
        uintMove |= (uint)move[1] << 8;
        uintMove |= (uint)move[2] << 16;

        board.playManual(uintMove);
    }

    /// <summary>
    /// Play a turn with the engine and return it.
    /// </summary>
    /// <param name="recursionDepth">Number of turns used to predict the turn.</param>
    /// <returns>Index array of cells used in the turn.</returns>
    public int[] PlayAuto(int recursionDepth = 1)
    {
        uint uintMove = board.playDepth(recursionDepth);
        int[] move = new int[3];
        move[0] = (int)(uintMove & 0xFFU);
        move[1] = (int)((uintMove >> 8) & 0xFFU);
        move[2] = (int)((uintMove >> 16) & 0xFFU);

        return move;
    }

    /// <summary>
    /// Set the board state of the engine.
    /// </summary>
    public void SetState(byte[] state)
    {
        PijersiEngine.byteArray arrayState = new PijersiEngine.byteArray(45);
        for (int k = 0; k < 45; k++)
        {
            arrayState.setitem(k, state[k]);
        }
        board.setState(arrayState.cast());
    }

    public void SetPlayer(byte color)
    {
        board.currentPlayer = color;
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