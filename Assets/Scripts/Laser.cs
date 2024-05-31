using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public enum laserIDs
    {
        Normal,
        LaserBeam
    }

    public laserIDs laserType;
    private int _speed = 8;
    private float _xLimit = 11.0f;
    private float _yLimit = 6.0f;
    private bool _isPlayerLaser = true;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    private void Update()
    {
        if (_isPlayerLaser)
        {
            MoveUp();
        }
        else
        {
            if (laserType == laserIDs.Normal)
            {
                MoveDown();
            }
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if ((Mathf.Abs(transform.position.y) > _yLimit) | (Mathf.Abs(transform.position.x) > _xLimit))
        {
            if (transform.parent.name == "Triple_Shot")
            {
                Destroy(transform.parent.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if ((transform.position.y > -_yLimit) | (Mathf.Abs(transform.position.x) > _xLimit))
        {
            if ((transform.parent != null) && (transform.parent.name != "EnemyLaserStandard_Container"))
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void PlayerLaser()
    {
        _isPlayerLaser = true;
    }

    public void EnemyLaser()
    {
        _isPlayerLaser = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isPlayerLaser == false)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            if (laserType == laserIDs.Normal)
            {
                ExplosionAnim(transform.position);
                Destroy(GetComponent<Collider2D>());
                Destroy(GetComponent<SpriteRenderer>());
                Destroy(gameObject, 2.7f);
            }
            else if (laserType == laserIDs.LaserBeam)
            {
                ExplosionAnim(other.transform.position);
            }
        }
    }

    private void ExplosionAnim(Vector3 _explosionPosition)
    {
        _explosionInstance = Instantiate(_explosionPrefab, _explosionPosition, transform.rotation);
        Destroy(_explosionInstance, 2.7f);
    }
}
