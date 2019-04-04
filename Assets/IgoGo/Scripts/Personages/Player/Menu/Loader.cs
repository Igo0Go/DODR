using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{

    public Slider slider;
    private AsyncOperation loader;
    void Start()
    {
        loader = SceneManager.LoadSceneAsync(LoadManager.NameSceneForLoad);
        
    }

    
    void Update()
    {
        slider.value = loader.progress;
    }
}
