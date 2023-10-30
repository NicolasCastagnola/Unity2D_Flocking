using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
	public bool IsDestroying { get; private set; }
	public event Action<GridEntity> OnUpdatePosition;
    [BoxGroup("GridEntity")] public bool isOnGrid;
    protected void UpdatePosition() => OnUpdatePosition?.Invoke(this);
    private void OnDestroy() => IsDestroying = true;
}
