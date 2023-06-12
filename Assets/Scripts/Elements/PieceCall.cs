using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Piece
{
    /// <summary>
    /// Init piece's move from his current position to his new cell.
    /// Applies a random offset to the destination position and rotation.
    /// </summary>
    /// <remarks>
    /// Cell.pieces must be updated before using this function.
    /// </remarks>
    /// <param name="cell">The destination cell.</param>
    /// <param name="rngPercent">Percent of rng applied to the destination position and rotation. Value between 0 and 1.</param>
    /// <param name="y">(optional) Y component of the destination position. Used for stack and unstack.</param>
    public void InitMove(Cell cell, float rngPercent, float y = float.NaN)
    {
        this.cell = cell;
        isJump = !float.IsNaN(y);

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].EndPosition : cell.Transform.position;

        Vector2 randPosition = maxPositionOffset * rngPercent * Random.insideUnitCircle;
        Vector3 positionOffset = new(randPosition.x, isJump ? y : Transform.position.y, randPosition.y);
        float angleOffset = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        startPosition = Transform.position;
        startRotation = Transform.rotation;
        EndPosition = position + positionOffset;
        endRotation = Quaternion.Euler(angleOffset * rngPercent * Vector3.up);
        startTime = Time.time;
    }

    /// <summary>
    /// Update piece move.
    /// </summary>
    /// <returns><list type="table">
    /// <item><term>True</term> the move isn't finished.</item>
    /// <item><term>False</term> the move is finished.</item>
    /// </list></returns>
    public bool UpdateMove()
    {
        if (startTime == 0f) return false;

        float step = Mathf.SmoothStep(0f, 1f, (Time.time - startTime) / moveDuration);
        Vector3 offset = isJump ? Vector3.up * jumpCurve.Evaluate(step) : Vector3.zero;
        Vector3 position = Vector3.Lerp(startPosition, EndPosition, step) + offset;
        Quaternion rotation = Quaternion.Lerp(startRotation, endRotation, step);
        Transform.SetPositionAndRotation(position, rotation);

        if (step >= 1f)
        {
            startTime = 0f;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Directly move this piece to the destination cell.
    /// </summary>
    /// <remarks></remarks>
    /// <inheritdoc cref="InitMove(Cell, float, float)"/>
    public void MoveTo(Cell cell, float rngPercent, float y = 0f)
    {
        this.cell = cell;

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].Transform.position : cell.Transform.position;

        Vector2 randPosition = Random.insideUnitCircle * maxPositionOffset * rngPercent;
        Vector3 positionOffset = new(randPosition.x, y, randPosition.y);
        float angleOffset = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        position += positionOffset;
        Quaternion rotation = Quaternion.Euler(angleOffset * rngPercent * Vector3.up);
        Transform.SetPositionAndRotation(position, rotation);
        EndPosition = position;
    }

    /// <summary>
    /// Returns all cells containing a piece that can eliminate this piece if it is on the designated cell.
    /// </summary>
    /// <param name="pieces">All pieces on the board.</param>
    /// <param name="cell">A cell where the piece can move through.</param>
    public virtual Cell[] GetDangers(Piece[] pieces, Cell cell)
    {
        // gets all hostile pieces
        IEnumerable<Piece> hostilePieces = pieces.Where(piece => piece.isActiveAndEnabled && piece.team != team && piece.prey == type);

        List<Cell> result = new();
        foreach (Piece hostilePiece in hostilePieces)
        {
            if (hostilePiece.CanAttack(this, cell))
                result.Add(hostilePiece.cell);
        }

        return result.Count > 0 ? result.ToArray() : null;
    }

    /// <summary>
    /// Returns all cells containing a piece that can eliminate this piece if it is on any designated cells.
    /// </summary>
    /// <param name="pieces">All pieces on the board.</param>
    /// <param name="cells">Cell list where the piece can move through.</param>
    public virtual Dictionary<Cell, Cell[]> GetDangers(Piece[] pieces, Cell[] cells)
    {
        IEnumerable<Piece> hostilePieces = pieces.Where(piece => piece.isActiveAndEnabled && piece.team != team && piece.prey == type);
        Dictionary<Cell, Cell[]> result = new();

        foreach (Cell cell in cells)
        {
            List<Cell> dangers = new();
            foreach (Piece hostilePiece in hostilePieces)
            {
                if (hostilePiece.CanAttack(this, cell))
                    dangers.Add(hostilePiece.cell);
            }

            if (dangers.Count > 0)
                result.Add(cell, dangers.ToArray());
        }

        return result.Count > 0 ? result : null;
    }

    /// <summary>
    /// Returns the binary code of this piece.
    /// </summary>
    public ushort ToByte()
    {
        ushort result = 1;
        if (team == 1)
            result += 2;
        switch (type)
        {
            case PieceType.Scissors:
                break;
            case PieceType.Paper:
                result += 4;
                break;
            case PieceType.Rock:
                result += 8;
                break;
            case PieceType.Wise:
                result += 12;
                break;
            default:
                break;
        }

        return result;
    }
}
