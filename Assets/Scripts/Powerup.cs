using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _powerupDuration = 7.0f;
    public enum _powerupIDs
    {
        TripleShot,
        SpeedBoost,
        Shields,
        Ammo,
        Health,
        WideShot,
        Homing,
        NoAmmo
    }
    [SerializeField] private _powerupIDs _powerupID;
    private float _verticalLimit = 7.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    private Spawn_Manager _spawnManager;
    private bool _moveTowardsPlayer = false;
    private Player _player;
    [SerializeField] private AudioClip _powerupAudioClip;
    [SerializeField] private AudioClip _explosionClip;
    private float _audioPositionZ = -9f;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    private void Update()
    {
        if (!_moveTowardsPlayer)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if ((_moveTowardsPlayer) & (_player != null))
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime * 2.0f);
        }
        if (transform.position.y <= -_verticalLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                if (_powerupID == _powerupIDs.NoAmmo)
                {
                    _audioPositionZ = -9.5f;
                }
                AudioSource.PlayClipAtPoint(_powerupAudioClip, new Vector3(0, 0, _audioPositionZ));
                switch (_powerupID)
                {
                    case _powerupIDs.TripleShot:
                        player.TripleShotActive(_powerupDuration);
                        break;
                    case _powerupIDs.SpeedBoost:
                        player.SpeedBoostActive(_powerupDuration);
                        break;
                    case _powerupIDs.Shields:
                        player.ShieldsActive();
                        break;
                    case _powerupIDs.Ammo:
                        player.AmmoFill();
                        break;
                    case _powerupIDs.Health:
                        player.AddShip();
                        break;
                    case _powerupIDs.WideShot:
                        player.WideShotActive(5.0f);
                        break;
                    case _powerupIDs.Homing:
                        player.HomingMissileActive();
                        break;
                    case _powerupIDs.NoAmmo:
                        player.NoAmmo();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogError("Player is NULL");
            }
            Destroy(gameObject);
        }
        else if (other.tag == "Laser")
        {
            AudioSource.PlayClipAtPoint(_explosionClip, new Vector3(0, 0, _audioPositionZ));
            ExplosionOnlyAnim();
            Destroy(gameObject);
        }
    }
    private void ExplosionOnlyAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 4.0f);
    }

    public void MoveTowardsPlayerEnable()
    {
        _moveTowardsPlayer = true;
    }

    public void MoveTowardsPlayerDisable()
    {
        _moveTowardsPlayer = false;
    }
}
