using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   public enum _enemyIDs
    {
        Standard,
        LaserBeam,
        SmartRearLaser,
        AvoidLaser
    }
    public _enemyIDs _enemyID;
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    private Game_Manager _gameManager;
    private Spawn_Manager _spawnManager;
    GameObject _laserSpawnPoint;
    GameObject _laserSpawnPointBack;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;
    private Player _player;
    private Animator _enemyAnimator;
    private float _explosionAnimLength = 0.0f;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _clipExplosion;
    [SerializeField] private AudioClip _laserClip;
    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFireAtTime = -1;
    private bool _isDestroyed = false;
    private bool _waveEnded = false;
    private float _randomWaitWavyTurn = -1;
    private GameObject _enemyLaserProjectile;
    private bool _laserBeamOn = false;
    public int afterLevelXLaserBeamEnemyWavyMove = 7;
    public int enemyShieldsChances = 0;
    [SerializeField] private bool _enemyShieldsActiveAlready = false;
    [SerializeField] private int _enemyShieldStrength = 0;
    [SerializeField] private GameObject _enemyShieldsOnEnemy;
    [SerializeField] private float _enemyShieldsScaling;
    [SerializeField] private Color _enemyShieldsColor;
    public int afterLevelXLaserBeamEnemyRamPlayerMove = 1;
    [SerializeField] private bool _aggressiveEnemy = false;
    public float _rammingDistance = 5.0f;
    private bool enemyBehindPlayer = false;
    [SerializeField] private float _avoidDistance = 3.0f;


    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<Game_Manager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _enemyAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if (_enemyAnimator == null)
        {
            Debug.LogError("Enemy Animator is NULL");
        }
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL");
        }
        _laserSpawnPoint = gameObject.transform.GetChild(0).gameObject;
        if ((_laserSpawnPoint == null) | (gameObject.transform.GetChild(0).name != "Laser_Spawn_Front"))
        {
            Debug.LogError("_laserSpawnPoint is NULL or not Laser_Spawn_Front");
        }
        switch (_enemyID)
        {
            case _enemyIDs.LaserBeam:
            case _enemyIDs.AvoidLaser:
                _explosionAnimLength = 0.0f;
                break;
            case _enemyIDs.SmartRearLaser:
                _explosionAnimLength = 0.0f;
                _laserSpawnPointBack = gameObject.transform.GetChild(1).gameObject;
                if ((_laserSpawnPointBack == null) | (gameObject.transform.GetChild(1).name != "Laser_Spawn_Back"))
                {
                    Debug.LogError("_laserSpawnPointBack is NULL or not Laser_Spawn_Back");
                }
                break;
            case _enemyIDs.Standard:
               
            default:
                if (_enemyShieldsOnEnemy == null)
                {
                    Debug.LogError("_enemyShieldsOnEnemy is NULL");
                }
                _explosionAnimLength = 2.6f;
                Invoke("ShieldsInitialize", 0.1f);
                DetermineIfPowerupInFront();
                break;
        }
    }

    private void ShieldsInitialize()
    {
        if ((enemyShieldsChances > 0) && (Random.Range(1, enemyShieldsChances) == 1) && (_enemyID == _enemyIDs.Standard))
        {
            ShieldsActive();
        }
    }

    private void Update()
    {
        switch (_enemyID)
        {
            case _enemyIDs.LaserBeam:
                DetermineEnemyAggression();
                if (!_aggressiveEnemy)
                {
                    if (_gameManager.waveID > afterLevelXLaserBeamEnemyWavyMove)
                    {
                        CalculateMovementWavy();
                    }
                    else
                    {
                        CalculateMovementStandard();
                    }
                }
                else if (_aggressiveEnemy)
                {
                    RamPlayer();
                }
                FireLaserBeam();
                break;
            case _enemyIDs.SmartRearLaser:
                CalculateMovementStandard();
                FireLaserBehind();
                break;
            case _enemyIDs.AvoidLaser:
                AvoidLaser();
                break;
            case _enemyIDs.Standard:
            default:
                CalculateMovementStandard();
                FireLaserNormal();
                break;
        }

    }

    private void CalculateMovementStandard()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        CalculateMovementAtScreenLimits();
    }

    private void CalculateMovementWavy()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        int _randomMultiplier = 1;
        if (Time.time > _randomWaitWavyTurn)
        {
            _randomWaitWavyTurn = Time.time + Random.Range(0f, 2f);
            _randomMultiplier = Random.Range(-45, 45);
        }
        float _newRotation = Mathf.Cos(Time.time * _randomMultiplier) * Time.deltaTime * 45f;
        transform.Rotate(0, 0, _newRotation);
        CalculateMovementAtScreenLimits();
    }

    private void CalculateMovementAvoidLaser(Laser _avoidThisLaser)
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        CalculateMovementAtScreenLimits();
    }

    private void CalculateMovementAtScreenLimits()
    {
        float _randomXPos = Random.Range(-_horizontalLimit, _horizontalLimit);
        if (transform.position.y <= -_verticalLimit)
        {
            transform.position = new Vector3(_randomXPos, _verticalLimit, 0);
            DetermineIfPowerupInFront();
        }
        if (Mathf.Abs(transform.position.x) > _horizontalLimit)
        {
            float _randomYPos = Random.Range(-_verticalLimit, _verticalLimit);
            transform.position = new Vector3(transform.position.x, _randomYPos, 0);
            DetermineIfPowerupInFront();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            DamageEnemy();
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(100);
            }
            DamageEnemy();
        }
    }

    private void FireLaserNormal()
    {
        if (Time.time > _canFireAtTime && _isDestroyed == false)
        {
            _fireRate = Random.Range(0f, 5f);
            _canFireAtTime = Time.time + _fireRate;
            GameObject _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPoint.transform.position, transform.rotation);
            _enemyLaser.transform.parent = _spawnManager.enemyLaserContainer.transform;
            Laser[] lasers = _enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].EnemyLaser();
            }
            _audioSource.clip = _laserClip;
            _audioSource.Play(0);
        }
    }

    private void FireLaserBeam()
    {
        if ((Time.time > _canFireAtTime) && (_isDestroyed == false) && (_laserBeamOn == false))
        {
            float _laserBeamDuration = 2.75f;
            _fireRate = Random.Range(10f, 20f);
            _canFireAtTime = Time.time + _fireRate;
            _audioSource.clip = _laserClip;
            _audioSource.Play(0);
            _audioSource.volume = 0.25f;
            StartCoroutine(LaserBeamOn(_laserBeamDuration));
        }
    }

    private void FireLaserBehind()
    {
        if (Time.time > _canFireAtTime && _isDestroyed == false)
        {
            _fireRate = 1.0f;
            _canFireAtTime = Time.time + _fireRate;
            DetermineIfBehind();
            if (enemyBehindPlayer)
            {
                GameObject _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPointBack.transform.position, Quaternion.Euler(0, 0, 100f));
                Laser[] lasers = _enemyLaser.GetComponentsInChildren<Laser>();
                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].EnemyLaser();
                }
                _audioSource.clip = _laserClip;
                _audioSource.Play(0);
            }
        }
    }

    IEnumerator LaserBeamOn(float Duration)
    {
        _laserBeamOn = true;
        _enemyLaserProjectile = Instantiate(_laserPrefab, _laserSpawnPoint.transform.position, transform.rotation);
        _enemyLaserProjectile.GetComponent<Laser>().EnemyLaser();
        _enemyLaserProjectile.transform.parent = transform;
        yield return new WaitForSeconds(Duration);
        LaserBeamOff();
    }

    private void LaserBeamOff()
    {
        Destroy(_enemyLaserProjectile);
        _laserBeamOn = false;
    }

    private void DamageEnemy()
    {
        if (_enemyShieldsActiveAlready)
        {
            ExplosionOnlyAnim();
            ShieldsNotActive();
        }
        else
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        _isDestroyed = true;
        _audioSource.clip = _clipExplosion;
        _audioSource.Play(0);
        switch (_enemyID)
        {
            case _enemyIDs.LaserBeam:
                ExplosionOnlyAnim();
                Destroy(GetComponentInChildren<Laser>());
                break;
            case _enemyIDs.SmartRearLaser:
            case _enemyIDs.AvoidLaser:
                ExplosionOnlyAnim();
                break;
            case _enemyIDs.Standard:
            default:
                _enemyAnimator.SetTrigger("OnEnemyDeath");
                break;
        }
        Destroy(GetComponent<Collider2D>());
        _speed = 0;
        Destroy(gameObject, _explosionAnimLength);
    }

    private void ExplosionOnlyAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 4.0f);
    }

    public void ClearField()
    {
        _canFireAtTime = -1;
        _waveEnded = true;
    }

    public void ShieldsActive()
    {
        _enemyShieldsOnEnemy.SetActive(true);
        _enemyShieldsActiveAlready = true;
        _enemyShieldStrength = 1;
    }

    public void ShieldsNotActive()
    {
        _enemyShieldsOnEnemy.SetActive(false);
        _enemyShieldsActiveAlready = false;
        _enemyShieldStrength = 0;
    }

    private void DetermineEnemyAggression()
    {
        if ((_gameManager.waveID > afterLevelXLaserBeamEnemyRamPlayerMove) && _player != null)
        {
            float _distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
            if (_distanceToPlayer < _rammingDistance)
            {
                _aggressiveEnemy = true;
            }
        }
        else
        {
            _aggressiveEnemy = false;
        }
    }

    private void AvoidLaser()
    {
        Vector2 _enemyMoveDirection = Vector2.down;
        Laser[] _trackLasers = _spawnManager.laserStandardContainer.GetComponentsInChildren<Laser>();
        for (int i = 0; i < _trackLasers.Length; i++)
        {
            Vector2 _directionToLaser = _trackLasers[i].transform.position - transform.position;
            if (Mathf.Abs(_directionToLaser.magnitude) < _avoidDistance)
            {
                _enemyMoveDirection = -_directionToLaser;
            }
        }
        transform.Translate(_enemyMoveDirection * _speed * Time.deltaTime);
        CalculateMovementAtScreenLimits();
    }

    private void RamPlayer()
    {
        if (_player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * 1.9f * Time.deltaTime);
        }
    }

    private void DetermineIfBehind()
    {
        if (_player != null)
        {
            Vector3 direction = _player.transform.position - transform.position;
            direction.Normalize();
            float checkBehind = Vector3.Dot(direction, -transform.up);
            if ((checkBehind > -0.75f) & (checkBehind < 0.0f))
            {
                enemyBehindPlayer = true;
            }
            else
            {
                enemyBehindPlayer = false;
            }
        }
    }

    private void DetermineIfPowerupInFront()
    {
        Powerup[] _allPowerups = transform.parent.parent.GetChild(1).GetComponentsInChildren<Powerup>();
        for (int i = 0; i < _allPowerups.Length; i++)
        {
            Vector3 directionPowerup = _allPowerups[i].transform.position - transform.position;
            directionPowerup.Normalize();
            float checkFront = Vector3.Dot(directionPowerup, -transform.up);
            if ((checkFront > -0.75f) & (checkFront < 0.0f))
            {
                Invoke("FireLaserNormal", 0.1f);
            }
        }
    }
}
