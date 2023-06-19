using System;
using UnityEngine;
public class Food : GridEntity, IDestroyable
{
    public Food Initialize(float sizeMultiplier = 1)
    {
        transform.localScale *= sizeMultiplier;
        
        GameManager.Instance.SpatialGrid.RegisterEntity(this);

        return this;
    }
    public void Update() => UpdatePosition();
    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}
