using UnityEngine;

namespace MJ198.Enemy
{
    [CreateAssetMenu(fileName = "DefaultEnemySettings", menuName = "Enemy/Settings")]
    public class EnemySettings : ScriptableObject
    {
        [Header("Shooting")]
        public GameObject BulletPrefab;
        public float LifeSpan = 1f;
        public float BulletVelocity;
        public string IgnoreTag = "Enemy";
        public float ShootingDst;
        public float FireRate;
        public float Damage;
        public int Score;
    }
}
