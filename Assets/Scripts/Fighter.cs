using UnityEngine;

public class Fighter
{
    Texture2D texture;
    public string name;
    bool hasWon;

    public Fighter(string name)
    {
        this.name = name;
    }

    public void SetAsWinner()
    {
        hasWon = true;
        Debug.Log("And the winner is: " + name);
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
