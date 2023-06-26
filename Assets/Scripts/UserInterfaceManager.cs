using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UserInterfaceManager : BaseMonoSingleton<UserInterfaceManager>
{
    private static readonly int Show = Animator.StringToHash("Show");
    private static readonly int Hide = Animator.StringToHash("Hide");
    
    private Camera mainCamera;
    private bool _isVisible = true;
    private bool _canInteract = true;
    
    [SerializeField, TabGroup("Slider Configurations")] private Slider fovSlider;
    [SerializeField, TabGroup("Sliders Values")] private TMP_Text fovValue;
    
    [SerializeField, TabGroup("Slider Configurations")] private Slider speedSlider;
    [SerializeField, TabGroup("Sliders Values")] private TMP_Text speedValue;
    
    [SerializeField, TabGroup("Slider Configurations")] private Slider neighborsRadiusSlider;
    [SerializeField, TabGroup("Sliders Values")] private TMP_Text neighborsRadiusValue;
    
    [SerializeField, TabGroup("Slider Configurations")] private Slider seekRadiusSlider;
    [SerializeField, TabGroup("Sliders Values")] private TMP_Text seekRadiusValue;
        
    [SerializeField, TabGroup("Slider Configurations")] private Slider avoidanceRadiusSlider;
    [SerializeField, TabGroup("Sliders Values")] private TMP_Text avoidanceRadiusValue;
    
    [SerializeField, TabGroup("Slider Configurations")] private Slider driveFactorSlider;
    [SerializeField, TabGroup("Sliders Values")] private TMP_Text driveFactorValue;
    
    [SerializeField] private List<AnimatedContainer> _collapsableContainers;
    [SerializeField] private Animator _animator;
    [SerializeField] private Button settingButtons;

    [SerializeField] private Flock flockManager;

    protected override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;
        
        fovSlider.onValueChanged.AddListener(UpdateQuantityValue);
        speedSlider.onValueChanged.AddListener(UpdateBoidsSpeed);
        neighborsRadiusSlider.onValueChanged.AddListener(UpdateNeighborsRadius);
        seekRadiusSlider.onValueChanged.AddListener(UpdateSeekRadius);
        avoidanceRadiusSlider.onValueChanged.AddListener(UpdateAvoidanceRadius);
        driveFactorSlider.onValueChanged.AddListener(UpdateDriveFactorRadius);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        neighborsRadiusSlider.onValueChanged.RemoveListener(UpdateNeighborsRadius);
        speedSlider.onValueChanged.RemoveListener(UpdateBoidsSpeed);
        fovSlider.onValueChanged.RemoveListener(UpdateQuantityValue);
        seekRadiusSlider.onValueChanged.RemoveListener(UpdateSeekRadius);
        avoidanceRadiusSlider.onValueChanged.RemoveListener(UpdateAvoidanceRadius);
    }
    private void UpdateDriveFactorRadius(float value)
    {
        driveFactorValue.text = value.ToString("0.0");
        flockManager.driveFactor = value;
    }
    private void UpdateAvoidanceRadius(float value)
    {
        avoidanceRadiusValue.text = value.ToString("0.0");
        flockManager.avoidanceRadiusMultiplier = value;
    }
    private void UpdateSeekRadius(float value)
    {
        seekRadiusValue.text = value.ToString("0.0");
        flockManager.seekRadiusMultiplier = value;
    }
    private void UpdateNeighborsRadius(float value)
    {
        neighborsRadiusValue.text = value.ToString("0.0");
        flockManager.neighborRadius = value;
    }
    private void UpdateBoidsSpeed(float value)
    {
        speedValue.text = value.ToString("0.0");
        flockManager.maxSpeed = value;
    }
    private void UpdateQuantityValue(float value)
    {
        fovValue.text = value.ToString("0.0");
        mainCamera.orthographicSize = value;
    }
    public void TriggerAnimator()
    {
        if (!_canInteract) return;

        _canInteract = false;
        settingButtons.interactable = false;
        StartCoroutine(SwapState());
    }
    private IEnumerator SwapState()
    {
        _isVisible = !_isVisible;
        _animator.Play(_isVisible ? Hide : Show);
        
        yield return new WaitForSeconds(1.5f);
        
        settingButtons.interactable = true;
        _canInteract = true;
    }

    public void CloseContainer(AnimatedContainer targetContainer)
    {
        if (_collapsableContainers.Count == 0) return;

        foreach (var container in _collapsableContainers.Where(container => targetContainer == container))
        {
            container.Close();
        }
    }
    public void OpenContainer(AnimatedContainer targetContainer)
    {
        if (_collapsableContainers.Count == 0) return;
        
        foreach (var container in _collapsableContainers)
        {
            if (targetContainer == container) container.Open();

            else container.Close();
        }
    }
}