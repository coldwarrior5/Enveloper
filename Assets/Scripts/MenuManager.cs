using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenuHolder;
    public GameObject CreditsHolder;
    public GameObject InfoHolder;

    public void Continue()
    {
        SceneManager.LoadScene("LoadObject");
    }

    public void About()
    {
        MainMenuHolder.SetActive(false);
        InfoHolder.SetActive(true);
    }

    public void Credits()
    {
        MainMenuHolder.SetActive(false);
        CreditsHolder.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        InfoHolder.SetActive(false);
        CreditsHolder.SetActive(false);
        MainMenuHolder.SetActive(true);
    }

    void Awake()
    {
        //check if directory doesn't exit
        if (!Directory.Exists("Generirane teksture"))
        {
            //if it doesn't, create it
            Directory.CreateDirectory("Generirane teksture");

        }
    }
}
