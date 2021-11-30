using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramerateCounter : MonoBehaviour
{
    private int _frameCounter = 0;
    private float _timeCounter = 0.0f;
    private float _refreshTime = 0.1f;
    public TextMeshProUGUI textDisplay;

    private void Update()
    {
        Calculate();
    }
    void Calculate()
    {
        if (_timeCounter < _refreshTime)
        {
            _timeCounter += Time.deltaTime;
            _frameCounter++;
        }
        else
        {
            float lastFrame = _frameCounter / _timeCounter;
            _frameCounter = 0;
            _timeCounter = 0.0f;
            textDisplay.text = "FPS: " + lastFrame.ToString("00.0");
        }
    }
}
