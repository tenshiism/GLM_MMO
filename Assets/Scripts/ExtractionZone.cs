using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    public float extractionDelay = 3f;
    public GameObject extractionEffect;

    private float timer;
    private bool extracting;
    private GameObject extractingPlayer;

    private void Update()
    {
        if (!extracting) return;
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (QuestManager.Instance != null && QuestManager.Instance.HasQuest)
                QuestManager.Instance.CompleteQuest();

            Debug.Log("Extraction complete! Returning to Hub.");
            SceneLoader.LoadHub();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || extracting) return;
        extracting = true;
        extractingPlayer = other.gameObject;
        timer = extractionDelay;
        Debug.Log($"Extracting in {extractionDelay} seconds...");

        if (extractionEffect != null)
            Instantiate(extractionEffect, transform.position, Quaternion.identity);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        extracting = false;
        extractingPlayer = null;
        Debug.Log("Extraction cancelled.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>() != null ? GetComponent<BoxCollider>().size : Vector3.one * 10);
    }
}
