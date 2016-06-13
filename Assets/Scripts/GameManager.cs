using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public string type, modelPath="", texturePath="";
    List<string> array = new List<string>();
    public static GameManager i;

    void Awake()
    {
        if (!i)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void setType(string what)
    {
        type = what;
    }

    public string getType()
    {
        return type;
    }

    public void setArray(List<string> input)
    {
        array = input;
    }

    public List<string> getArray()
    {
        return array;
    }

    public void setModel(string input)
    {
        modelPath = input;
    }

    public string getModel()
    {
        return modelPath;
    }

    public void setTexture(string input)
    {
        texturePath = input;
    }

    public string getTexture()
    {
        return texturePath;
    }

    public bool checkContinue()
    {
        if (!modelPath.Equals("") && correctFormat(array, modelPath, "Model") && 
            ((!texturePath.Equals("") && array.Count==0 && correctFormat(array, texturePath, "Texture"))
            || (array.Count > 2 && !texturePath.Equals("") && correctFormat(array, "", "Images"))))
        {
            return true;
        }
        else return false;
    }

    public bool correctFormat(List<string> array, string input = "", string type = "")
    {
        if (type.Equals("Images"))
        {
            bool returnValue = true;
            foreach (string imagePath in array)
            {
                string ending = imagePath.Substring(imagePath.Length-3);
                if (!ending.Equals("jpg") && !ending.Equals("png") && !ending.Equals("gif"))
                {
                    returnValue = false;
                }
            }
            return returnValue;
        }
        else if (type.Equals("Model"))
        {
            string ending = input.Substring(input.Length - 3);
            if (ending.Equals("obj"))
            {
                return true;
            }
            else return false;
        }
        else if (type.Equals("Texture"))
        {
            string ending = input.Substring(input.Length - 3);
            if (ending.Equals("jpg") || ending.Equals("png") || ending.Equals("gif"))
            {
                return true;
            }
            else return false;
        }
        else return true;
    }
}