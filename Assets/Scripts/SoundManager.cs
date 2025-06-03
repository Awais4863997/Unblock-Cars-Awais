using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip moveClip, winClip;
    private AudioSource src;

    private IEventHandler events;

    private void Awake()
    {
        src = gameObject.AddComponent<AudioSource>();

        events = FindObjectOfType<EventHandler>();

        events.Subscribe("OnWinSound", PlayWinSound);
        events.Subscribe("OnMoveSound", PlayMoveSound);
    }

    private void PlayMoveSound()
    {
        print("----------Play Move Sound----------");
        src.PlayOneShot(moveClip);
    }

    private void PlayWinSound()
    {
        print("----------Play Win Sound----------");
        src.PlayOneShot(winClip);
    }

}
