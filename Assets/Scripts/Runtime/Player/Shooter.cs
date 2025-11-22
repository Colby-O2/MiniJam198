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
            Bullet b = GameObject.Instantiate(_bulletPrefab, _shootLocation.position, _head.rotation).GetComponent<Bullet>();
            b.Velocity = _settings.BulletVelocity;
        }
    }
}
