using PlazmaGames.Attribute;
using System.Threading;
using UnityEngine;

namespace MJ198.Enemy
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private EnemySettings _settings;
        [SerializeField] private HealthTaker _healthTaker;

        [SerializeField] private Player.Controller _player;

        private TimedTrigger _lastShot = new();

        private void OnDeath()
        {
            _player.GetComponent<Player.Manager>().AddScore(_settings.Score);
            Destroy(gameObject);
        }

        private void Awake()
        {
            if (!_healthTaker) _healthTaker = GetComponent<HealthTaker>();
            _healthTaker.OnDeath.AddListener(OnDeath);
        }

        private void Update()
        {
            if (GetDistanceToPlayer() < _settings.ShootingDst) TryShoot();
        }

        private float GetDistanceToPlayer()
        {
            return Vector3.Distance(transform.position, _player.transform.position);
        }

        private Quaternion GetBulletDirection()
        {
            return Quaternion.LookRotation(Vector3.Normalize((_player.transform.position + _player.GetVelocity() * GetDistanceToPlayer() / _settings.BulletVelocity) - transform.position));
        }

        private void TryShoot()
        {
            if (!_lastShot.Try(_settings.FireRate)) return;
            Quaternion dir = GetBulletDirection();
            Vector3 pos = dir * Vector3.forward;
            Bullet b = GameObject.Instantiate(_settings.BulletPrefab, transform.position + _settings.BulletSpawnDst * pos, dir).GetComponent<Bullet>();
            b.Damage = _settings.Damage;
            b.Velocity = _settings.BulletVelocity;
        }
    }
}
