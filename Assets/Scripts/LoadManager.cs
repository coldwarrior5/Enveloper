using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadManager : MonoBehaviour
{
    public GameObject LoadMenuHolder;
    public GameObject InfoMenuHolder;
    public GameObject InfoModelHolder;
    public GameObject InfoImagesHolder;
    public GameObject InfoTextureHolder;

    public void Continue()
    {
        if (GameManager.i.checkContinue())
        {
            SceneManager.LoadScene("Main scene");
        } 
    }

    public void Model()
    {

        GameManager.i.setType("Model");
        SceneManager.LoadScene("tst");
    }

    public void Images()
    {
        GameManager.i.setType("Images");
        SceneManager.LoadScene("tst");
    }

    public void Texture()
    {
        GameManager.i.setType("Texture");
        SceneManager.LoadScene("tst");
    }

    public void Return()
    {
        GameManager.i.setArray(new List<string>());
        GameManager.i.setModel("");
        GameManager.i.setTexture("");
        GameManager.i.setBoth(false);
        GameManager.i.setType("");
        SceneManager.LoadScene("MainMenu");
    }

    void Start()
    {
        InfoMenuHolder.GetComponentInChildren<Text>().text = GameManager.i.getInfoMenu();
        InfoModelHolder.GetComponentInChildren<Text>().text = GameManager.i.getInfoModel();
        InfoImagesHolder.GetComponentInChildren<Text>().text = GameManager.i.getInfoImages();
        InfoTextureHolder.GetComponentInChildren<Text>().text = GameManager.i.getInfoTexture();

        InfoMenuHolder.SetActive(true);
        InfoModelHolder.SetActive(true);
        InfoImagesHolder.SetActive(true);
        InfoTextureHolder.SetActive(true);
        LoadMenuHolder.SetActive(true);
    }


}
