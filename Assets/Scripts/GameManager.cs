using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : BaseMonoSingleton<GameManager>
{
    private Camera _mainCamera;
    
    [TabGroup("UI")] public TextMeshProUGUI agentsDisplay;
    [TabGroup("UI")] public TextMeshProUGUI hunterState;
    [TabGroup("UI")] public TextMeshProUGUI hunterEnergy;
    [TabGroup("UI")] public Button respawnHunterButton;

    [TabGroup("Entities")] public Transform[] hunterWaypoints;
    [TabGroup("Entities")] public Flock flockManager;
    [TabGroup("Entities")] public Hunter hunter;

    [TabGroup("Prefabs")] public GameObject hunterPrefab;
    [TabGroup("Prefabs")] public GameObject foodPrefab;
    
    [SerializeField, TabGroup("Properties")] private float spawnRate = 1f;
    [SerializeField, TabGroup("Properties")] private bool _shouldSpawnFood = true;

    protected override void Awake()
    {
        base.Awake();

        _mainCamera = Camera.main;
    }

    private void Update()
    {
        agentsDisplay.text = "A_COUNT: " + flockManager.GetTotalAgents.Count;

        if (hunter != null)
        {
            hunterState.text = "H_STATE: " + hunter.currentStateDisplay;   
        }
        else
        {
            hunterState.text = "H_STATE: DIE DUE STARVATION";
            respawnHunterButton.gameObject.SetActive(true);
        }
                

        hunterEnergy.text = "H_ENERGY: " + hunter.energy.ToString("0.00");

        if (_shouldSpawnFood)
        {
            StartCoroutine(SpawnFoodTimer());
        }
    }


    IEnumerator SpawnFoodTimer()
    {
        _shouldSpawnFood = false;

        yield return new WaitForSeconds(spawnRate);

        SpawnFoodInsideScreenBounds();

        _shouldSpawnFood = true;
    }

    public void RespawnHunter()
    {
        Instantiate(hunterPrefab, transform).GetComponent<Hunter>().SetWaypoints(hunterWaypoints);
        
        respawnHunterButton.gameObject.SetActive(false);
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

        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }
}
