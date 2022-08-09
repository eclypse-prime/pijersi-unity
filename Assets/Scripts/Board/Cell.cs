using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public new MeshRenderer renderer;

    private Color baseColor;

    public new Transform transform { get; private set; }
    [HideInInspector] public int x;
    [HideInInspector] public int y;
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

    public void SetColor(Color color)
    {
        renderer.material.color = color;
    }

    public void ResetColor()
    {
        SetColor(baseColor);
    }

    #region get
    public Vector2Int[] GetPossibleNeighbours()
    {
        int yOffset = -1;
        if (x % 2 == 0)
            yOffset = 1;

        Vector2Int[] result = new Vector2Int[6];
        result[0] = new Vector2Int(x + 0, y - 1);
        result[1] = new Vector2Int(x + 0, y + 1);
        result[2] = new Vector2Int(x - 1, y + 0);
        result[3] = new Vector2Int(x + 1, y + 0);
        result[4] = new Vector2Int(x - 1, y + yOffset);
        result[5] = new Vector2Int(x + 1, y + yOffset);

        return result;
    }
    #endregion
}
