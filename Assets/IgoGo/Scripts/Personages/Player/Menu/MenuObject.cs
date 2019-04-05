using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum MenuType
{
    Play,
    Exit
}

public class MenuObject : MonoBehaviour
{
    public MenuType type;
    public MenuScript menu;


    private SimpleHandler action;
    private AsyncOperation loader;

    void Start()
    {
        
        switch (type)
        {
            case MenuType.Exit:
                action = ExitFunction;
                break;
            case MenuType.Play:
                action = PlayFunction;
                LoadManager.NameSceneForLoad = "0 Tutorial";
                loader = SceneManager.LoadSceneAsync("Load");
                loader.allowSceneActivation = false;
                break;
        }
    }

    private void OnMouseUpAsButton()
    {
        action();
    }

    private void PlayFunction()
    {
        loader.allowSceneActivation = true;

        Debug.Log("Играть!");
    }


    private void ExitFunction()
    {
        Application.Quit();
        Debug.Log("Выход.");
    }
}

public static class LoadManager
{
    public static string NameSceneForLoad = "Menu";
}