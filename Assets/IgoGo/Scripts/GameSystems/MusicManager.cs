using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MusicBox
{
    public AudioClip clip;
    public bool loop;
}

public class MusicManager : MyTools {

    public Text audioName;
    public AudioSource source;
    public Animator anim;
    [Space(10)]
    public MusicBox[] musicBoxes;

    
    public int CurrentBox
    {
        get
        {
            return _currentBox;
        }
        set
        {
            if(_currentBox != value)
            {
                _currentBox = value;
                targetVolume = 0;
                change = true;
            }
        }
    }

    public int number;

    private int _currentBox;
    private bool change;
    private float targetVolume;
    private float maxVolume;

    private void Start()
    {
        maxVolume = source.volume;
        ChangeClip(0);
    }

    private void Update()
    {
        CurrentBox = number;
        SetMusicBox();
        CheckMusic();
    }

    private void SetMusicBox()
    {
        if (change)
        {
            source.volume = SmoothlyChange(source.volume, targetVolume, Time.deltaTime * 0.5f);

            if (source.volume == 0)
            {
                ChangeClip(_currentBox);
            }
            else if (source.volume == maxVolume)
            {
                change = false;
            }
        }
    }
    private void CheckMusic()
    {
        if(!source.loop)
        {
            if(!source.isPlaying)
            {
                int next = CurrentBox + 1;
                if(next > musicBoxes.Length - 1)
                {
                    next = 0;
                }
                number = next;
            }
        }
    }
    private void ChangeClip(int number)
    {
        if(number < 0 || number > musicBoxes.Length - 1)
        {
            Debug.LogError("MusicManager. Передан некорректный нормер музыкальной заготовки");
        }
        if(source.isPlaying)
        {
            source.Stop();
        }
        source.clip = musicBoxes[number].clip;
        audioName.text = source.clip.name;
        anim.SetTrigger("ChangeMusic");
        source.loop = musicBoxes[number].loop;
        targetVolume = maxVolume;
        source.Play();
    }
}
