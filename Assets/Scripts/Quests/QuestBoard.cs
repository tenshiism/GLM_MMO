using UnityEngine;
using System.Collections.Generic;

public class QuestBoard : Interactable
{
    [Header("Available Quests")]
    public List<QuestDefinition> availableQuests = new List<QuestDefinition>();

    private QuestDefinition selectedQuest;
    private Vector2 scrollPos;
    private bool showUI;

    private void Start()
    {
        interactPrompt = "Quest Board (E)";
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

        float w = 500;
        float h = 400;
        float x = (Screen.width - w) / 2;
        float y = (Screen.height - h) / 2;

        GUI.Box(new Rect(x, y, w, h), "Quest Board");

        float contentY = y + 30;
        float contentH = h - 60;
        float innerW = w - 20;

        GUI.BeginGroup(new Rect(x + 10, contentY, innerW, contentH));

        if (QuestManager.Instance != null && QuestManager.Instance.HasQuest)
        {
            var active = QuestManager.Instance.ActiveQuest;
            GUI.Label(new Rect(0, 0, innerW, 30), $"Active Quest: {active.questName}");
            int kills = QuestManager.Instance.killCount;
            if (active.killTarget > 0)
                GUI.Label(new Rect(0, 25, innerW, 30), $"Kills: {kills}/{active.killTarget}");
            if (GUI.Button(new Rect(0, contentH - 30, 100, 25), "Close"))
                CloseUI();
            GUI.EndGroup();
            return;
        }

        scrollPos = GUI.BeginScrollView(new Rect(0, 0, innerW, contentH - 40), scrollPos, new Rect(0, 0, innerW - 20, availableQuests.Count * 100));

        for (int i = 0; i < availableQuests.Count; i++)
        {
            var q = availableQuests[i];
            if (q == null) continue;

            float qy = i * 100;
            GUI.Box(new Rect(0, qy, innerW - 20, 95), "");

            GUI.Label(new Rect(10, qy + 5, innerW - 40, 25), $"<b>{q.questName}</b>");
            GUI.Label(new Rect(10, qy + 30, innerW - 40, 20), q.questType.ToString());
            GUI.Label(new Rect(10, qy + 50, innerW - 40, 20), $"XP: {q.xpReward}  Credits: {q.creditReward}");
            if (q.killTarget > 0)
                GUI.Label(new Rect(10, qy + 70, 150, 20), $"Kill {q.killTarget} enemies");

            if (GUI.Button(new Rect(innerW - 120, qy + 10, 90, 30), "Accept"))
            {
                AcceptQuest(q);
                break;
            }
        }

        GUI.EndScrollView();

        if (GUI.Button(new Rect(0, contentH - 30, 100, 25), "Close"))
            CloseUI();

        GUI.EndGroup();
    }

    private void AcceptQuest(QuestDefinition quest)
    {
        if (QuestManager.Instance == null)
        {
            var go = new GameObject("QuestManager");
            go.AddComponent<QuestManager>();
        }
        QuestManager.Instance.AcceptQuest(quest);
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
            UIBlocker.Close();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
