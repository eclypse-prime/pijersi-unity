using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderDisplay : MonoBehaviour
{
    private TMP_Text text;

    public float Text
    {
        set
        {
            text.text = value.ToString();
        }
    }

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
}
