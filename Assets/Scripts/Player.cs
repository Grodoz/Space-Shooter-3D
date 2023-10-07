using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variables for game and game objects
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMult = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    private Spawn_Manager _spawnManager;
    private bool _isTripleShot;
    private bool _isSpeedBoost;
    private bool _isShield;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private int _score;
    private UI_Manager _uiManager;
    [SerializeField] private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        _audioSource = GetComponent<AudioSource>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn manager is null.");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is null.");
        }
        if (_audioSource == null)
        {
            Debug.LogError("Audio source for the player is null!");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            ShootLaser();
        }
        
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new(horizontalInput, verticalInput, 0);
        
        transform.Translate(_speed * Time.deltaTime * direction);
        

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void ShootLaser()
    {
        // player hits space key, spawn the laser object
        
        _nextFire = Time.time + _fireRate;
       
        if (_isTripleShot == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShield == true)
        {
            _isShield = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        _lives--;
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }
    public void TripleShotActive()
    {
        _isTripleShot = true;
        StartCoroutine(TripleShotPowerDown());

    }
    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShot = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoost = true;
        _speed *= _speedMult;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoost = false;
        _speed /= _speedMult;
    }
    public void ShieldActive()
    {
        _isShield = true;
        _shieldVisualizer.gameObject.SetActive(true);
        StartCoroutine(ShieldPowerDown());
    }
    IEnumerator ShieldPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isShield = false;
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
