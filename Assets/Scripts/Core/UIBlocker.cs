public static class UIBlocker
{
    public static bool IsOpen { get; private set; }
    private static int openCount;

    public static void Open()
    {
        openCount++;
        IsOpen = true;
    }

    public static void Close()
    {
        openCount--;
        if (openCount <= 0)
        {
            openCount = 0;
            IsOpen = false;
        }
    }

    public static void ForceClose()
    {
        openCount = 0;
        IsOpen = false;
    }
}
