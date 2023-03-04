using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Cell : MonoBehaviour
{
    public new MeshRenderer renderer;

    private Color baseColor;

    public new Transform transform { get; private set; }
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    public Cell[] nears { get; private set; }
    [HideInInspector] public Piece[] pieces = new Piece[2];

    public bool isEmpty => pieces[0] == null;
    public bool isFull => pieces[1] != null;
    public Piece lastPiece => isFull ? pieces[1] : pieces[0];


    private void Awake()
    {
        transform   = base.transform;
    }

    private void Start()
    {
        baseColor = renderer.material.color;
    }

    public void SetNears(Cell[] cells)
    {
        nears = cells;
    }

    public void SetColor(Color color)
    {
        renderer.material.color = color;
    }

    public void ResetColor()
    {
        SetColor(baseColor);
    }

    public Cell[] GetFarNears()
    {
        List<Cell> farNears = new List<Cell>();
        if (isFull)
        {
            for (int i = 0; i < nears.Length; i++)
            {
                if (nears[i]?.nears[i] != null)
                    farNears.Add(nears[i].nears[i]);
            }
        }

        return farNears.ToArray();
    }

    public int GetNearIndex(Cell cell)
    {
        for (int i = 0; i < nears.Length; i++)
        {
            if (cell == nears[i])
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Return the binary code of all pieces on this cell.
    /// </summary>
    public ushort PiecesToByte()
    {
        if (isEmpty) return 0;

        if (!isFull) return pieces[0].ToByte();

        return (ushort)(pieces[0].ToByte() + (pieces[1].ToByte() << 4));
    }
}
