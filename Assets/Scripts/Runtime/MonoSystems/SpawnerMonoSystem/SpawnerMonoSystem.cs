using System.Collections.Generic;
using MJ198.Math;
using MJ198.Player;
using UnityEngine;
using UnityEngine.Events;

namespace MJ198.MonoSystems
{
    public class SpawnerMonoSystem : MonoBehaviour, ISpawnerMonoSystem
    {
        [SerializeField] private float _enemyRadiusWhenSpawning = 0.5f;
        [SerializeField] private float _maxEnemiesAtOnce = 15;
        [SerializeField] private float _startSpawnTime = 5;
        [SerializeField] private float _biasStrength = 0.3f;

        private List<Enemy.Manager> _enemies = new();

        private GameObject _enemyPrefab;
        
        private Bounds _spawnBounds;
            
        private TimedTrigger _spawnTrigger = new();
        private float _currentSpawnTime;

        private Player.Controller _player;
        
        private void Start()
        {
            _currentSpawnTime = _startSpawnTime;
            BoxCollider bc = GameObject.FindWithTag("SpawnBounds").GetComponent<BoxCollider>();
            _spawnBounds = new Bounds(
                bc.transform.position,
                bc.size.Mul(bc.transform.lossyScale));
            _enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
            _player = GameObject.FindWithTag("Pllayer").GetComponent<Player.Controller>();
        }

        private void Update()
        {
            if (_spawnTrigger.Try(_currentSpawnTime)) TrySpawn();
        }

        public void SetPlayer(Player.Controller player)
        {
            _player = player;
        }

        public void Restart()
        {
            _enemies.Clear();
        }

        private void TrySpawn()
        {
            if (_enemies.Count >= _maxEnemiesAtOnce) return;
            if (TryGetSpawnLocation() is not {} pos) return;
            Enemy.Manager e = GameObject.Instantiate(_enemyPrefab, pos, Quaternion.identity).GetComponent<Enemy.Manager>();
            e.SetPlayer(_player);
            UnityAction handle = null;
            handle = () =>
            {
                _enemies.Remove(e);
                e.OnDeath.RemoveListener(handle);
            };
            e.OnDeath.AddListener(handle);
            _enemies.Add(e);
        }

        Vector3? TryGetSpawnLocation()
        {
            const int maxTries = 100;
            for (int i = 0; i < maxTries; i++)
            {
                Vector3 randomPos = MathExt.RandomPointInBounds(_spawnBounds);

                Vector3 biasedPos = Vector3.Lerp(_player.transform.position, randomPos, _biasStrength);

                if (IsValidLocation(biasedPos, _enemyRadiusWhenSpawning))
                {
                    return biasedPos;
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

