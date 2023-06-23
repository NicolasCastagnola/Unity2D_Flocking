using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : BaseMonoSingleton<GameManager>
{
    private Camera _mainCamera;
    
    [field:SerializeField] public SpatialGrid SpatialGrid { get; private set; }
    
    [TabGroup("UI")] public TextMeshProUGUI agentsDisplay;
    [TabGroup("UI")] public TextMeshProUGUI hunterState;
    [TabGroup("UI")] public TextMeshProUGUI hunterEnergy;
    [TabGroup("UI")] public Button respawnHunterButton;

    [TabGroup("Entities")] public Transform[] ActiveHunterWaypoints;
    [TabGroup("Entities")] public Flock flockManager;
    
    [ReadOnly, ShowInInspector] private List<Hunter> activeHunter = new List<Hunter>();
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
        {
            ActiveHunterWaypoints.AddRange(GetComponentsInChildren<Transform>());
        }
    }
    
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnHunter();
        }
        
        if (_shouldSpawnFood)
        {
            StartCoroutine(SpawnFoodTimer());
        }
    }


    private IEnumerator SpawnFoodTimer()
    {
        _shouldSpawnFood = false;

        yield return new WaitForSeconds(spawnRate);

        SpawnFoodInsideScreenBounds();

        _shouldSpawnFood = true;
    }

    [Button]
    private void SpawnHunter()
    {
        var newHunter = Instantiate(hunterPrefab, ActiveHunterWaypoints[0].position, Quaternion.identity)
                       .GetComponent<Hunter>()
                       .Initialize(ActiveHunterWaypoints);
        
        activeHunter.Add(newHunter);
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
