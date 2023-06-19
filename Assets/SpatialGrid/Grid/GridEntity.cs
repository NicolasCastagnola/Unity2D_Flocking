using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnUpdatePosition;
    [BoxGroup("GridEntity")] public bool isOnGrid;
    protected void UpdatePosition() => OnUpdatePosition?.Invoke(this);

}
