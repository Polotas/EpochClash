using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public AudioClip audioButtonClick;
    public AudioClip audioCoinCollect;

    public void Start()
    {
        AudioManager.SetButtonClip(audioButtonClick);
        AudioManager.SetCoinCollect(audioCoinCollect);
        AudioManager.SetVolume(AudioManager.AudioType.FX,0.5f);
    }
}
