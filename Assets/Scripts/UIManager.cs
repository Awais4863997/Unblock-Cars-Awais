using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject WinPanel;

    private void Awake()
    {
        var events = FindObjectOfType<EventHandler>();
        events.Subscribe("OnWin", OnLevelWin);
    }

    private void OnLevelWin()
    {
        print("----------UIMANAGER YOU WIN!----------");
        WinPanel.gameObject.SetActive(true);
    }

    public void NextButtonFun()
    {
        print("----------Press Next Btn----------");
    }

}
