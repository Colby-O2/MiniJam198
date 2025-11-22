using System.Collections.Generic;
using MJ198.Math;
using UnityEngine;

namespace MJ198.MonoSystems
{
    public class SpawnerMonoSystem : MonoBehaviour, ISpawnerMonoSystem
    {
        [SerializeField] private float _enemyRadiusWhenSpawning = 0.5f;
        [SerializeField] private float _maxEnemiesAtOnce = 15;

        private List<GameObject> _enemies = new();

        private GameObject _enemyPrefab;
        
        private Bounds _spawnBounds;
            
        private TimedTrigger _spawnTrigger;
        private float _currentSpawnTime;
        
        
        
        private void Awake()
        {
            _spawnBounds = GameObject.FindWithTag("SpawnBounds").GetComponent<BoxCollider>().bounds;
            _enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        }

        private void Update()
        {
            if (_spawnTrigger.Try(_currentSpawnTime)) TrySpawn();
        }

        private void TrySpawn()
        {
            if (_enemies.Count >= _maxEnemiesAtOnce) return;
            if (TryGetSpawnLocation() is not {} pos) return;
            GameObject e = GameObject.Instantiate(_enemyPrefab, pos, Quaternion.identity);
            _enemies.Add(e);
            Quaternion.Look
        }

        Vector3? TryGetSpawnLocation()
        {
            const int maxTries = 100;
            for (int i = 0; i < maxTries; i++)
            {
                Vector3 pos = MathExt.RandomPointInBounds(_spawnBounds);
                if (IsValidLocation(pos, _enemyRadiusWhenSpawning))
                {
                    return pos;
                }
            }

            return null;
        }

        bool IsValidLocation(Vector3 position, float radius)
        {
            return !Physics.CheckSphere(position, radius);
        }
    }
}

