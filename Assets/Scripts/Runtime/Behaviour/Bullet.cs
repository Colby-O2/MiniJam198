using MJ198.Player;
using UnityEngine;

namespace MJ198
{
    public class Bullet : MonoBehaviour
    {
        public float Velocity = 20;
        public float Damage = 10f;
        public float LifeSpan = 1f;
        public string IgnoreTag;

        [SerializeField] private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer > LifeSpan) Destroy(gameObject);

            transform.Translate(0, 0, Velocity * Time.deltaTime);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(IgnoreTag) && other.TryGetComponent<HealthTaker>(out HealthTaker health))
            {
                health.TakeHealth(Damage);
            }

            Debug.Log($"Tag {other.tag} Name {other.name}");

            if (!other.CompareTag("Bullet")) Destroy(gameObject);
        }
    }
}
