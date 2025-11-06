using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{

    public enum GamePhase
    {
        RoundStart,
        PlaceBet,
        SpeakingToNPC,
        RoundEnd
    }

    private static GameManager instance;

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

            if(instance == null)
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

    public Player Player { get; set; }
    public OddsManager OddsManager { get; set; }
    public GamePhase Phase;

    [SerializeField] Button nextGamePhase;

    void Start()
    {
        nextGamePhase.onClick.AddListener(MoveToNextPhase);
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
    }

    public void MoveToNextPhase()
    {
        Phase++;
        if((int)(Phase)> 3)
        {
            Phase = 0;
        }
        Debug.Log(Phase.ToString());
    }


    void Update()
    {
        
    }
}
