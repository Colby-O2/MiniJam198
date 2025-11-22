using PlazmaGames.Attribute;
using UnityEngine;
using UnityEngine.Events;

namespace MJ198
{
    public class HealthTaker : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _maxHealth = 100f;

        [Header("Current Stats")]
        [SerializeField, ReadOnly] private float _currentHealth;

        public UnityEvent OnDeath = new UnityEvent();

        public float GetHealthPercent() => _currentHealth / _maxHealth;
        public float GetMaxHealth() => _maxHealth;
        public float GetCurrentHealth() => _currentHealth;

        private void Awake()
        {
            ResetHealth();
        }

        public void TakeHealth(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDeath?.Invoke();
            }
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
        }
    }
}
