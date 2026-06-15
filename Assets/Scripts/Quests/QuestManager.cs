using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public QuestDefinition ActiveQuest { get; private set; }

    [Header("Tracking")]
    public int killCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AcceptQuest(QuestDefinition quest)
    {
        ActiveQuest = quest;
        killCount = 0;
        Debug.Log($"Quest accepted: {quest.questName}");
    }

    public void AddKill(string enemyType)
    {
        if (ActiveQuest == null) return;
        killCount++;
        Debug.Log($"Kill {killCount}/{ActiveQuest.killTarget}");

        if (ActiveQuest.killTarget > 0 && killCount >= ActiveQuest.killTarget)
            CompleteQuest();
    }

    public bool HasQuest => ActiveQuest != null;

    public void CompleteQuest()
    {
        if (ActiveQuest == null) return;
        Debug.Log($"Quest complete: {ActiveQuest.questName}");

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var cl = player.GetComponent<CharacterLevel>();
            if (cl != null) cl.AddXP(ActiveQuest.xpReward);
        }

        ActiveQuest = null;
        killCount = 0;
    }

    public void ClearQuest()
    {
        ActiveQuest = null;
        killCount = 0;
    }
}
