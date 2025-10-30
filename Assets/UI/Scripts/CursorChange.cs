using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorState m_newCursorState;
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorController.Instance.UpdateCursor(m_newCursorState);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorController.Instance.UpdateCursor(CursorState.Default);
    }
}
