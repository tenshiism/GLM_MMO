using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactPrompt = "Interact";

    public bool playerInRange;
    public GameObject playerObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerObject = other.gameObject;
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerObject = null;
            OnPlayerExit();
        }
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
    public abstract void OnInteract(GameObject player);
}
