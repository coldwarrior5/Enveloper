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
        GUILayout.TextField("Odabrani dokumenti: " + output);
        //draw and display output
        int returnedValue = fb.draw();
        if (returnedValue ==1)
        { //true is returned when a file has been selected
          //the output file is a member if the FileInfo class, if cancel was selected the value is null
            newFile = fb.outputFile;
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
        else if (returnedValue == 2)
        {

            if (GameManager.i.getType().Equals("Model"))
            {
                string isNull = "";
                if (array[0] != null)
                {
                    isNull = array[0];
                }
                GameManager.i.setModel(isNull);
            }
            else if (GameManager.i.getType().Equals("Texture"))
            {
                string isNull = "";
                if (array[0] != null)
                {
                    isNull = array[0];
                }
                if (GameManager.i.getBoth())
                {
                    GameManager.i.setArray(new List<string>());
                }
                GameManager.i.setTexture(isNull);
            }
            else if (GameManager.i.getType().Equals("Images"))
            {
                if (GameManager.i.getBoth())
                {
                    GameManager.i.setTexture("");
                }
                foreach (string element in array)
                {
                    if (element == null)
                    {
                        array.Remove(element);
                    }
                }

                GameManager.i.setArray(array);
            }
            SceneManager.LoadScene("LoadObject");
        }
	}
}
