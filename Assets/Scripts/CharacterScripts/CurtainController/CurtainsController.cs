using System.Collections;
using System.Security;
using UnityEngine;

public class CurtainsController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float openClipLenght = 1f;
    [SerializeField] private float closeClipLenght = 1f;

    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    public IEnumerator Open()
    {
        if (!animator)
        {
            yield break;
        }
        else
        {
            animator.SetTrigger(openTrigger);
        }
        yield return new WaitForSeconds(openClipLenght);
    }
    public IEnumerator Close()
    {
        if (!animator)
        {
            yield break;
        }
        else
        {
            animator.SetTrigger(closeTrigger);
        }
        yield return new WaitForSeconds(closeClipLenght);
    }
}
