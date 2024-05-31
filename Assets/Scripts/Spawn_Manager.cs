
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Spawn_Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    public GameObject _enemyContainer;
    public GameObject powerupContainer;
    public GameObject enemyLaserContainer;
    public GameObject laserStandardContainer;
    [SerializeField] private GameObject[] _powerupPrefabFrequent;
    [SerializeField] private GameObject[] _powerupPrefab;
    [SerializeField] private GameObject[] _powerupPrefabRare;
    [SerializeField] private float _waitTimePowerupFrequentMin = 2.0f;
    [SerializeField] private float _waitTimePowerupFrequentMax = 5.0f;
    [SerializeField] private float _waitTimePowerupNormalMin = 5.0f;
    [SerializeField] private float _waitTimePowerupNormalMax = 15.0f;
    [SerializeField] private float _waitTimePowerupRareMin = 15.0f;
    [SerializeField] private float _waitTimePowerupRareMax = 25.0f;
    public int afterLevelXNewEnemySpawn = 1;
    public int afterLevelXNewEnemyAngleSpawned = 6;
    public int afterLevelXStartEnemyShields = 1;
    float _yPositionLimit = 6f;
    float _xPositionLimit = 9.0f;
    float _randomX;
    float _waitTimePowerups = 7.0f;
    private bool _stopSpawning = false;
    private Game_Manager _gameManager;
    float _spawnZAngle = 0;
    float _waitTimeEnemy = 5.0f;
    float _waitTimeWaves = 7.0f;
    int _maxEnemiesSpawned = 1;

    private void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<Game_Manager>();
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL");
        }
        if (_enemyContainer == null)
        {
            Debug.LogError("The Enemy Container is NULL");
        }
        if (powerupContainer == null)
        {
            Debug.LogError("The Powerup Container is NULL");
        }
        if (enemyLaserContainer == null)
        {
            Debug.LogError("The Enemy Laser Container is NULL");
        }
        if (laserStandardContainer == null)
        {
            Debug.LogError("The Laser Container is NULL");
        }
    }

    private void Update()
    {
        
    }

    public void StartSpawning(int waveID)
    {
        _stopSpawning = false;
        GetWaveInfo(waveID);
        StartCoroutine(SpawnEnemyRoutine());
        if (waveID < 2)
        {
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefabFrequent, _waitTimePowerupFrequentMin, _waitTimePowerupFrequentMax));
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefab, _waitTimePowerupNormalMin, _waitTimePowerupNormalMax));
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefabRare, _waitTimePowerupRareMin, _waitTimePowerupRareMax));
        }
    }

    IEnumerator InitialPowerupDelay()
    {
        yield return new WaitForSeconds(10.0f);
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
        ClearEnemies();
    }

    private void ClearEnemies()
    {
        Debug.Log("Cleared enemies");
        Enemy[] _activeEnemies = _enemyContainer.GetComponentsInChildren<Enemy>();
        foreach (Enemy _enemy in _activeEnemies)
        {
            _enemy.ClearField();
        }
    }

    private void GetWaveInfo(int waveID)
    {
        switch (waveID)
        {
            case 1:
                _maxEnemiesSpawned = 2;
                _waitTimeEnemy = 3.5f;
                break;
            case 2:
                _maxEnemiesSpawned = 4;
                _waitTimeEnemy = 3.0f;
                break;
            case 3:
                _maxEnemiesSpawned = 8;
                _waitTimeEnemy = 2.5f;
                break;
            case 4:
                _maxEnemiesSpawned = 16;
                _waitTimeEnemy = 2.0f;
                break;
            case 5:
                _maxEnemiesSpawned = 32;
                _waitTimeEnemy = 1.0f;
                break;
            case 6:
                _maxEnemiesSpawned = 32;
                _waitTimeEnemy = 0.5f;
                break;
            case 7:
                _maxEnemiesSpawned = 32;
                _waitTimeEnemy = 3.5f;
                break;
            case 8:
                _maxEnemiesSpawned = 64;
                _waitTimeEnemy = 4.0f;
                break;
            case 9:
                _maxEnemiesSpawned = 64;
                _waitTimeEnemy = 6.0f;
                break;
            case 10:
                _maxEnemiesSpawned = 128;
                _waitTimeEnemy = 10.0f;
                break;
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        int _enemyIndex = 0;
        while ((_stopSpawning == false) && (_gameManager._isGameOver == false))
        {
            for (int i = 0; i < _maxEnemiesSpawned; i++)
            {
                yield return new WaitForSeconds(_waitTimeEnemy);
                if ((_stopSpawning == false) && (_gameManager._isGameOver == false))
                {
                    _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
                    Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);
                    if (_gameManager.waveID > afterLevelXNewEnemySpawn)
                    {
                        _enemyIndex = Random.Range(0, _enemyPrefab.Length);
                    }
                    if (_gameManager.waveID > afterLevelXNewEnemyAngleSpawned)
                    {
                        _spawnZAngle = Random.Range(-45f, 45f);
                    }
                    GameObject newEnemy = Instantiate(_enemyPrefab[_enemyIndex], spawnPosition, Quaternion.Euler(0, 0, _spawnZAngle));
                    newEnemy.transform.parent = _enemyContainer.transform;
                    if (_gameManager.waveID > afterLevelXStartEnemyShields)
                    {
                        newEnemy.GetComponent<Enemy>().enemyShieldsChances = 4;
                    }
                    if ((_stopSpawning == true) && (_gameManager._isGameOver == true))
                    {
                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(_waitTimeWaves); // change this later when we implement wave system.
        }
        
    }

    IEnumerator SpawnPowerupRoutine(GameObject[] _spawnList, float _waitTimeMin, float _waitTimeMax)
    {
        int _randomPowerupIndex = 0;
        while ((_stopSpawning == false) && (_gameManager._isGameOver == false))
        {
            _waitTimePowerups = Random.Range(_waitTimeMin, _waitTimeMax);
            yield return new WaitForSeconds(_waitTimePowerups);
            if ((_stopSpawning == false) && (_gameManager._isGameOver == false))
            {
                _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
                Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);
                _randomPowerupIndex = Random.Range(0, _spawnList.Length);
                GameObject newPowerup = Instantiate(_spawnList[_randomPowerupIndex], spawnPosition, Quaternion.identity);
                newPowerup.transform.parent = powerupContainer.transform;
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        StartCoroutine(SpawnEnemyRoutine());

        _gameManager.GameOver();
    }

}