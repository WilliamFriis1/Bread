using TMPro;
using UnityEngine;

public class EvalBarBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject m_evalIconObj;
    [SerializeField] private TextMeshProUGUI m_fighterAText;
    [SerializeField] private TextMeshProUGUI m_fighterBText;
    private FightMenuBehaviour m_fightMenuBehaviour;
    private Vector3 m_evalIconOrigin;
    private Vector3 m_evalIconRightMaximum;
    private Vector3 m_evalIconLeftMaximum;
    private float m_distanceFromOrigin = 150;

    #region Unity Methods
    private void Start()
    {
        m_fightMenuBehaviour = GetComponent<FightMenuBehaviour>();
        m_evalIconOrigin = m_evalIconObj.transform.localPosition;
        m_evalIconLeftMaximum = m_evalIconOrigin + new Vector3(-m_distanceFromOrigin, 0, 0);
        m_evalIconRightMaximum = m_evalIconOrigin + new Vector3(m_distanceFromOrigin, 0, 0);
    }
    #endregion


}
