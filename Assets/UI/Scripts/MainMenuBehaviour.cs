using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private Image m_characterImg;
    private CanvasGroup m_menuGroup;

    private Vector3 m_targetstartButtonPosition;
    private Vector3 m_targetQuitButtonPosition;
    private Vector3 m_targetTitlePosition;
    private Vector3 m_targetCharacterPosition;

    private void Start()
    {
        m_menuGroup = GetComponentInChildren<CanvasGroup>();
        m_targetstartButtonPosition = m_startButton.GetComponent<RectTransform>().localPosition;
        m_targetQuitButtonPosition = m_quitButton.GetComponent<RectTransform>().localPosition;
        m_targetTitlePosition = m_titleText.GetComponent<RectTransform>().localPosition;
        m_targetCharacterPosition = m_characterImg.GetComponent<RectTransform>().localPosition;

        InitUIComponent(m_startButton.gameObject, 1000);
        InitUIComponent(m_quitButton.gameObject, 1000);
        InitUIComponent(m_titleText.gameObject, 1000);
        InitUIComponent(m_characterImg.gameObject, -1200);
        m_menuGroup.alpha = 0.0f;
        StartCoroutine(FadeCanvasGroup(m_menuGroup, 0f, 1f, 1.5f));
        StartCoroutine(AnimateMenuUI(1f));
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void InitUIComponent(GameObject uiComponent, float value)
    {
        Vector3 newPos = uiComponent.transform.localPosition;
        newPos.x += value;
        uiComponent.transform.localPosition = newPos;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float finalAlpha, float duration)
    {
        float elapsedTime = 0f;
        group.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, finalAlpha, elapsedTime / duration);
            yield return null;
        }

        group.alpha = finalAlpha;
    }

    IEnumerator AnimateMenuUI(float duration)
    {
        float elapsedTime = 0f;

        Vector3 startButtonStartPos = m_startButton.transform.localPosition;
        Vector3 quitButtonStartPos = m_quitButton.transform.localPosition;
        Vector3 titleStartPos = m_titleText.transform.localPosition;
        Vector3 characterStartPos = m_characterImg.transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            LerpToTargetPosition(m_startButton.gameObject, startButtonStartPos, m_targetstartButtonPosition, t);
            LerpToTargetPosition(m_quitButton.gameObject, quitButtonStartPos, m_targetQuitButtonPosition, t);
            LerpToTargetPosition(m_titleText.gameObject, titleStartPos, m_targetTitlePosition, t);
            LerpToTargetPosition(m_characterImg.gameObject, characterStartPos, m_targetCharacterPosition, t);
            yield return null;
        }

        SetFinalPosition(m_startButton.gameObject, m_targetstartButtonPosition);
        SetFinalPosition(m_quitButton.gameObject, m_targetQuitButtonPosition);
        SetFinalPosition(m_titleText.gameObject, m_targetTitlePosition);
        SetFinalPosition(m_characterImg.gameObject, m_targetCharacterPosition);
    }

    private void SetFinalPosition(GameObject uiComponent, Vector3 targetPos)
    {
        Vector3 finalPos = uiComponent.transform.localPosition;
        finalPos.x = targetPos.x;
        uiComponent.transform.localPosition = finalPos;
    }

    private void LerpToTargetPosition(GameObject uiComponent, Vector3 startPos, Vector3 targetPos, float t)
    {
        Vector3 currentPos = startPos;
        currentPos.x = Mathf.Lerp(currentPos.x, targetPos.x, t);
        uiComponent.transform.localPosition = currentPos;
    }
}
