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
        GameManager.i.setInfoMenu("Morate učitati model te slike ili UV mapu");
        GameManager.i.setInfoModel("Moguće učitati samo jedan model");
        GameManager.i.setInfoImages("Jedna ili više slika .jpg, .png, ili .gif formata");
        GameManager.i.setInfoTexture("Jedna UV mapa .jpg, .png, ili .gif formata");
        //check if directory doesn't exit
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        if (!Directory.Exists(path+"\\Oblozitelj"))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(path+"/Oblozitelj");

        }
    }
}
