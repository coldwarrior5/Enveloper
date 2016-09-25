using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class testFileBrowser : MonoBehaviour {
	//skins and textures
	public GUISkin[] skins;
    string output = "Nije odabran niti jedan dokument", newFile, type;
    public Texture2D file,folder,back,drive;
    List<string> array = new List<string>();

    //initialize file browser
    FileBrowser fb = new FileBrowser();
	// Use this for initialization
	void Start () {
        type = GameManager.i.getType();
		//setup file browser style
		fb.guiSkin = skins[0]; //set the starting skin
		//set the various textures
		fb.fileTexture = file; 
		fb.directoryTexture = folder;
		fb.backTexture = back;
		fb.driveTexture = drive;
	}
	
	void OnGUI(){
        GUILayout.TextArea("Odabrani dokumenti: " + output);
        //draw and display output
        int returnedValue = fb.draw();
        if (returnedValue == 1)
        { //true is returned when a file has been selected
          //the output file is a member if the FileInfo class, if cancel was selected the value is null
            newFile = fb.outputFile;
            if (newFile != null) { 
                if (output.Equals("Nije odabran niti jedan dokument") || type.Equals("Model") || type.Equals("Texture"))
                {
                    output = newFile;
                    if (array.Count != 0)
                    {
                        array[0] = newFile;
                    }
                    else
                    {
                        array.Add(newFile);
                    }
                }
                else
                {
                       output += ", " + newFile;
                       array.Add(newFile);
                }
            }
        }
        else if (returnedValue == 2)
        {
            bool render = false;

            //Model
            if (GameManager.i.getType().Equals("Model"))
            {
                string isNull = "";
                if (array.Count != 0 && array[0] != "")
                {
                    isNull = array[0];
                    if (GameManager.i.correctFormat(new List<string>(), isNull, "Model"))
                    {
                        GameManager.i.setModel(isNull);
                        GameManager.i.setInfoModel("Uspješno učitan model");
                    }
                    else
                    {
                        GameManager.i.setInfoModel("Model je neispravnog formata");
                        GameManager.i.setInfoMenu("Morate učitati model");
                        render = true;
                    }
                }
                else
                {
                    GameManager.i.setInfoMenu("Morate učitati model");
                    render = true;
                }
                
            }

            //UV map
            else if (GameManager.i.getType().Equals("Texture"))
            {
                string isNull = "";
                if (array.Count != 0 && array[0] != "")
                {
                    isNull = array[0];
                    if (GameManager.i.correctFormat(new List<string>(), isNull, "Texture"))
                    {
                        GameManager.i.setInfoTexture("Uspješno učitana UV mapa");
                        GameManager.i.setInfoImages("");
                        GameManager.i.setTexture(isNull);
                        if (GameManager.i.getArray().Count != 0)
                        {
                            GameManager.i.setBoth(true);
                        }
                    }
                    else
                    {
                        GameManager.i.setInfoTexture("UV mapa je neispravnog formata, ispravno: .jpg, .png, .gif");
                    }
                }
                if (GameManager.i.getBoth())
                {
                    GameManager.i.setArray(new List<string>());
                    GameManager.i.setInfoImages("Uklonjene prethodno odabrane slike");
                }
            }

            //Images
            else if (GameManager.i.getType().Equals("Images"))
            {
                foreach (string element in array)
                {
                    if (element == "")
                    {
                        array.Remove(element);
                    }
                }
                if (array.Count != 0)
                {
                    if (GameManager.i.correctFormat(array, "", "Images"))
                    {
                        GameManager.i.setInfoImages("Uspješno učitane slike");
                        GameManager.i.setInfoTexture("");

                        GameManager.i.setArray(array);
                        if (!GameManager.i.getTexture().Equals(""))
                        {
                            GameManager.i.setBoth(true);
                        }
                    }
                    else
                    {
                        GameManager.i.setInfoImages("Slike su neispravnog formata, ispravno: .jpg, .png, .gif");
                    }
                    if (GameManager.i.getBoth())
                    {
                        GameManager.i.setTexture("");
                        GameManager.i.setInfoTexture("Uklonjena prethodno odabrana UV mapa");
                    }
                }
            }


            //Conditions

            if (GameManager.i.getModel().Equals(""))
            {
                if (!render)
                {
                    GameManager.i.setInfoMenu("Morate učitati model");
                    render = true;
                }
                
            }
            if (GameManager.i.getTexture().Equals("") && GameManager.i.getArray().Count != 0)
            {
                if(!render)
                {
                    GameManager.i.setInfoMenu("");
                    render = true;
                }
            }
            else if (!GameManager.i.getTexture().Equals("") && GameManager.i.getArray().Count == 0)
            {
                if (!render)
                {
                    GameManager.i.setInfoMenu("");
                    render = true;
                }
            }
            else if (GameManager.i.getTexture().Equals("") && GameManager.i.getArray().Count == 0)
            {
                if (render)
                {
                    GameManager.i.setInfoMenu(GameManager.i.getInfoMenu() + " te morate učitati slike ili UV mapu");
                }
                else
                {
                    GameManager.i.setInfoMenu("Morate učitati UV mapu ili slike");
                    render = true;
                }
            }
            if (GameManager.i.checkContinue())
            {
                GameManager.i.setInfoMenu("Možete nastaviti");
                render = true;
            }
            SceneManager.LoadScene("LoadObject");
        }
    }
}
