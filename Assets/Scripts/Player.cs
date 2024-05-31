using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speedDefault = 3.5f;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private int _lives = 3;
    private Vector3 _initPosition = new Vector3(0, -3.0f, 0);
    [SerializeField] private float verticalLimit = 5.0f;
    [SerializeField] private float horizontalLimit = 11.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    private Vector3 _laserOffset = new Vector3(0, 1.05f, 0);
    [SerializeField] private float _fireRate = 0.15f;
    private float _canFire = -1.0f;
    [SerializeField] private int _ammoCount = 25;
    [SerializeField] private int _maxAmmoCount = 25;
    Spawn_Manager _spawnManager;
    [SerializeField] private float _speedBoostMultiplierShift = 2.5f;
    [SerializeField] private bool _tripleShotActive = false;
    [SerializeField] private bool _wideShotActive = false;
    [SerializeField] private bool _speedBoostPowerupActive = false;
    [SerializeField] private bool _shieldsActive = false;
    [SerializeField] private int _shieldStrength = 0;
    [SerializeField] private GameObject _shieldOnPlayer;
    private SpriteRenderer _shieldOnPlayerSpriteRenderer;
    [SerializeField] private float[] _shieldScaling;
    [SerializeField] private Color[] _shieldColor;
    [SerializeField] private float _speedBoostMultiplierPowerup = 7.0f;
    [SerializeField] private GameObject _homingMissilePrefab;
    [SerializeField] private bool _homingMissile = false;
    private int _score = 0;
    UI_Manager _uiManager;
    [SerializeField] private GameObject _damageSmokeLeft;
    [SerializeField] private GameObject _damageSmokeRight;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioClip _noAmmoClip;
    private AudioSource _effectAudioSource;
    [SerializeField] private float _shiftKeyThrusterWaitTimeLimit = 3.0f;
    [SerializeField] private float _thrusterChargeLevelMax = 10.0f;
    [SerializeField] private float _thrusterChargeLevel;
    [SerializeField] private float _changeDecreaseThrusterChargeBy = 1.5f;
    [SerializeField] private float _changeIncreaseThrusterChargeBy = 0.001f;
    [SerializeField] private bool _canUseThrusters = true;
    [SerializeField] private bool _speedBoostShiftActive = false;
    private Main_Camera _mainCamera;

    private void Start()
    {
        transform.position = _initPosition;
        _thrusterChargeLevel = _thrusterChargeLevelMax;
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        _mainCamera = GameObject.Find("Main_Camera").GetComponent<Main_Camera>();
        _effectAudioSource = GetComponent<AudioSource>();
        _shieldOnPlayerSpriteRenderer = _shieldOnPlayer.GetComponent<SpriteRenderer>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }
        else
        {
            _uiManager.UpdateAmmo(_ammoCount, _maxAmmoCount);
        }
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera is NULL");
        }
        if (_effectAudioSource == null)
        {
            Debug.LogError("Effect Audio Source is NULL");
        }
        else
        {
            _effectAudioSource.clip = _laserSoundClip;
        }
        if (_shieldOnPlayerSpriteRenderer == null)
        {
            Debug.LogError("Shield On Player Sprite Renderer is NULL");
        }
        if (_tripleShotPrefab == null)
        {
            Debug.LogError("Triple Shot Prefab is NULL");
        }
        if (_homingMissilePrefab == null)
        {
            Debug.LogError("Homing Missile Prefab is NULL");
        }
    }

    private void Update()
    {
        _thrusterChargeLevel = Mathf.Clamp(_thrusterChargeLevel, 0, _thrusterChargeLevelMax);
        if (_thrusterChargeLevel <= 0.0f)
        {
            _canUseThrusters = false;
        }
        else if (_thrusterChargeLevel >= 0.0f)
        {
            _canUseThrusters = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canUseThrusters)
        {
            SpeedBoostActiveShift();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SpeedReset();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_spawnManager.powerupContainer != null)
            {
                Powerup[] _allPowerups = _spawnManager.powerupContainer.GetComponentsInChildren<Powerup>();
                if (_allPowerups.Length > 0)
                {
                    for (int i = 0; i < _allPowerups.Length; i++)
                    {
                        _allPowerups[i].MoveTowardsPlayerEnable();
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            if (_spawnManager.powerupContainer != null)
            {
                Powerup[] _allPowerups = _spawnManager.powerupContainer.GetComponentsInChildren<Powerup>();
                if (_allPowerups.Length > 0)
                {
                    for (int i = 0; i < _allPowerups.Length; i++)
                    {
                        _allPowerups[i].MoveTowardsPlayerDisable();
                    }
                }
            }
        }
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.deltaTime > _canFire)
        {
            Firelaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(_speed * Time.deltaTime * direction);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -verticalLimit, verticalLimit), 0);
        if (transform.position.x >= horizontalLimit)
        {
            transform.position = new Vector3(-horizontalLimit, transform.position.y, 0);
        }
        else if (transform.position.x <= -horizontalLimit)
        {
            transform.position = new Vector3(horizontalLimit, transform.position.y, 0);
        }
        if (_speedBoostShiftActive)
        {
            ThrustersActive();
        }
        else if (!_speedBoostShiftActive)
        {
            StartCoroutine(ThrustersPowerReplenishRoutine());
        }
    }

    void ThrustersActive()
    {
        if (_canUseThrusters == true)
        {
            _thrusterChargeLevel -= Time.deltaTime * _changeDecreaseThrusterChargeBy;
            _uiManager.UpdateThrusterSlider(_thrusterChargeLevel);
            if (_thrusterChargeLevel <= (_thrusterChargeLevelMax * 0.25))
            {
                _uiManager.ThrustersSliderUsableColor(false);
                if (_thrusterChargeLevel <= 0)
                {
                    _speedBoostShiftActive = false;
                    _canUseThrusters = false;
                    SpeedReset();
                }
            }
            else
            {
                _uiManager.ThrustersSliderUsableColor(true);
            }
        }
    }

    IEnumerator ThrustersPowerReplenishRoutine()
    {
        yield return new WaitForSeconds(_shiftKeyThrusterWaitTimeLimit);
        while ((_thrusterChargeLevel <= _thrusterChargeLevelMax) && !_speedBoostShiftActive)
        {
            _thrusterChargeLevel += Time.deltaTime * _changeIncreaseThrusterChargeBy;
            _uiManager.UpdateThrusterSlider(_thrusterChargeLevel);
            if (_thrusterChargeLevel >= (_thrusterChargeLevelMax * 0.25))
            {
                _uiManager.ThrustersSliderUsableColor(true);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void Firelaser()
    {
        _canFire = Time.time + _fireRate;
        GameObject _newLaser;
        if (_ammoCount > 0)
        {
            Vector3 _laserPosition = transform.position + _laserOffset;
            if (_tripleShotActive == true && (_wideShotActive == false & _homingMissile == false))
            {
                _newLaser = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
                _newLaser.transform.parent = _spawnManager.laserStandardContainer.transform;
            }
            else if (_wideShotActive == true & (_tripleShotActive == false & _homingMissile == false))
            {
                FireWideShot();
            }
            else if (_homingMissile == true & (_tripleShotActive == false & _wideShotActive == false))
            {
                FireMissile();
            }
            else
            {
                _newLaser = Instantiate(_laserPrefab, _laserPosition, Quaternion.identity);
                _newLaser.transform.parent = _spawnManager.laserStandardContainer.transform;
            }
            _ammoCount--;
            _uiManager.UpdateAmmo(_ammoCount, _maxAmmoCount);
            _effectAudioSource.clip = _laserSoundClip;
            _effectAudioSource.Play();
        }
    }

    void FireWideShot()
    {
        GameObject[] newMultiLasers = new GameObject[5];
        int _spawnAngle = -90;
        for (int i = 0; i < 5; i++)
        {
            newMultiLasers[i] = Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, _spawnAngle));
            newMultiLasers[i].transform.parent = _spawnManager.laserStandardContainer.transform;
            _spawnAngle += 45;
        }
    }

    void FireMissile()
    {
        GameObject _newMissile = Instantiate(_homingMissilePrefab, transform.position, Quaternion.identity);
        _newMissile.transform.parent = _spawnManager.laserStandardContainer.transform;
    }

    public void Damage()
    {
        if (!_shieldsActive)
        {
            _lives--;
            _uiManager.UpdateLives(_lives);
            UpdateSmokeDamage();
            _mainCamera.CameraShake();
            if (_lives < 1)
            {
                ExplosionAnim();
                _spawnManager.OnPlayerDeath();
                _uiManager.GameOverSequence();
                Destroy(gameObject);
            }
        }
        else
        {
            if (_shieldStrength >= 1)
            {
                --_shieldStrength;
            }
            if (_shieldStrength == 0)
            {
                _shieldsActive = false;
            }
            ShieldUpdateVisualization();
        }
    }

    public void TripleShotActive(float _duration)
    {
        _tripleShotActive = true;
        _wideShotActive = false;
        _homingMissile = false;
        StartCoroutine(TripleShotDuration(_duration));
    }

    IEnumerator TripleShotDuration(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _tripleShotActive = false;
    }

    public void WideShotActive(float _duration)
    {
        _tripleShotActive = false;
        _homingMissile = false;
        _wideShotActive = true;
        StartCoroutine(WideShotDuration(_duration));
    }

    IEnumerator WideShotDuration(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _wideShotActive = false;
    }

    public void HomingMissileActive()
    {
        _homingMissile = true;
        _wideShotActive = false;
        _tripleShotActive = false;
        StartCoroutine(HomingMissileDuration(20.0f));
    }

    IEnumerator HomingMissileDuration(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _homingMissile = false;
    }

    public void SpeedBoostActive(float _duration)
    {
        _speedBoostPowerupActive = true;
        _speed = _speedDefault * _speedBoostMultiplierPowerup;
        StartCoroutine(SpeedBoostDurationCoroutine(_duration));
    }

    IEnumerator SpeedBoostDurationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpeedReset();
    }

    public void SpeedBoostActiveShift()
    {
        if (_speedBoostPowerupActive == false)
        {
            _speedBoostShiftActive = true;
            _speed = _speedDefault * _speedBoostMultiplierShift;
        }
    }

    public void SpeedReset()
    {
        _speed = _speedDefault;
        _speedBoostPowerupActive = false;
        _speedBoostShiftActive = false;
    }

    public void ShieldsActive()
    {
        _shieldOnPlayer.SetActive(true);
        _shieldsActive = true;
        _shieldStrength = 3;
        ShieldUpdateVisualization();
    }

    private void ShieldUpdateVisualization()
    {
        if (_shieldStrength > 0)
        {
            _shieldOnPlayerSpriteRenderer.color = _shieldColor[_shieldStrength - 1];
            _shieldOnPlayer.transform.localScale = new Vector3(_shieldScaling[_shieldStrength - 1], _shieldScaling[_shieldStrength - 1], 1);
        }
        else
        {
            _shieldOnPlayer.SetActive(false);
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    private void ExplosionAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 2.7f);
    }

    public void AmmoFill()
    {
        _ammoCount = 25;
        _uiManager.UpdateAmmo(_ammoCount, _maxAmmoCount);
    }

    private void UpdateSmokeDamage()
    {
        if (_lives == 2)
        {
            _damageSmokeLeft.SetActive(true);
            _damageSmokeRight.SetActive(false);
        }
        else if (_lives == 1)
        {
            _damageSmokeLeft.SetActive(true);
            _damageSmokeRight.SetActive(true);
        }
        else
        {
            _damageSmokeLeft.SetActive(false);
            _damageSmokeRight.SetActive(false);
        }
    }

    public void AddShip()
    {
        if (_lives < 3)
        {
            _lives++;
            UpdateSmokeDamage();
            _uiManager.UpdateLives(_lives);
        }
    }

    public void NoAmmo()
    {
        _ammoCount = 0;
        _uiManager.UpdateAmmo(_ammoCount, _maxAmmoCount);
    }
}


