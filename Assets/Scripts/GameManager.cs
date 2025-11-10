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
    [SerializeField] private CanvasGroup m_pauseScene;
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
        nextGamePhase.onClick.AddListener(MoveToNextPhase);
        m_winScene.blocksRaycasts = false;
        m_loseScene.blocksRaycasts = false;
        Phase = GamePhase.SpeakingToNPC;
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
        if ((int)(Phase) > 3)
        {
            Phase = 0;
            MoveToNextDay();
        }
        Debug.Log(Phase.ToString());
    }

    public void MoveToNextDay()
    {
        Day++;
        CheckWinCondition();

        //if ((int)(Day) == 4)
        //{
        //    CheckWinCondition();
        //}
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
        m_gameScene.blocksRaycasts = false;
        m_loseScene.blocksRaycasts = false;
        m_fadeAnimator.FadeOut(m_gameScene, 2f);
        yield return new WaitForSeconds(0.5f);
        m_fadeAnimator.FadeIn(m_winScene, 1f);
        m_winScene.blocksRaycasts = true;
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
