using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UserInterfaceManager : BaseMonoSingleton<UserInterfaceManager>
{
    [SerializeField] private List<UI_CollapsableContainer> _collapsableContainers;
    
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