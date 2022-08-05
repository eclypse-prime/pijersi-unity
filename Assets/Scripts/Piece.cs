using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum Type
    {
        Rock,
        Leaf,
        Scissors,
        Infinite
    }

    public Type type;
    public Cell cell;

    private new Transform transform;

    private void Awake()
    {
        transform = base.transform;
    }


}
