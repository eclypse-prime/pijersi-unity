using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PijersiConfig")]
public class PijersiConfig : ScriptableObject
{
    public PlayerType[] playerTypes;
    public string partyData;
    public int winMax;
}
