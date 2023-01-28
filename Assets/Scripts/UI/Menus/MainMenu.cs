using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        GameManager.LoadScene(name);
    }

    public void Quit()
    {
        GameManager.Quit();
    }
}
