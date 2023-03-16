using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    private bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Spawn an enemy every 5 seconds
    // Create a coroutine of type IEnumerator
    IEnumerator EnemySpawn()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new(UnityEngine.Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab,posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
