using System;
using TMPro;
using UnityEngine;

public class EvalBarBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject m_evalIconObj;
    [SerializeField] private TextMeshProUGUI m_fighterAText;
    [SerializeField] private TextMeshProUGUI m_fighterBText;

    private FightMenuBehaviour m_fightMenuBehaviour;
    private Vector3 m_evalIconOrigin;
    private float m_distanceFromOrigin = 150;

    #region Unity Methods
    private void Start()
    {
        m_evalIconOrigin = m_evalIconObj.transform.localPosition;
    }

    private void OnEnable()
    {
        m_fightMenuBehaviour = GetComponent<FightMenuBehaviour>();
        if (m_fightMenuBehaviour != null)
            m_fightMenuBehaviour.OnFightStarted += UpdateEvalBar;

    }private void OnDisable()
    {
        m_fightMenuBehaviour = GetComponent<FightMenuBehaviour>();
        if (m_fightMenuBehaviour != null)
            m_fightMenuBehaviour.OnFightStarted -= UpdateEvalBar;

    }
    #endregion

    private void UpdateEvalBar(float fightDuration, string fighterAName, string fighterBName)
    {
        Vector3 leftLimit = m_evalIconOrigin + new Vector3(m_distanceFromOrigin, 0, 0);
        Vector3 rightLimit = m_evalIconOrigin + new Vector3(-m_distanceFromOrigin, 0, 0);

        //float elapsedTime = 0f;
        //Vector3 newPos = new();
        //while (elapsedTime < fightDuration)
        //{
        //    elapsedTime += Time.deltaTime;
        //    float t = Mathf.Clamp01(elapsedTime / fightDuration);
        //    newPos.x = Mathf.Lerp(leftLimit.x, rightLimit.x, t);
        //    m_evalIconObj.transform.localPosition = newPos;
        //}

        if(GameManager.Instance.Player.GetSelectedFighterName() == fighterAName)
        {
            m_evalIconObj.transform.localPosition = leftLimit;
        }
        else
        {
            m_evalIconObj.transform.localPosition = rightLimit;
        }
    }
}
