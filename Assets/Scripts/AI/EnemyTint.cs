using UnityEngine;

public class EnemyTint : MonoBehaviour
{
    [Header("Level Thresholds (relative to player)")]
    public float baseLevel = 1f;
    public float blueThreshold = 5f;
    public float yellowThreshold = 11f;
    public float redThreshold = 20f;

    [Header("Tint Colors")]
    public Color baseColor = Color.white;
    public Color blueTint = new Color(0.4f, 0.6f, 1f);
    public Color yellowTint = new Color(1f, 0.9f, 0.2f);
    public Color redTint = new Color(1f, 0.3f, 0.2f);

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private int lastPlayerLevel = -1;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var cl = player.GetComponent<CharacterLevel>();
        if (cl == null) return;

        int playerLevel = cl.Level.Value;
        if (playerLevel != lastPlayerLevel)
        {
            lastPlayerLevel = playerLevel;
            ApplyTint(playerLevel);
        }
    }

    public void ApplyTint(int playerLevel)
    {
        float diff = baseLevel - playerLevel;
        Color tint = GetTintColor(diff);

        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", tint);
            propBlock.SetColor("_BaseColor", tint);
            r.SetPropertyBlock(propBlock);
        }
    }

    public Color GetTintColor(float levelDiff)
    {
        float absDiff = Mathf.Abs(levelDiff);

        if (absDiff >= redThreshold)
            return redTint;
        if (absDiff >= yellowThreshold)
            return yellowTint;
        if (absDiff >= blueThreshold)
            return blueTint;
        return baseColor;
    }

    public string GetTintName(float levelDiff)
    {
        float absDiff = Mathf.Abs(levelDiff);
        if (absDiff >= redThreshold) return "Red (Elite)";
        if (absDiff >= yellowThreshold) return "Yellow (Hard)";
        if (absDiff >= blueThreshold) return "Blue (Veteran)";
        return "Base";
    }
}
