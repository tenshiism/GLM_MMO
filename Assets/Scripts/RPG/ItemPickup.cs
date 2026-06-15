using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    public ItemStack itemStack;
    public float pickupRadius = 2f;
    public float lifetime = 60f;

    private float hoverTimer;
    private Transform lookTarget;

    private void Start()
    {
        hoverTimer = Random.value * Mathf.PI * 2f;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        hoverTimer += Time.deltaTime * 2f;
        Vector3 pos = transform.position;
        pos.y += Mathf.Sin(hoverTimer) * 0.05f;
        transform.position = pos;
        transform.Rotate(Vector3.up, 30f * Time.deltaTime);
    }

    public void SetItem(ItemStack stack)
    {
        itemStack = stack;
        var def = ItemRegistry.Get(stack.itemId);
        if (def != null)
            name = $"Pickup_{def.itemName}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var inventory = other.GetComponent<Inventory>();
        if (inventory != null && inventory.AddItem(itemStack.itemId, itemStack.quantity))
        {
            Destroy(gameObject);
        }
    }
}
