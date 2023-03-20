using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variables for game and game objects
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    private Spawn_Manager _spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn manager is null.");
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
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
    }
    public void Damage()
    {
        _lives--;
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }
}
