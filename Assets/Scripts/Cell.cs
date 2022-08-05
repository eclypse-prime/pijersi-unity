using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public new Transform transform { get; private set; }
    private Color baseColor;

    public new MeshRenderer renderer;
    public int posX;
    public int posY;
    public Piece[] pieces = new Piece[2];


    private void Awake()
    {
        transform = base.transform;
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
