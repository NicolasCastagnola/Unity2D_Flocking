using System.Collections;
using System.Collections.Generic;
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
        
        // agentsDisplay.text = "A_COUNT: " + flockManager.GetTotalAgents.Count;
        //
        // // if (hunter != null)
        // {
        //     hunterState.text = "H_STATE: " + hunter.CurrentStateDisplay;   
        //     respawnHunterButton.gameObject.SetActive(false);
        // }
        // else
        // {
        //     hunterState.text = "H_STATE: DIE DUE STARVATION";
        //     respawnHunterButton.gameObject.SetActive(true);
        // }
        //         
        // hunterEnergy.text = "H_ENERGY: " + hunter.energy.ToString("0.00");

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
    
    public void SpawnHunter()
    {
        var newHunter = Instantiate(hunterPrefab, ActiveHunterWaypoints.GetRandom().position, Quaternion.identity)
                       .GetComponent<Hunter>()
                       .Initialize(ActiveHunterWaypoints, true);
        
        activeHunter.Add(newHunter);
    }
    private void SpawnFoodInsideScreenBounds()
    {
        float spawnY = Random.Range(
            _mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).y, 
            _mainCamera.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
    
        float spawnX = Random.Range(
            _mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).x, 
            _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
            

        var spawnPosition = new Vector2(spawnX, spawnY);

        Instantiate(foodPrefab, spawnPosition, Quaternion.identity, transform).GetComponent<Food>().Initialize(Random.Range(1f, 2f));
    }
}
