using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pausePanel;
    //public GameObject settingPanel;


    public ReplicSceneLoader loader;

    // Use this for initialization
    void Start()
    {
        pausePanel.SetActive(false);
        //settingPanel.SetActive(false);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ContinueButtonClick();
                return;
            }

//            if (settingPanel.activeSelf)
//            {
//                BackSettingButtonClick();
//            }
            Time.timeScale = 0.001f;
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ContinueButtonClick()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    public void ReloadButtonClick()
    {
        Time.timeScale = 1;
        LoadManager.NameSceneForLoad = SceneManager.GetActiveScene().name;
        loader.loader.allowSceneActivation = true;
    }

    public void SettingButtonClick()
    {
        pausePanel.SetActive(false);
        //  settingPanel.SetActive(true);
    }

    public void ExidButtonClick()
    {
        Time.timeScale = 1;
        LoadManager.NameSceneForLoad = "Menu";
        loader.loader.allowSceneActivation = true;
    }

    public void BackSettingButtonClick()
    {
        pausePanel.SetActive(true);
        //  settingPanel.SetActive(false);
    }
}