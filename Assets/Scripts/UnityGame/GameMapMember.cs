using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class GameMapMember : MonoBehaviour, IPointerClickHandler
{
    public static GameMapMember Active;

    [SerializeField] private GameObject activeEffect;
    [SerializeField] private GameObject highlightEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int MapIndex;

    public delegate void matchChecker(int t1, int t2);
    private matchChecker checkForMatch;

    public void SetHighlight(bool value)
    {
        if (value != highlightEffect.activeSelf)
        highlightEffect.SetActive(value);
    }

    public void Set (int Index, Sprite sprite, matchChecker mChecker)
    {
        MapIndex = Index;
        spriteRenderer.sprite = sprite;
        checkForMatch = mChecker;
    }

    public void SetActiveness(bool value)
    {
        activeEffect.SetActive(value);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Active != null && Active != this)
        {
            // Check for match.
            checkForMatch?.Invoke(Active.MapIndex, MapIndex);
            SetActiveness(false);
            Active = null;
            return;
        }

        if (Active != null)
        {
            SetActiveness(false);

            bool isThisActive = Active == this; 
            Active = null;
            if (isThisActive)
                return;
        }

        Active = this;
        SetActiveness(true);
    }
}