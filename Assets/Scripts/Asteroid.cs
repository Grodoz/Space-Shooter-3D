using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    private Spawn_Manager _spawnManager;
    private Player _player;
    private Game_Manager _gameManager;
    private UI_Manager _uiManager;
    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<Game_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            ExplosionAnim();
            _gameManager.StartSpawning();
        }
        else if (other.tag == "Player")
        {
            ExplosionAnim();
            _player.Damage();
            _gameManager.StartSpawning();
        }
        Destroy(gameObject, 0.0f);
    }

    private void ExplosionAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 4.0f);
    }
}

