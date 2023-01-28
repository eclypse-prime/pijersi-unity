using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    [Range(30, 240, order = 5)]
    [SerializeField] private int maxFpsLimit;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = maxFpsLimit;
    }
}
