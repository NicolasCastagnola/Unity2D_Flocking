using UnityEngine;
using System;
public class Food : GridEntity, IDestroyable
{
    private void Start()
    {
        grid = FindObjectOfType<SpatialGrid>();
        grid.AddEntity(this);
    }
    public void Destroy()
    {
        OnDestroy(this);
        Destroy(gameObject);
    }
}
