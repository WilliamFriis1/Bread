using UnityEngine;

public class Fighter : MonoBehaviour
{
    Texture2D texture;
    string fighterName;
    bool hasWon;

    public Fighter(string name)
    {
        fighterName = name;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
