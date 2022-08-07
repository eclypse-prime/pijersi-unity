using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public new Transform transform { get; private set; }

    public int team;
    public PieceType type;
    public PieceType prey;
    private void Awake()
    {
        transform = base.transform;
    }

    public void MoveTo(Cell cell, float y = 0f)
    {
        transform.position = cell.transform.position + Vector3.up * y;
    }
}
