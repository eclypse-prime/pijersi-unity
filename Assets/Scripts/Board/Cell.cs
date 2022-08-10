using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[SelectionBase]
public class Cell : MonoBehaviour
{
    public new MeshRenderer renderer;

    private Color baseColor;

    public new Transform transform { get; private set; }
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    [ShowInInspector] public Cell[] nears { get; private set; }
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
        this.nears = cells;
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
}
