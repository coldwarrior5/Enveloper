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
        InfoMenuHolder.SetActive(false);
        InfoModelHolder.SetActive(false);
        InfoImagesHolder.SetActive(false);
        bool render = false;

        if (GameManager.i.checkContinue())
        {
            SceneManager.LoadScene("Main scene");
        }
        else
        {
            if (GameManager.i.getModel().Equals(""))
            {
                InfoMenuHolder.GetComponentInChildren<Text>().text = "Morate učitati .obj dokument";
                render = true;
            }
            else
            {
                if (GameManager.i.correctFormat(new List<string>(), GameManager.i.getModel(), "Model"))
                {
                    InfoModelHolder.GetComponentInChildren<Text>().text = "Neispravan dokument";
                    InfoModelHolder.SetActive(true);
                }
            }
            if (GameManager.i.getTexture().Equals("") && GameManager.i.getArray().Count != 0)
            {

                if (GameManager.i.getArray().Count < 3)
                {
                    InfoImagesHolder.GetComponentInChildren<Text>().text = "Trebate učitati minimalno tri slike";
                    InfoImagesHolder.SetActive(true);
                }
                else if (GameManager.i.correctFormat(GameManager.i.getArray(), "", "Images"))
                {
                    InfoModelHolder.GetComponentInChildren<Text>().text = "Prihvatljiv format slika: .jpg, .png, .gif";
                    InfoModelHolder.SetActive(true);
                }
                
            }
            else if (!GameManager.i.getTexture().Equals("") && GameManager.i.getArray().Count == 0)
            {
                if (GameManager.i.correctFormat(new List<string>(), GameManager.i.getTexture(), "Texture"))
                {
                    InfoTextureHolder.GetComponentInChildren<Text>().text = "Prihvatljiv format teksture: .jpg, .png, .gif";
                    InfoTextureHolder.SetActive(true);
                }
            }
            else if(GameManager.i.getTexture().Equals("") && GameManager.i.getArray().Count == 0)
            {
                if (render)
                {
                    InfoMenuHolder.GetComponentInChildren<Text>().text += " te morate učitati teksturu ili slike";
                }
                else
                {
                    InfoMenuHolder.GetComponentInChildren<Text>().text = "Morate učitati teksturu ili slike";
                    render = true;
                }
            }
            else
            {
                if (render)
                {
                    InfoMenuHolder.GetComponentInChildren<Text>().text += " te ne možete učitati teksturu i slike";
                    GameManager.i.setArray(new List<string>());
                    GameManager.i.setTexture("");
                }
                else
                {
                    InfoMenuHolder.GetComponentInChildren<Text>().text = "Učitajte samo teksturu ili samo slike";
                    GameManager.i.setArray(new List<string>());
                    GameManager.i.setTexture("");
                    render = true;
                }
            }
            if (render)
            {
                InfoMenuHolder.SetActive(true);
            }
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
        SceneManager.LoadScene("MainMenu");
    }

    void Start()
    {
        InfoMenuHolder.SetActive(false);
        InfoModelHolder.SetActive(false);
        InfoImagesHolder.SetActive(false);
        LoadMenuHolder.SetActive(true);
    }


}
