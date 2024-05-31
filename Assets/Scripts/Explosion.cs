using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    private void Start()
    {
        AudioSource.PlayClipAtPoint(_audioClip, new Vector3(0, 0, -9));
        Destroy(gameObject, 3.0f);
    }
}

