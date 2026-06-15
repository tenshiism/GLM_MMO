using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct ItemStack : INetworkSerializable
{
    public string itemId;
    public int quantity;

    public ItemStack(string id, int qty = 1)
    {
        itemId = id;
        quantity = qty;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemId);
        serializer.SerializeValue(ref quantity);
    }
}
