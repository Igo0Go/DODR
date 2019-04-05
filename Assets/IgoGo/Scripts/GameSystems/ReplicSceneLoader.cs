using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplicSceneLoader : UsingObject
{
    [SerializeField] private string sceneName = "Menu";


    private bool replicComplete;
    [SerializeField] private bool trigger = false;
    private AsyncOperation loader;

    private void Start()
    {
        LoadManager.NameSceneForLoad = sceneName;
        loader = SceneManager.LoadSceneAsync("Load");
        loader.allowSceneActivation = false;
    }

    public void CompleteReplic()
    {
        replicComplete = true;
        CheckComplete();
    }


    private void CheckComplete()
    {
        if (replicComplete && trigger)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        loader.allowSceneActivation = true;
    }

    public override void Use()
    {
        trigger = true;
        CheckComplete();
    }
}