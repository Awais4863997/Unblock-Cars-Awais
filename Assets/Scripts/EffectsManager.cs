using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private IEventHandler events;

    void Awake()
    {
        events = FindObjectOfType<EventHandler>();

        events.Subscribe("OnEscapeEffect", EscapeEffect);
        events.Subscribe("OnWinEffect", WinEffect);
    }

    private void EscapeEffect()
    {
        print("----------- show Effect on Car Escape -------------");
    }

    private void WinEffect()
    {
        print("----------- show Effect on Level Win -------------");
    }

}
