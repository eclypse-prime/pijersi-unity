using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PijersiConfig")]
public class PijersiConfig : ScriptableObject
{
    public PlayerType[] playerTypes;
    public int winMax;
}
