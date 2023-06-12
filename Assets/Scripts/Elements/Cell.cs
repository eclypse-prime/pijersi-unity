using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Cell : MonoBehaviour
{
    public new MeshRenderer renderer;

    private Color baseColor;

    public Transform Transform { get; private set; }
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    public Cell[] Nears { get; private set; }
    [HideInInspector] public Piece[] pieces = new Piece[2];

    public bool IsEmpty => pieces[0] == null;
    public bool IsFull => pieces[1] != null;
    public Piece LastPiece => IsFull ? pieces[1] : pieces[0];


    private void Awake()
    {
        Transform = transform;
    }

    private void Start()
    {
        baseColor = renderer.material.color;
    }

    public void SetNears(Cell[] cells) => Nears = cells;
    public void SetColor(Color color) => renderer.material.color = color;
    public void ResetColor() => SetColor(baseColor);

    /// <summary>
    /// Returns all cells that are aligned to the cell and its nears.
    /// </summary>
    public Cell[] GetDirectFarNears()
    {
        List<Cell> farNears = new();
        for (int i = 0; i < Nears.Length; i++)
        {
            if (Nears[i]?.Nears[i] != null)
                farNears.Add(Nears[i].Nears[i]);
        }

        return farNears.ToArray();
    }

    /// <summary>
    /// Returns the binary code of all pieces on this cell.
    /// </summary>
    public ushort PiecesToByte()
    {
        if (IsEmpty) return 0;

        if (!IsFull) return pieces[0].ToByte();

        return (ushort)(pieces[0].ToByte() + (pieces[1].ToByte() << 4));
    }
}
