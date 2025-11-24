using UnityEngine;
using MJ198.MonoSystems;
using PlazmaGames.Core;
using UnityEditor.Rendering;

namespace MJ198.Player
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private MovementSettings _settings;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _shootLocation;
        
        private IInputMonoSystem _input;

        private TimedTrigger _lastShot = new();
        
        private void OnEnable()
        {
            _input.ShootAction.AddListener(TryShoot);
        }

        private void OnDisable()
        {
            _input.ShootAction.RemoveListener(TryShoot);
        }

        private void Awake()
        {
            _input = GameManager.GetMonoSystem<IInputMonoSystem>();
            
        }

        private void TryShoot()
        {
            if (!_lastShot.Try(_settings.ShootTime)) return;

            Ray ray = new Ray(_head.position, _head.forward);

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f)) 
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * 100f; 
            }

            Vector3 bulletDirection = (targetPoint - _shootLocation.position).normalized;

            Bullet b = Instantiate(_bulletPrefab, _shootLocation.position, Quaternion.LookRotation(bulletDirection))
                             .GetComponent<Bullet>();

            b.Velocity = _settings.BulletVelocity;
            b.LifeSpan = _settings.BulletLifeSpan;
            b.Damage = _settings.BulletDamage;
            b.IgnoreTag = _settings.IgnoreTag;
        }
    }
}
