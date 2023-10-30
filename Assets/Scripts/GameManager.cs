using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Utilities;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class GameManager : BaseMonoSingleton<GameManager>
{
    private Camera _mainCamera;
    [field:SerializeField] public SpatialGrid SpatialGrid { get; private set; }
    
    [TabGroup("UI")] public TextMeshProUGUI agentsDisplay;

    [SerializeField] private Slider hunterSpeedValueSlider;

    [TabGroup("Entities")] public Transform[] ActiveHunterWaypoints;
    [TabGroup("Entities")] public Flock flockManager;
    
    [ReadOnly, ShowInInspector] private List<Hunter> activeHunters = new List<Hunter>();
    [ReadOnly, ShowInInspector] private List<Food> activeFood = new List<Food>();

    [TabGroup("Prefabs")] public GameObject hunterPrefab;
    [TabGroup("Prefabs")] public GameObject foodPrefab;
    
    [SerializeField, TabGroup("Properties")] private float spawnRate = 1f;
    [SerializeField, TabGroup("Properties")] private bool _shouldSpawnFood = true;

    protected override void Awake()
    {
        base.Awake();

        _mainCamera = Camera.main;

        if (ActiveHunterWaypoints.Length == 0 || ActiveHunterWaypoints == null)
            ActiveHunterWaypoints.AddRange(GetComponentsInChildren<Transform>());

        hunterSpeedValueSlider.onValueChanged.AddListener(SetHunterSpeed);
    }

    protected override void OnDestroy()
    {
        hunterSpeedValueSlider.onValueChanged.RemoveListener(SetHunterSpeed);
        base.OnDestroy();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SpawnHunter();
        
        if (_shouldSpawnFood) StartCoroutine(SpawnFoodTimer());
    }

    public void ShuffleWaypoints() => ActiveHunterWaypoints.Shuffle();
    private IEnumerator SpawnFoodTimer()
    {
        _shouldSpawnFood = false;

        yield return new WaitForSeconds(spawnRate);

        SpawnFoodInsideScreenBounds();

        _shouldSpawnFood = true;
    }

    public void SetHunterSpeed(float value)
    {
        foreach (var hunter in activeHunters)
        {
            hunter.speed = value;
        }
    }
    
    public void ClearAllHunters()
    {
        foreach (var hunter in activeHunters)
        {
            hunter.Terminate();
            
            SpatialGrid.UnRegisterEntity(hunter);
            
            Destroy(hunter);
        }
        
        activeHunters.Clear();
    }

    [Button]
    public void SpawnHunter()
    {
        var newHunter = Instantiate(hunterPrefab, ActiveHunterWaypoints[0].position, Quaternion.identity)
                       .GetComponent<Hunter>()
                       .Initialize(ActiveHunterWaypoints);
        
        activeHunters.Add(newHunter);
    }
    [Button]
    private void SpawnFoodInsideScreenBounds()
    {
        float spawnY = Random.Range(_mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).y, 
                                    _mainCamera.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
    
        float spawnX = Random.Range(_mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).x, 
                                    _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        var newFood = Instantiate(foodPrefab, new Vector2(spawnX, spawnY), Quaternion.identity, transform)
           .GetComponent<Food>()
           .Initialize(Random.Range(1f, 2f));
        
        activeFood.Add(newFood);
    }
}
