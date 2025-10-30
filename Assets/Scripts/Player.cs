using UnityEngine;

public class Player : MonoBehaviour
{
    Fighter selectedFighter;
    int chips = 100;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        GameManager.Instance.Player = this;
    }

    public Fighter GetSelectedFighter() { return selectedFighter; }

    public int GetChips() { return chips; }

    public void RemoveChips(int chipsToRemove)
    {
        chips -= chipsToRemove;
    }

    public void AddChips(int chipsToAdd)
    {
        chips += chipsToAdd;
    }

}
