using UnityEngine;
using System.Collections.Generic;

public class ShopVendor : Interactable
{
    [Header("Shop")]
    public string shopName = "Armory";
    public List<ItemDefinition> stock = new List<ItemDefinition>();

    private bool showUI;
    private Vector2 scrollPos;
    private string buyAmountStr = "1";
    private string message = "";

    private void Start()
    {
        interactPrompt = $"{shopName} (E)";
    }

    public override void OnInteract(GameObject player)
    {
        showUI = !showUI;
        if (showUI)
        {
            UIBlocker.Open();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            UIBlocker.Close();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnGUI()
    {
        if (!showUI) return;

        float w = 600;
        float h = 500;
        float x = (Screen.width - w) / 2;
        float y = (Screen.height - h) / 2;

        GUI.Box(new Rect(x, y, w, h), shopName);

        float contentY = y + 30;
        float contentH = h - 80;
        float innerW = w - 20;

        GUI.BeginGroup(new Rect(x + 10, contentY, innerW, contentH));

        GUI.Label(new Rect(0, 0, 200, 25), "Buy:");
        buyAmountStr = GUI.TextField(new Rect(40, 0, 50, 22), buyAmountStr);
        int.TryParse(buyAmountStr, out int buyAmount);
        if (buyAmount < 1) buyAmount = 1;

        scrollPos = GUI.BeginScrollView(new Rect(0, 28, innerW, contentH - 60), scrollPos, new Rect(0, 0, innerW - 20, stock.Count * 70));

        var inventory = playerObject != null ? playerObject.GetComponent<Inventory>() : null;

        for (int i = 0; i < stock.Count; i++)
        {
            var item = stock[i];
            if (item == null) continue;

            float iy = i * 70;
            GUI.Box(new Rect(0, iy, innerW - 20, 65), "");

            GUI.Label(new Rect(10, iy + 5, 200, 22), item.itemName);
            GUI.Label(new Rect(10, iy + 27, 150, 20), $"{item.rarity} | ${item.buyPrice}");

            if (GUI.Button(new Rect(innerW - 120, iy + 8, 90, 25), $"Buy x{buyAmount}"))
            {
                if (inventory != null)
                {
                    inventory.AddItem(item.name, buyAmount);
                    message = $"Bought {buyAmount}x {item.itemName}";
                }
            }
        }

        GUI.EndScrollView();

        if (!string.IsNullOrEmpty(message))
        {
            GUI.Label(new Rect(0, contentH - 30, 300, 25), message);
        }

        GUI.EndGroup();

        if (GUI.Button(new Rect(x + w - 110, y + h - 35, 100, 25), "Close"))
            CloseUI();
    }

    private void CloseUI()
    {
        showUI = false;
        UIBlocker.Close();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (showUI)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
