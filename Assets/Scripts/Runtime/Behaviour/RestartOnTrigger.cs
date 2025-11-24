using UnityEngine;

namespace MJ198
{
    public class RestartOnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pllayer"))
            {
                MJ198GameManager.Restart();
            }
        }
    }
}
