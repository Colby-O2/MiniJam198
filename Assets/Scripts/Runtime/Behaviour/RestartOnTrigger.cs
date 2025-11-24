using UnityEngine;

namespace MJ198
{
    public class RestartOnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pllayer"))
            {
                if (MJ198GameManager.Player) MJ198GameManager.Player.OnDeath();
            }
        }
    }
}
