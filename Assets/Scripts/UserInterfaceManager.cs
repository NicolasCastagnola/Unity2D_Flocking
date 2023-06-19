using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UserInterfaceManager : BaseMonoSingleton<UserInterfaceManager>
{
    private static readonly int Show = Animator.StringToHash("Show");
    private static readonly int Hide = Animator.StringToHash("Hide");
    private bool isVisible = true;
    
    [SerializeField] private List<UI_CollapsableContainer> _collapsableContainers;
    [SerializeField] private Animator _animator;

    public void TriggerAnimator()
    {
        isVisible = !isVisible;
        
        _animator.Play(isVisible ? Hide : Show);
    }

    public void CollapseContainer(UI_CollapsableContainer targetContainer)
    {
        if (_collapsableContainers.Count == 0) return;

        foreach (var container in _collapsableContainers.Where(container => targetContainer == container))
        {
            container.Collapse();
        }
    }
    public void OpenContainer(UI_CollapsableContainer targetContainer)
    {
        if (_collapsableContainers.Count == 0) return;
        
        foreach (var container in _collapsableContainers)
        {
            if (targetContainer == container) container.Open();

            else container.Collapse();
        }
    }
}