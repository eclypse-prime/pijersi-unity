using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameInfo")]
public class GameInfo : ScriptableObject
{
    public int currentTeam;
    public Cell currentCell;
    public Piece currentPiece;
}
