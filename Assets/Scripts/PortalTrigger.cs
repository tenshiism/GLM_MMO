using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (QuestManager.Instance == null || !QuestManager.Instance.HasQuest)
        {
            Debug.Log("Portal: Accept a quest from the Quest Board first!");
            return;
        }

        SceneLoader.LoadHuntingGround();
    }
}