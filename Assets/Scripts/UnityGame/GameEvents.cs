using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    #region Inputs
    public static Action OnUserWantsHint;
    public static Action OnUserDeselect;
    #endregion

    #region Outputs
    /// <summary>
    /// game finished with win or lose.
    /// mapId, isSucceeded, score.
    /// </summary>
    public static Action<string, bool, int> OnGameFinished;
    public static Action<int> OnScoreUpdate;
    public static Action OnGameStarting;
    #endregion

    #region Unity calls.
    public void CancelGame ()
    {
        OnGameFinished?.Invoke(null, false, 0);
    }

    public void UserWantsHint()
    {
        OnUserWantsHint?.Invoke();
    }

    public void UserWantsDeselect()
    {
        OnUserDeselect?.Invoke();
    }
    #endregion
}
