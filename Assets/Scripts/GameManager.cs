using UnityEngine;

public class GameManager : MonoBehaviour
{
    private IEventHandler events;
    public LevelData[] _Levels;
    public LevelData CurrentLevel;
    int CurrentLevelNo;
    

    private void Awake()
    {
        events = GetComponent<EventHandler>();
        events.Subscribe("OnWin", OnLevelWin);
    }

    private void Start()
    {
        CurrentLevelNo = PlayerPrefs.GetInt("CurrentLevelNo", 0) % _Levels.Length;
        CurrentLevel = Instantiate(_Levels[CurrentLevelNo]);
    }

    private void OnLevelWin()
    {
        print("----------YOU WIN!----------");

        events.Publish("OnWinEffect");
        events.Publish("OnWinSound");

        PlayerPrefs.SetInt("CurrentLevelNo", PlayerPrefs.GetInt("CurrentLevelNo") + 1);
    }

   
}
