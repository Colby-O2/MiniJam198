using UnityEngine;

namespace MJ198.Enemy
{
    [CreateAssetMenu(fileName = "DefaultEnemySettings", menuName = "Enemy/Settings")]
    public class EnemySettings : ScriptableObject
    {
        [Header("Shooting")]
        public GameObject BulletPrefab;
        public float BulletSpawnDst = 1f;
        public float BulletVelocity;
        public float ShootingDst;
        public float FireRate;
        public float Damage;
        public int Score;
    }
}
