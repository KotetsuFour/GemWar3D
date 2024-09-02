public class SupportData
{
    public int id;
    public int supportAmount;
    public SupportLevel level;
    public int requiredForC;
    public int requiredForB;
    public int requiredForA;
    public string[] scriptC;
    public string[] scriptB;
    public string[] scriptA;
    public SupportData(int id, int requiredForC, int requiredForB, int requiredForA,
        string[] scriptC, string[] scriptB, string[] scriptA)
    {
        this.id = id;
        this.requiredForC = requiredForC;
        this.requiredForB = requiredForB;
        this.requiredForA = requiredForA;
        this.scriptC = scriptC;
        this.scriptB = scriptB;
        this.scriptA = scriptA;
    }

    public string[] getAvailableScript()
    {
        if (level == SupportLevel.NONE && supportAmount >= requiredForC)
        {
            return scriptC;
        }
        else if (level == SupportLevel.C && supportAmount >= requiredForB)
        {
            return scriptB;
        }
        else if (level == SupportLevel.B && supportAmount >= requiredForA)
        {
            return scriptA;
        }
        return null;
    }

    public enum SupportLevel
    {
        NONE, C, B, A, FUSE
    }
}
