using UnityEngine;
using System;
public class Food : GridEntity, IDestroyable
{
    public void Initialize()
    {
        grid = FindObjectOfType<SpatialGrid>();
        grid.AddEntity(this);
        MoveCallback();
    }
    public void Destroy()
    {
        OnDestroy(this);
        //OnDestroy -= grid.RemoveEntity;
        Destroy(gameObject);
    }
}
