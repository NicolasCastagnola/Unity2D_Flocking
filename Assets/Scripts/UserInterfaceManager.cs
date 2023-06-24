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
    
    [SerializeField, TabGroup("Sliders")] private Slider fovSlider;
    [SerializeField, TabGroup("Sliders")] private TMP_Text fovValue;
    
    [SerializeField] private List<AnimatedContainer> _collapsableContainers;
    [SerializeField] private Animator _animator;
    [SerializeField] private Button settingButtons;

    [SerializeField] private Flock flockManager;

    protected override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;
        fovSlider.onValueChanged.AddListener(UpdateQuantityValue);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        fovSlider.onValueChanged.RemoveListener(UpdateQuantityValue);
    }
    private void UpdateQuantityValue(float value)
    {
        fovValue.text = value.ToString(CultureInfo.InvariantCulture);
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