using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PijersiConfig")]
public class PijersiConfig : ScriptableObject
{
    public GameType gameType;
    public int playerId;
    public int winRound;
}
