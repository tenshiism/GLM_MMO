using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Inventory : NetworkBehaviour
{
    public int maxSlots = 20;

    private readonly List<ItemStack> items = new List<ItemStack>();

    public System.Action<ItemStack> OnItemAdded;
    public System.Action<ItemStack> OnItemRemoved;
    public System.Action OnInventoryChanged;

    public bool AddItem(string itemId, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemId)) return false;

        var def = ItemRegistry.Get(itemId);
        if (def == null)
        {
            Debug.LogWarning($"Item not found in registry: {itemId}");
            return false;
        }

        if (def.maxStackSize > 1)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var stack = items[i];
                if (stack.itemId == itemId && stack.quantity < def.maxStackSize)
                {
                    int space = def.maxStackSize - stack.quantity;
                    int add = Mathf.Min(quantity, space);
                    stack.quantity += add;
                    items[i] = stack;
                    quantity -= add;
                    OnItemAdded?.Invoke(stack);
                    OnInventoryChanged?.Invoke();
                    if (quantity <= 0) return true;
                }
            }
        }

        while (quantity > 0)
        {
            if (items.Count >= maxSlots)
            {
                Debug.Log("Inventory full!");
                return false;
            }

            int add = Mathf.Min(quantity, def.maxStackSize);
            var newStack = new ItemStack(itemId, add);
            items.Add(newStack);
            OnItemAdded?.Invoke(newStack);
            OnInventoryChanged?.Invoke();
            quantity -= add;
        }

        return true;
    }

    public bool RemoveItem(string itemId, int quantity = 1)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemId == itemId)
            {
                var stack = items[i];
                if (stack.quantity <= quantity)
                {
                    items.RemoveAt(i);
                    OnItemRemoved?.Invoke(stack);
                    OnInventoryChanged?.Invoke();
                    return true;
                }
                else
                {
                    stack.quantity -= quantity;
                    items[i] = stack;
                    OnItemRemoved?.Invoke(new ItemStack(itemId, quantity));
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }
        return false;
    }

    public int GetItemCount(string itemId)
    {
        int count = 0;
        foreach (var stack in items)
        {
            if (stack.itemId == itemId)
                count += stack.quantity;
        }
        return count;
    }

    public bool HasItem(string itemId, int quantity = 1)
    {
        return GetItemCount(itemId) >= quantity;
    }

    public List<ItemStack> GetAllItems()
    {
        return new List<ItemStack>(items);
    }

    public void Clear()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) enabled = false;
    }
}
