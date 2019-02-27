using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MenuType
{
    Play,
    Exit
}

public class MenuObject : MonoBehaviour {

    public MenuType type;
    public MenuScript menu;


    private SimpleHandler action;

    void Start () {
		switch (type)
        {
            case MenuType.Exit:
                action = ExitFunction;
                break;
            case MenuType.Play:
                action = PlayFunction;
                break;
        }
	}

    private void OnMouseUpAsButton()
    {
        action();
    }

    private void PlayFunction()
    {
        Debug.Log("Играть!");
    }
    private void ExitFunction()
    {
        Debug.Log("Выход.");
    }
}
