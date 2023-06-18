using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnMove;
    public Action<GridEntity> OnDestroy = delegate { };
    [BoxGroup("GridEntity")] public bool isOnGrid;

	public SpatialGrid grid;

    private void Awake()
    {
       // grid = GetComponentInParent<SpatialGrid>();
       // OnDestroy += grid.RemoveEntity;
    }

    private void Start()
    {
        if(grid != null)
            grid.AddEntity(this);
        //MoveCallback();
    }
    //    private Renderer _renderer;
    //    private void Awake() => _renderer = GetComponent<Renderer>();
    //    public void Update() 
    //    {
    //        // _renderer.material.color = onGrid ? Color.red : Color.gray;
    // 	//Optimization: Hacer esto solo cuando realmente se mueve y no en el update
    //     // OnMove(this);
    // }
    protected void MoveCallback() => OnMove?.Invoke(this);


}
