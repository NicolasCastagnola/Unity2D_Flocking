using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    private const float REFRESH_TIME = 0.1f;
    private int _frameCounter;
    private float _timeCounter;

    public TextMeshProUGUI framesDisplay;
    public TextMeshProUGUI memoryDisplay;
    public TextMeshProUGUI currentBoidsDisplay;

    private void Update() => CalculateStats();
    private void CalculateStats()
    {
        if (_timeCounter < REFRESH_TIME)
        {
            _timeCounter += Time.deltaTime;
            _frameCounter++;
        }
        else
        {
            float lastFrame = _frameCounter / _timeCounter;
            _frameCounter = 0;
            _timeCounter = 0.0f;
            framesDisplay.text = "FPS: " + lastFrame.ToString("00.0");
        }

        memoryDisplay.text = $"Total Memory: {GC.GetTotalMemory(true) / (1024f * 1024f):f2} MB";
        
        currentBoidsDisplay.text = $"Total Boids: {GameManager.Instance.flockManager.GetTotalAgents.Count}";
    }
}
