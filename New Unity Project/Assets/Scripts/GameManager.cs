using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI agentsDisplay;
    public TextMeshProUGUI hunterState;
    public TextMeshProUGUI hunterEnergy;

    public Flock flockManager;
    public Hunter hunter;

    public GameObject foodGo;
    private float spawnRate = 1f;
    bool _shouldSpawn = true;

    private void Start()
    {
        
    }
    private void Update()
    {
        agentsDisplay.text = "A_COUNT: " + flockManager.GetTotalAgents.Count.ToString();

        hunterState.text = "H_STATE: " + hunter.currentState;

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
