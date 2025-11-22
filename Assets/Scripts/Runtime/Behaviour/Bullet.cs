using UnityEngine;

namespace MJ198
{
    public class Bullet : MonoBehaviour
    {
        public float Velocity = 20;
        private void Update()
        {
            transform.Translate(0, 0, Velocity);
        }
    }
}
