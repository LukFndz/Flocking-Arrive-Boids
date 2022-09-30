using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float _spawnTime;
    [SerializeField] private GameObject _objectToSpawn;

    private float _currentSpawnTime;

    private void Start()
    {
        _currentSpawnTime = _spawnTime;
    }

    private void Update()
    {
        if (_objectToSpawn != null && _currentSpawnTime <= 0)
            Spawn();
        else
            _currentSpawnTime -= Time.deltaTime;
    }

    private void Spawn()
    {
        float x = Random.Range(-GameManager.Instance.BoundWidth, GameManager.Instance.BoundWidth);
        float z = Random.Range(-GameManager.Instance.BoundHeight, GameManager.Instance.BoundHeight);

        Instantiate(_objectToSpawn, new Vector3(x, 0, z), transform.rotation);

        _currentSpawnTime = _spawnTime;
    }
}
