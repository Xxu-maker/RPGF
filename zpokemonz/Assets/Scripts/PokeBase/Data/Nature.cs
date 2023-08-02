using UnityEngine;
[System.Serializable]
public class Nature
{
    [SerializeField] string natureName;
    public string Name => natureName;

    private int varyUp;
    private int varyDown;
    public int Up => varyUp;
    public int Down => varyDown;

    public Nature(string name, int upIndex, int downIndex)
    {
        natureName = name;
        varyUp = upIndex;
        varyDown = downIndex;
    }
}