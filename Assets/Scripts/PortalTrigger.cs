using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.LoadHuntingGround();
        }
    }
}