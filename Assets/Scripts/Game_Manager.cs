using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    public bool _isGameOver;
    private UI_Manager _uiManager;
    private Spawn_Manager _spawnManager;
    public int waveID = 0;
    public int waveLast = 10;
    private float _waveTime = 5.0f;
    private float _holdTime = 2.0f;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main_Menu" && _isGameOver == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(1);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void GameOver()
    {
        _isGameOver = true;
    }

    public void GameNotOver()
    {
        _isGameOver = false;
    }
    public void StartSpawning()
    {
        waveID++;
        _waveTime += 10f;
        if (waveID > waveLast)
        {
            Debug.Log("You win!");
            return;
        }
        _uiManager.WaveDisplayOn();
        _uiManager.WaveIDUpdate(waveID);
        StartCoroutine(WaveCountDown(_waveTime));
        _spawnManager.StartSpawning(waveID);
    }

    private IEnumerator WaveCountDown(float _time)
    {
        while ((_time > 0) && (!_isGameOver))
        {
            _time -= Time.deltaTime;
            _uiManager.WaveTimeUpdate(_time);
            yield return new WaitForEndOfFrame();
        }
        _spawnManager.StopSpawning();
        yield return _holdTime;
        StartSpawning();
    }
    
}
