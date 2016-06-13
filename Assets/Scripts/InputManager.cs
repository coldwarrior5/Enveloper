using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {

    public GameObject SaveHolder;
    public Dropdown myDropdown;
    public Vector3 mousePosition, drag;
    public GameObject target;
    public string objPath, texturePath = null;
    public string[] photoPaths = null;

    // Use this for initialization
    void Start ()
    {
        myDropdown.onValueChanged.AddListener(delegate {
            myDropdownValueChangedHandler(myDropdown);
        });
        target = OBJLoader.LoadOBJFile(@"C:/Users/Dean/Downloads/Tiger/Tiger/Tiger.obj");
        string directory = "file://C:/Users/Dean/Downloads/Tiger/Tiger/Texture/bengal_tiger.jpg";
        WWW www = new WWW(directory);
        target.GetComponentInChildren<Renderer>().material.mainTexture = www.texture;
        float scale = 0.004F;
        target.transform.localScale = new Vector3(scale, scale, scale);
        
    }

    // Update is called once per frame
    public void Update()
    {
        var closer = Input.GetAxis("Mouse ScrollWheel");
        if (target != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mousePosition.x = Input.mousePosition.x;
            }
            if (Input.GetMouseButton(0))
            {
                transform.RotateAround(target.transform.position, Vector3.up, Input.mousePosition.x-mousePosition.x);
                mousePosition.x = Input.mousePosition.x;
                
            }
            if(closer != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, closer*3);
            }
        }
        
    }

    public void Save()
    {
        Texture2D output = target.GetComponentInChildren<Renderer>().material.mainTexture as Texture2D;
        byte[] bytes = output.EncodeToPNG();
        string filename = TextureName(GameObject.Find("Name").GetComponent<Text>().text);
        System.IO.File.WriteAllBytes(filename, bytes);
        SaveHolder.SetActive(false);
    }

    public void Hide()
    {
        GameObject.Find("Name").GetComponent<Text>().text = "";
        SaveHolder.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Destroy()
    {
        myDropdown.onValueChanged.RemoveAllListeners();
    }

    private void myDropdownValueChangedHandler(Dropdown target)
    {
        switch (target.value)
        {
            case 0:
                SaveHolder.SetActive(true);
                break;
            case 1:
                MainMenu();
                break;
        }
        target.value = 2;
    }

    public static string TextureName(string name)
    {
        return string.Format("{0}/../Generirane teksture/{1}_{2}.png", Application.dataPath, name, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm"));
    }

    public void AddPictures(string[] array)
    {
        photoPaths = array;
    }

}

   
