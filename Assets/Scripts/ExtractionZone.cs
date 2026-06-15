using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (QuestManager.Instance != null && QuestManager.Instance.HasQuest)
            QuestManager.Instance.CompleteQuest();

        SceneLoader.LoadHub();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>() != null ? GetComponent<BoxCollider>().size : Vector3.one * 10);
    }
}
