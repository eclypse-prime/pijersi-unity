using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private new MeshRenderer renderer;

    private Color baseColor;

    public new Transform transform { get; private set; }
    public int x;
    public int y;
    public Piece[] pieces;

    public bool isEmpty => pieces[0] == null;
    public bool isFull => pieces[1] != null;
    public Piece lastPiece => isFull ? pieces[1] : pieces[0];


    private void Awake()
    {
        transform   = base.transform;
        pieces      = new Piece[2];
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
}
