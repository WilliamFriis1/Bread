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

            Vector3 startButtonPos = startButtonStartPos;
            startButtonPos.x = Mathf.Lerp(startButtonStartPos.x, m_targetstartButtonPosition.x, t);
            m_startButton.transform.localPosition = startButtonPos;

            Vector3 quitButtonPos = quitButtonStartPos;
            quitButtonPos.x = Mathf.Lerp(quitButtonStartPos.x, m_targetQuitButtonPosition.x, t);
            m_quitButton.transform.localPosition = quitButtonPos;

            Vector3 titlePos = titleStartPos;
            titlePos.x = Mathf.Lerp(titleStartPos.x, m_targetTitlePosition.x, t);
            m_titleText.transform.localPosition = titlePos;

            Vector3 characterPos = characterStartPos;
            characterPos.x = Mathf.Lerp(characterStartPos.x, m_targetCharacterPosition.x, t);
            m_characterImg.transform.localPosition = characterPos;

            yield return null;
        }

        Vector3 finalStartButtonPos = m_startButton.transform.localPosition;
        finalStartButtonPos.x = m_targetstartButtonPosition.x;
        m_startButton.transform.localPosition = finalStartButtonPos;

        Vector3 finalQuitButtonPos = m_quitButton.transform.localPosition;
        finalQuitButtonPos.x = m_targetQuitButtonPosition.x;
        m_quitButton.transform.localPosition = finalQuitButtonPos;

        Vector3 finalTitlePos = m_titleText.transform.localPosition;
        finalTitlePos.x = m_targetTitlePosition.x;
        m_titleText.transform.localPosition = finalTitlePos;

        Vector3 finalCharacterPos = m_characterImg.transform.localPosition;
        finalCharacterPos.x = m_targetCharacterPosition.x;
        m_characterImg.transform.localPosition = finalCharacterPos;
    }
}
