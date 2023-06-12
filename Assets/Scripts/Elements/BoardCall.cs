using System;

public partial class Board
{
    public void ResetBoard()
    {
        foreach (Piece piece in Pieces)
        {
            piece.cell.pieces = new Piece[2];
            piece.gameObject.SetActive(true);
        }

        for (int i = 0; i < Pieces.Length; i++)
            MovePieceToCell(Pieces[i], Cells[CoordsToIndex(starter[i].x, starter[i].y)]);
    }

    public int CoordsToIndex(int x, int y)
    {
        if (x % 2 == 0)
            return (columnCount * 2 - 1) * x / 2 + y;

        return (columnCount * 2 - 1) * (x - 1) / 2 + columnCount - 1 + y;
    }

    public int CoordsToIndex(char x, char y) => 
        CoordsToIndex(lineCount - Array.FindIndex(letters, l => l == x) - 1, (int)char.GetNumericValue(y) - 1);

    public void Move(Cell start, Cell end, bool skipAnimation = false)
    {
        end.pieces = start.pieces;
        start.pieces = new Piece[2];

        if (skipAnimation)
        {
            end.pieces[0].MoveTo(end, PlacementRng);
            end.pieces[1]?.MoveTo(end, PlacementRng);
            return;
        }

        end.pieces[0].InitMove(end, PlacementRng);
        end.pieces[1]?.InitMove(end, PlacementRng);
    }

    public void Stack(Cell start, Cell end, bool skipAnimation = false)
    {
        int startId = start.IsFull ? 1 : 0;
        end.pieces[1] = start.pieces[startId];
        start.pieces[startId] = null;

        if (skipAnimation)
        {
            end.pieces[1].MoveTo(end, PlacementRng, PieceHeight);
            return;
        }

        end.pieces[1].InitMove(end, PlacementRng, PieceHeight);
    }

    public void Unstack(Cell start, Cell end, bool skipAnimation = false)
    {
        end.pieces[0] = start.pieces[1];
        end.pieces[1] = null;
        start.pieces[1] = null;

        if (skipAnimation)
        {
            end.pieces[0].MoveTo(end, PlacementRng, 0f);
            return;
        }

        end.pieces[0].InitMove(end, PlacementRng, 0f);
    }

    public bool UpdateMove(Cell cell)
    {
        bool firstUpdate = cell.pieces[0].UpdateMove();
        bool secondUpdate = cell.pieces[1]?.UpdateMove() == true;
        return firstUpdate || secondUpdate;
    }

    public void KillPieces(Cell cell)
    {
        if (cell.IsEmpty) return;

        cell.pieces[0].gameObject.SetActive(false);
        deadPieces.Add(cell.pieces[0]);

        if (!cell.IsFull) return;

        cell.pieces[1].gameObject.SetActive(false);
        deadPieces.Add(cell.pieces[1]);
    }

    public void ReviveLastPieces(Cell cell)
    {
        if (deadPieces.Count == 0) return;

        int lastId = deadPieces.Count - 1;

        Piece piece = deadPieces[lastId];
        if (piece.cell != cell) return;

        cell.pieces[0] = piece;
        piece.gameObject.SetActive(true);
        deadPieces.RemoveAt(lastId);

        bool noMorePiece = lastId < 1 || deadPieces[lastId - 1].cell != cell;
        if (noMorePiece) return;

        piece = deadPieces[lastId - 1];
        cell.pieces[1] = cell.pieces[0];
        cell.pieces[0] = piece;
        piece.gameObject.SetActive(true);
        deadPieces.RemoveAt(lastId - 1);
    }

    public byte[] GetState()
    {
        byte[] state = new byte[45];

        for (int i = 0; i < 45; i++)
            state[i] = (byte)Cells[i].PiecesToByte();

        return state;
    }
}
