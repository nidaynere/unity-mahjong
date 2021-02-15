using UnityEngine;
using UnityEngine.UI;

public class UIMapItem : MonoBehaviour
{
    [SerializeField] private Text mapNameText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject star;

    public void Set(string mapName)
    {
        mapNameText.text = mapName;
        button.onClick.AddListener(() => {
            GameManager.LoadGame(mapNameText.text);
        });
    }

    public void Refresh (bool IsCompleted)
    {
        star.gameObject.SetActive(IsCompleted);
    }
}