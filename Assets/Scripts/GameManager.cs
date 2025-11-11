using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button nextGamePhase;
    [SerializeField] private CanvasGroup m_gameScene;
    [SerializeField] private CanvasGroup m_winScene;
    [SerializeField] private CanvasGroup m_loseScene;
    //[SerializeField] private CanvasGroup m_pauseScene;
    public enum GameDay
    {
        Day0, Day1, Day2, Day3
    }
    public enum GamePhase
    {
        RoundStart, PlaceBet, SpeakingToNPC, RoundEnd
    }
    public Player Player { get; set; }
    public OddsManager OddsManager { get; set; }
    public GamePhase Phase;
    public GameDay Day;
    [Header("Refs")]
    public DayDirector dayDirector;                 // builds the day's NPC queue
    public EncounterDirector encounterDirector;

    private static GameManager instance;
    private FadeAnimator m_fadeAnimator;
    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            //If in editor:
            if (!Application.isPlaying)
            {
                //When application is not playing, return nothing;
                return null;
            }

            if (instance == null)
            {
                //If the application is playing, but there isn't a game manager, make one.
                //doesnt work
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            //Return the created instance.
            return instance;
        }
    }

    void Start()
    {
        if (nextGamePhase)
        {
            nextGamePhase.onClick.AddListener(MoveToNextPhase);
        }
        m_winScene.blocksRaycasts = false;
        m_loseScene.blocksRaycasts = false;

        Day = GameDay.Day0;
        if (dayDirector)
        {
            dayDirector.SetupDay(Day);
        }
        Phase = GamePhase.RoundStart;
    }

    private void Awake()
    {
        if (instance == null)
        {
            //On awake, when not in the editor, make the instance equals to this.
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        m_fadeAnimator = GetComponent<FadeAnimator>();
        m_winScene.alpha = 0.0f;
        m_loseScene.alpha = 0.0f;
        m_gameScene.alpha = 1.0f;
    }

    public void MoveToNextPhase()
    {
        Phase++;
        if ((int)Phase > 3)
        {
            Phase = 0;
            MoveToNextDay();
            return;
        }

        Debug.Log("[GM] Phase → " + Phase);

        if (Phase == GamePhase.SpeakingToNPC && encounterDirector != null)
        {
            // reset per-speaking counters (if you have them) and start chain
            encounterDirector.BeginSpeakingPhase();
            encounterDirector.StartNpcEncounter();
        }

        if (Phase == GamePhase.RoundEnd)
        {
            // Do NOT auto-fight here anymore. Just land in RoundEnd and wait for the Fight button.
            // Curtains should already be closed after the last NPC via CharacterSlot -> EncounterDirector.
        }
    }
    // New: called by the Fight button (via OddsManager) AFTER simulating the fight
    public void ResolveRoundAfterFight()
    {
        // Payout + reset round values
        if (OddsManager != null)
            OddsManager.OnRoundEnd();

        // Go to next day (your design says a fight ends the day)
        MoveToNextDay();

        // Start the new day at RoundStart (don't auto-start NPCs)
        Phase = GamePhase.RoundStart;
        Debug.Log("[GM] Round resolved → next day, back to RoundStart");
    }

    public void MoveToNextDay()
    {
        Day++;
        CheckWinCondition();

        //if ((int)(Day) == 4)
        //{
        //    CheckWinCondition();
        //}
        if (dayDirector)
        {
            dayDirector.SetupDay(Day);
        }
        // Phase = GamePhase.RoundStart;
    }

    public void CheckWinCondition()
    {
        if (OddsManager.CheckIfPlayerWon())
        {
            Win();
        }
        if (Player.GetChips() <= 0)
        {
            Lose();
        }

    }

    void Win()
    {
        StartCoroutine(FadeToWin());
        Debug.Log("Win");
    }

    void Lose()
    {
        StartCoroutine(FadeToLose());
        Debug.Log("Lose");
    }
    IEnumerator FadeToWin()
    {
        // m_gameScene.blocksRaycasts = false;
        // m_loseScene.blocksRaycasts = false;
        // m_fadeAnimator.FadeOut(m_gameScene, 2f);
        // yield return new WaitForSeconds(0.5f);
        // m_fadeAnimator.FadeIn(m_winScene, 1f);
        // m_winScene.blocksRaycasts = true;
        if (m_gameScene && m_fadeAnimator) { m_gameScene.blocksRaycasts = false; m_fadeAnimator.FadeOut(m_gameScene, 2f); }
        yield return new WaitForSeconds(0.5f);
        if (m_winScene && m_fadeAnimator) { m_fadeAnimator.FadeIn(m_winScene, 1f); m_winScene.blocksRaycasts = true; }
    }
    IEnumerator FadeToLose()
    {
        m_gameScene.blocksRaycasts = false;
        m_winScene.blocksRaycasts = false;
        m_fadeAnimator.FadeOut(m_gameScene, 2f);
        yield return new WaitForSeconds(0.5f);
        m_fadeAnimator.FadeIn(m_loseScene, 1f);
        m_loseScene.blocksRaycasts = true;
    }
}
