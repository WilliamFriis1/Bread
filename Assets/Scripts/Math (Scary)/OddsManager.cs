using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    float currentOdds = 0.5f;
    float minimumOdds = 0.25f;
    float maximumOdds = 0.75f;

    int currentBet;
    int payout;
    float multiplier = 1f;

    [SerializeField] InputField betInputField;
    [SerializeField] Button increaseOddsButton;
    [SerializeField] Button decreaseOddsButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        fighterList = new List<Fighter>();
        
        GameManager.Instance.OddsManager = this;
        player = GameManager.Instance.Player;
        InitFighterList();
    }

    // Update is called once per frame
    void Update()
    {
        //Make sure currentOdds stays between min and max
        Mathf.Clamp(currentOdds, minimumOdds, maximumOdds);

        //Update multiplier constantly. Could move this to be event based. Useful for making the UI.
        SetMultiplier();
    }

/// <summary>
/// Odds is a number that goes between 0 and 1. 0 means it is guaranteed FighterA will win, and 1 means it is guaranteed FighterB will win.
/// In short, the *higher* the currentOdds value is, the more likely Fighter B is to win, and vice versa.
/// Raising the odds for FighterB entails currentOdds being closer to 1. Raising the odds for FighterA entails currentOdds being closer to 0.
/// That's how the two methods below were made.
/// </summary>

    void RaiseFighterBOdds(float amount)
    {
        currentOdds += amount;
    }

    void RaiseFighterAOdds(float amount)
    {
        currentOdds -= amount;
    }

    void InitFighterList()
    {
        //Clear list, so we can make sure only 4 fighters are on there.
        //list is null for some reason no clue why
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

    void PlayerMakesBet(int playerBet)
    {
        //Probably should clamp playerBet to be less than player chips. Skips the if statement.
        if(playerBet <= player.GetChips())
        {
            currentBet = playerBet;
            player.RemoveChips(playerBet);
        }
        else
        {
            Debug.Log("you broke lmao");
        }
    }

    void SelectFighters()
    {
        System.Random rand = new System.Random();

        int indexA = rand.Next(0, 3);

        FighterA = fighterList[indexA];

        //Has to be a better way to do this. Figure it out.
        fighterList.RemoveAt(indexA);

        int indexB = rand.Next(0, 2);

        FighterB = fighterList[indexB];
    }

    void Fight()
    {
        CheckWinner();
        
        OnRoundEnd();
    }

    void SetMultiplier()
    {
        if (player.GetSelectedFighter() == null) return;

        if (player.GetSelectedFighter() == FighterA)
        {
            multiplier = 1f + currentOdds;
        }
        else
        {
            multiplier = 2f - currentOdds;
        }
    }

    void CheckWinner()
    {
        float winningNumber = UnityEngine.Random.value;

        if (winningNumber < currentOdds)
            FighterA.SetAsWinner();
        else
            FighterB.SetAsWinner();
    }

    void OnRoundEnd()
    {
        if (player.GetSelectedFighter().IsWinner())
        {
            payout = (int)(currentBet * multiplier);
            player.AddChips(payout);
        }

        ResetValues();
    }

    void ResetValues()
    {
        payout = 0;

        currentBet = 0;
        multiplier = 1f;

        FighterA = null;
        FighterB = null;
        currentOdds = 0.5f;

        InitFighterList();
        
    }


}
