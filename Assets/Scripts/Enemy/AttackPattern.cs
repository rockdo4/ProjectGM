[System.Serializable]
public class AttackPattern
{
    public string patternName;
    public bool[] pattern;

    public AttackPattern(string name, bool[] data)
    {
        patternName = name;
        pattern = data;
    }
}
