using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Rock,
    Paper,
    Scissors,
    Infinite
}

public class Piece : MonoBehaviour
{
    public new Transform transform { get; private set; }
    private Color baseColor;

    public int team;
    public PieceType type;

    private void Awake()
    {
        transform = base.transform;
    }
}
