using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FramerateCounter : MonoBehaviour
{
    [SerializeField] Slider fovSlider;

    public void OnFOVSliderValueChanged()
    {
        Camera.main.orthographicSize = fovSlider.value;
    }


}
