using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MusicBox
{
    public AudioClip clip;
    public bool loop;
    public bool hide;
}

public abstract class MyTools : MonoBehaviour
{
    public static bool MyGetComponent<T>(GameObject obj, out T component)
    {
        component = obj.GetComponent<T>();
        if (component == null)
        {
            return false;
        }
        return true;
    }

    public static bool ContainsPhisicsMaterial(Collider obj, out PhysicMaterial component)
    {
        component = obj.sharedMaterial;
        if (component == null)
        {
            return false;
        }
        return true;
    }

    public static float SmoothlyChange(float min, float max, float value, float step)
    {
        if (max <= min)
        {
            Debug.LogError("В методе SmootlyChange неправильно указаны границы.");
        }

        float result = value;

        result += step;
        if (result > max || result < min)
        {
            if (result > max)
            {
                return max;
            }
            if (result < min)
            {
                return min;
            }
        }
        return result;
    }

    public static float SmoothlyChange(float value, float target, float step)
    {
        if (value == target)
        {
            return target;
        }

        int multiply = (target >= value) ? 1 : -1;

        float dir = multiply * step;

        if (Mathf.Abs(target - value) > dir)
        {
            return value + dir;
        }
        else
        {
            return target;
        }
    }

    public void Invoke(SimpleHandler task, float time)
    {
        Invoke(task.Method.Name, time);
    }
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

    [Space(20)]
    public bool debug;
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
        if(debug)
        {
            CurrentBox = number;
        }
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
                CurrentBox = next;
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
        if(!musicBoxes[number].hide)
        {
            anim.SetTrigger("ChangeMusic");
        }
        source.loop = musicBoxes[number].loop;
        targetVolume = maxVolume;
        source.Play();
    }
}
