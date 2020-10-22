using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public AudioSource audioSource;
    public AudioClip button;
    public AudioClip correct;
    public AudioClip error;
    public AudioClip fail;
    public AudioClip newQuestion;
    public AudioClip victory;

    private void Awake()
    {
        instance = this;
    }

    public void UIMButtonSound()
    {
        PlaySoundOf(button);
    }
    
    public void UIMCorrectSound()
    {
        PlaySoundOf(correct);
    }
    
    public void UIMErrorSound()
    {
        PlaySoundOf(error);
    }
    
    public void UIMFailSound()
    {
        PlaySoundOf(fail);
    }
    
    public void UIMVictorySound()
    {
        PlaySoundOf(victory);
    }
    
    public void UIMNewQuestionSound()
    {
        PlaySoundOf(newQuestion);
    }

    private void PlaySoundOf(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
}
