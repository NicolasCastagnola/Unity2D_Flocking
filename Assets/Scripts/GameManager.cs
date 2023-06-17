using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return _instance; } }
    private static GameManager _instance;
    
    
    public Transform[] hunterWaypoints;

    public TextMeshProUGUI agentsDisplay;
    public TextMeshProUGUI hunterState;
    public TextMeshProUGUI hunterEnergy;

    public Flock flockManager;

    public Hunter hunter;
    public GameObject hunterPrefab;
    public Button respawnHunter;

    public GameObject foodGo;
    private float spawnRate = 1f;
    bool _shouldSpawn = true;

    private void Awake()
    {
        _instance = this;
    }
    private void Update()
    {
        agentsDisplay.text = "A_COUNT: " + flockManager.GetTotalAgents.Count.ToString();

        if (hunter != null)
        {
            hunterState.text = "H_STATE: " + hunter.currentStateDisplay;   
        }
        else
        {
            hunterState.text = "H_STATE: DIE DUE STARVATION";
            respawnHunter.gameObject.SetActive(true);
        }
                

        hunterEnergy.text = "H_ENERGY: " + hunter.energy.ToString("0.00");

        if (_shouldSpawn)
        {
            StartCoroutine(SpawnFoodTimer());
        }
    }


    IEnumerator SpawnFoodTimer()
    {
        _shouldSpawn = false;

        yield return new WaitForSeconds(spawnRate);

        SpawnFood();

        _shouldSpawn = true;
    }

    public void RespawnHunter()
    {
        GameObject temp = Instantiate(hunterPrefab, transform);
        temp.GetComponent<Hunter>().SetWaypoints(hunterWaypoints);
        respawnHunter.gameObject.SetActive(false);
    }

    private void SpawnFood()
    {
        float spawnY = Random.Range(
            Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, 
            Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
    
        float spawnX = Random.Range(
            Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, 
            Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
            

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        Instantiate(foodGo, spawnPosition, Quaternion.identity);
    }
}
