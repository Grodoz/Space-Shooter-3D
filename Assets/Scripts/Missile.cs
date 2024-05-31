using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

public class Missile : MonoBehaviour
{
    public enum missileIDs
    {
        Normal,
        Homing
    }
    public missileIDs MissileType;
    private float _speed = 5.0f;
    private float _xLimit = 11.0f;
    private float _yLimit = 6.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    private Spawn_Manager _spawnManager;
    [SerializeField] float _nearestEnemyDistance = 200.0f;
    [SerializeField] private Enemy _nearestEnemy;
    [SerializeField] Enemy[] _allEnemies = new Enemy[1];

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _allEnemies = _spawnManager._enemyContainer.GetComponentsInChildren<Enemy>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if (_allEnemies.Length > 0)
        {
            StartCoroutine(GetNearestEnemy(_allEnemies));
        }
    }

    private void Update()
    {
        switch (MissileType)
        {
            case missileIDs.Homing:
                MoveHomingMissile();
                break;
            case missileIDs.Normal:
            default:
                MoveUp();
                break;
        }
        _allEnemies = _spawnManager._enemyContainer.GetComponentsInChildren<Enemy>();
        if (_allEnemies.Length > 0)
        {
            StartCoroutine(GetNearestEnemy(_allEnemies));
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        DestroyAtScreenLimits();
    }

    void MoveHomingMissile()
    {
        if (_nearestEnemy != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _nearestEnemy.transform.position, _speed * Time.deltaTime);
            Vector3 diff = _nearestEnemy.transform.position - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        else
        {
            MoveUp();
        }
    }

    void DestroyAtScreenLimits()
    {
        if ((Mathf.Abs(transform.position.y) > _yLimit) | (Mathf.Abs(transform.position.x) > _xLimit))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator GetNearestEnemy(Enemy[] _allEnemies)
    {
        _nearestEnemy = _allEnemies[0];
        if (_allEnemies.Length > 1)
        {
            Vector2 _distanceToEnemy = _allEnemies[0].transform.position - transform.position;
            float _previousDistance = Mathf.Abs(_distanceToEnemy.magnitude);
            for (int i = 1; i < _allEnemies.Length; i++)
            {
                _distanceToEnemy = _allEnemies[i].transform.position - transform.position;
                if ((Mathf.Abs(_distanceToEnemy.magnitude) < _nearestEnemyDistance) & (Mathf.Abs(_distanceToEnemy.magnitude) < _previousDistance))
                {
                    _nearestEnemy = _allEnemies[i];
                    _previousDistance = Mathf.Abs(_distanceToEnemy.magnitude);
                }
            }
        }
        yield return null;
    }
}
