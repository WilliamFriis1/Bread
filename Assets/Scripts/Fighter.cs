using UnityEngine;

public class Fighter
{
    Texture2D texture;
    string fighterName;
    bool hasWon;

    public Fighter(string name)
    {
        fighterName = name;
    }

    public void SetAsWinner()
    {
        hasWon = true;
    }

    public bool IsWinner()
    {
        return hasWon;
    }

    public void Reset()
    {
        hasWon = false;
    }

}
