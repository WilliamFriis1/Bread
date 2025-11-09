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
        Debug.Log(GameManager.Instance.Player);
    }

    public Fighter GetSelectedFighter() { return selectedFighter; }
    public string GetSelectedFighterName() {  return selectedFighter.Name; }
    public void SetSelectedFigher(Fighter fighter) 
    { 
        selectedFighter = fighter; 
        Debug.Log("Selected fighter is: " + selectedFighter.Name);
        GameManager.Instance.MoveToNextPhase();
    }

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
