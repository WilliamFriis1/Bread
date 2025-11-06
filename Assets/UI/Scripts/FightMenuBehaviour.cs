using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightMenuBehaviour : MonoBehaviour
{
    [SerializeField] private Button m_startButton;
    [SerializeField] private GameObject m_overlay;
    [SerializeField] private GameObject m_oddsManagerObj;
    [SerializeField] private GameObject m_parentObj;
    [SerializeField] private Image m_fighterA;
    [SerializeField] private Image m_fighterB;
    [SerializeField] private Sprite m_butterBusterSprite;
    [SerializeField] private Sprite m_leCroissantSprite;
    [SerializeField] private Sprite m_masterCupcakeSprite;
    [SerializeField] private Sprite m_doughAndNoughtSprite;

    public delegate void FightStartHandler(float fightDuration, string fighterA, string fighterB);
    public event FightStartHandler OnFightStarted;

    private OddsManager m_oddsManager;
    private FadeAnimator m_fadeAnimator;
    private Vector3 m_fighterAStartPosition;
    private Vector3 m_fighterBStartPosition;
    private Vector3 m_parentTargetPosition;
    private CanvasGroup m_overlayGroup;

    #region Unity Methods
    void Start()
    {
        m_startButton.onClick.AddListener(delegate { Init(); });
        m_fadeAnimator = GetComponent<FadeAnimator>();
        m_oddsManager = m_oddsManagerObj.GetComponent<OddsManager>();
        m_fighterAStartPosition = m_fighterA.GetComponent<RectTransform>().localPosition;
        m_fighterBStartPosition = m_fighterB.GetComponent<RectTransform>().localPosition;
        m_parentTargetPosition = m_parentObj.transform.localPosition;
        m_overlayGroup = m_overlay.GetComponent<CanvasGroup>();
        SetFighterSprites();
        Vector3 newPos = m_parentObj.transform.localPosition;
        newPos.y += 1100;
        m_parentObj.transform.localPosition = newPos;
        m_overlayGroup.alpha = 0.0f;
    }
    #endregion

    public void Init()
    {
        m_fadeAnimator.FadeIn(m_overlayGroup, 1f);
        StartCoroutine(InitFightScene(1f));
        StartCoroutine(StartFight(2f, 5f));
    }

    private void SetFighterSprites()
    {
        if (!string.IsNullOrEmpty(m_oddsManager.GetFighterAName) && !string.IsNullOrEmpty(m_oddsManager.GetFighterBName))
        {
            string fighterAName = m_oddsManager.GetFighterAName;
            string fighterBName = m_oddsManager.GetFighterBName;

            m_fighterA.sprite = GetFighterSprite(fighterAName);

            m_fighterB.sprite = GetFighterSprite(fighterBName);
        }
        m_fighterA.SetNativeSize();
        m_fighterB.SetNativeSize();

        float scaleFactor = 0.3f;

        RectTransform fighterARect = m_fighterA.GetComponent<RectTransform>();
        RectTransform fighterBRect = m_fighterB.GetComponent<RectTransform>();

        fighterARect.transform.localScale *= scaleFactor;
        fighterBRect.transform.localScale *= scaleFactor;
    }

    private Sprite GetFighterSprite(string fighterName)
    {
        switch (fighterName)
        {
            case "Butter Buster":
                return m_butterBusterSprite;
            case "Le Croissant":
                return m_leCroissantSprite;
            case "Master Cupcake":
                return m_masterCupcakeSprite;
            case "Dough & Nought":
                return m_doughAndNoughtSprite;
            default:
                Debug.LogWarning($"Fighter {fighterName} has no assigned sprite!");
                return null;
        }
    }

    IEnumerator InitFightScene(float duration)
    {
        yield return new WaitForSeconds(2f);
        float elapsedTime = 0f;
        Vector3 startPos = m_parentObj.transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            LerpToTargetPosition(m_parentObj, startPos, m_parentTargetPosition, t);
            yield return null;
        }
        SetFinalPosition(m_parentObj, m_parentTargetPosition);
    }
     
    IEnumerator StartFight(float startUpDuration, float fightDuration)
    {
        yield return new WaitForSeconds(4f);
        float elapsedTime = 0f;

        Vector3 fighterATargetPosition = m_fighterAStartPosition + new Vector3(250, 0, 0);
        Vector3 fighterBTargetPosition = m_fighterBStartPosition + new Vector3(-250, 0, 0);

        while (elapsedTime < startUpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / startUpDuration);
            LerpToTargetPosition(m_fighterA.gameObject, m_fighterAStartPosition, fighterATargetPosition, t);
            LerpToTargetPosition(m_fighterB.gameObject, m_fighterBStartPosition, fighterBTargetPosition, t);
            yield return null;
        }

        SetFinalPosition(m_fighterA.gameObject, fighterATargetPosition);
        SetFinalPosition(m_fighterB.gameObject, fighterBTargetPosition);
        OnFightStarted?.Invoke(fightDuration, m_oddsManager.GetFighterAName, m_oddsManager.GetFighterBName);
    }

    private void LerpToTargetPosition(GameObject uiComponent, Vector3 startPos, Vector3 targetPos, float t)
    {
        Vector3 currentPos = startPos;
        currentPos.x = Mathf.Lerp(currentPos.x, targetPos.x, t);
        currentPos.y = Mathf.Lerp(currentPos.y, targetPos.y, t);
        uiComponent.transform.localPosition = currentPos;
    }

    private void SetFinalPosition(GameObject uiComponent, Vector3 targetPos)
    {
        Vector3 finalPos = uiComponent.transform.localPosition;
        finalPos.x = targetPos.x;
        uiComponent.transform.localPosition = finalPos;
    }
}
