using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class OddsManager : MonoBehaviour
{
    List<Fighter> fighterList;

    //Keeping these here for now while it's a stub. Should move these out.
    Fighter butterBuster = new Fighter("Butter Buster");
    Fighter leCroissant = new Fighter("Le Croissant");
    Fighter masterCupcake = new Fighter("Master Cupcake");
    Fighter doughNought = new Fighter("Dough & Nought");

    Player player;
    Fighter FighterA;
    Fighter FighterB;
    public Fighter GetFighterA { get { return FighterA; } }
    public Fighter GetFighterB { get { return FighterB; } }

    float currentOdds = 0.5f;
    float minimumOdds = 0.25f;
    float maximumOdds = 0.75f;

    int currentBet;
    int payout;
    float multiplier = 1f;

    System.Random rand = new System.Random();

    [SerializeField] GameObject currentChipsTextObj;
    [SerializeField] TMP_InputField betInputField;
    [SerializeField] Button fightButton;
    [SerializeField] Button selectFighterAButton;
    [SerializeField] Button selectFighterBButton;
    private TextMeshProUGUI currentChipsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectFighterAButton.onClick.AddListener(delegate { player.SetSelectedFigher(FighterA); });
        selectFighterBButton.onClick.AddListener(delegate { player.SetSelectedFigher(FighterB); });

        betInputField.onEndEdit.AddListener(delegate {PlayerMakesBet(Convert.ToInt32(betInputField.text)); });

        fightButton.onClick.AddListener(Fight);
        currentChipsText = currentChipsTextObj.GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        fighterList = new List<Fighter>();
        
        GameManager.Instance.OddsManager = this;
        player = GameManager.Instance.Player;
        InitFighterList();
        SelectFighters();
    }

    // Update is called once per frame
    void Update()
    {
        currentChipsText.text = GameManager.Instance.Player.GetChips().ToString();
        if(GameManager.Instance.Phase == GameManager.GamePhase.RoundStart)
        {
            selectFighterAButton.gameObject.SetActive(true);
            selectFighterBButton.gameObject.SetActive(true);
        }
        else
        {
            selectFighterAButton.gameObject.SetActive(false);
            selectFighterBButton.gameObject.SetActive(false);
        }
        if (GameManager.Instance.Phase == GameManager.GamePhase.PlaceBet)
        {
            betInputField.gameObject.SetActive(true);
        }
        else
        {
            betInputField.gameObject.SetActive(false);
        }
        if(GameManager.Instance.Phase == GameManager.GamePhase.SpeakingToNPC)
        {
            fightButton.gameObject.SetActive(true);
        }
        else
        {
            fightButton.gameObject.SetActive(false);
        }
    }
    public void OnRoundEnd()
    {
        if (player.GetSelectedFighter().IsWinner())
        {
            payout = (int)(currentBet * multiplier);
            player.AddChips(payout);
            Debug.Log("Player won " + payout + " chips!");
        }

        ResetValues();
        GameManager.Instance.MoveToNextPhase();
    }
    void ResetValues()
    {
        payout = 0;

        currentBet = 0;
        multiplier = 1f;

        FighterA = null;
        FighterB = null;
        currentOdds = 0.5f;

        betInputField.text = string.Empty;

        InitFighterList();
        SelectFighters();
    }

    /// <summary>
    /// Odds is a number that goes between 0 and 1. 0 means it is guaranteed FighterA will win, and 1 means it is guaranteed FighterB will win.
    /// In short, the *higher* the currentOdds value is, the more likely Fighter B is to win, and vice versa.
    /// Raising the odds for FighterB entails currentOdds being closer to 1. Raising the odds for FighterA entails currentOdds being closer to 0.
    /// That's how the two methods below were made.
    /// </summary>

    void RaiseFighterBOdds(float amount)
    {
        currentOdds = Mathf.Clamp(currentOdds + amount, minimumOdds, maximumOdds);
        SetMultiplier();
        Debug.Log(currentOdds);
    }

    void RaiseFighterAOdds(float amount)
    {
        currentOdds = Mathf.Clamp(currentOdds - amount, minimumOdds, maximumOdds);
        SetMultiplier();
        Debug.Log(currentOdds);
    }
    
    void SetMultiplier()
    {
        if (player.GetSelectedFighter() == null) return;

        if (player.GetSelectedFighter() == FighterA)
        {
            multiplier = 2f + currentOdds;
        }
        else
        {
            multiplier = 3f - currentOdds;
        }
    }

    void PlayerMakesBet(int playerBet)
    {
        //Probably should clamp playerBet to be less than player chips. Skips the if statement.
        if (playerBet <= player.GetChips())
        {
            currentBet = playerBet;
            player.RemoveChips(playerBet);

            Debug.Log("Current bet: " + currentBet);
            Debug.Log("Money Left: " + player.GetChips());

            //If player can make this bet, move to next phase.
            GameManager.Instance.MoveToNextPhase();
        }
        else
        {
            Debug.Log("you broke lmao");
        }
    }

    void InitFighterList()
    {
        //Clear list, so we can make sure only 4 fighters are on there.
        fighterList.Clear();

        fighterList.Add(butterBuster);
        fighterList.Add(leCroissant);
        fighterList.Add(masterCupcake);
        fighterList.Add(doughNought);

        //Reset the fighters too.
        foreach (var fighter in fighterList)
        {
            fighter.Reset();
        }
    }

    void SelectFighters()
    {
        int indexA = rand.Next(0, fighterList.Count);

        FighterA = fighterList[indexA];

        //Has to be a better way to do this. Figure it out.
        fighterList.RemoveAt(indexA);

        int indexB = rand.Next(0, fighterList.Count);

        FighterB = fighterList[indexB];

        //Sets names of Fighters on the select Fighter buttons
        TextMeshProUGUI fighterAButtonTextObj = selectFighterAButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI fighterBButtonTextObj = selectFighterBButton.GetComponentInChildren<TextMeshProUGUI>();
        fighterAButtonTextObj.text = $"Place bet on: \n {FighterA.Name}";
        fighterBButtonTextObj.text = $"Place bet on: \n {FighterB.Name}";

        Debug.Log("The fighters are: " + FighterA.Name + " and " + FighterB.Name);
    }

    void DisqualifyFighter(Fighter fighter)
    {
        //Take in the fighter from an NPC interaction
        //Set that fighter's odds to minimum (1 for fighterA, 0 for fighterB)
        //Run CheckWinner and RoundEnd
        //Basically just an instant win
    }


    void Fight()
    {
        CheckWinner();
        GameManager.Instance.MoveToNextPhase();
    }

    void CheckWinner()
    {
        float winningNumber = rand.Next(0, 100);

        if (winningNumber/100 < currentOdds)
        {
            FighterA.SetAsWinner();
        }
        else
        {
            FighterB.SetAsWinner();
        }
    }

    


}
