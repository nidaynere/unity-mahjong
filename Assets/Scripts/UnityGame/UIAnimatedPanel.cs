using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class UIAnimatedPanel : MonoBehaviour
{
    [SerializeField] private Animator _uiAnimator;

    public bool activeSelf = false;

    private void OnEnable()
    {
        _uiAnimator.SetBool("IsOpened", activeSelf);
    }

    public void SetPanel(bool Value)
    {
        activeSelf = Value;

        gameObject.SetActive(true);

        _uiAnimator.SetBool("IsOpened", Value);
    }

    public void Close()
    {
        SetPanel(false);
    }

    public void Open(bool Value = true)
    {
        SetPanel(Value);
    }
}
