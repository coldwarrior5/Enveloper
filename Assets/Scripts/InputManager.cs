using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class InputManager : MonoBehaviour {

    public GameObject SaveHolder;
    public Dropdown myDropdown;
    private Vector3 mousePosition;
    public GameObject target;
    private string objPath, texturePath = null;
    private List<string> photoPaths = null;
    public float mMoveSpeed =  2.0f;
    public GameObject ProjectorPrefab;
    private List<GameObject> mProjectors = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {

        myDropdown.options.Clear();
        myDropdown.options.Add(new Dropdown.OptionData() {text = "Spremi UV mapu" });
        myDropdown.options.Add(new Dropdown.OptionData() { text = "Povratak u glavni izbornik" });
        myDropdown.options.Add(new Dropdown.OptionData() { text = "Ispeci UV mapu" });
        myDropdown.options.Add(new Dropdown.OptionData() { text = "Zatvori izbornik" });
        myDropdown.value = 3;
        myDropdown.onValueChanged.AddListener(delegate {
            myDropdownValueChangedHandler(myDropdown);
        });
        
        objPath = GameManager.i.getModel();
        texturePath = GameManager.i.getTexture();
        target = OBJLoader.LoadOBJFile(objPath);
        if (!texturePath.Equals(""))
        {
            WWW www = new WWW("file://"+ texturePath);
            target.GetComponentInChildren<Renderer>().material.mainTexture = www.texture;
        }

        photoPaths = GameManager.i.getArray();        
        float scale = 1.0f;
        float min =  GetMin(target.GetComponentInChildren<Renderer>().bounds.extents.x, target.GetComponentInChildren<Renderer>().bounds.extents.y, target.GetComponentInChildren<Renderer>().bounds.extents.z);
        if (min < 1.0)
        {
            scale = 1.0f / min;
        }
        float max = GetMax(target.GetComponentInChildren<Renderer>().bounds.extents.x, target.GetComponentInChildren<Renderer>().bounds.extents.y, target.GetComponentInChildren<Renderer>().bounds.extents.z);
        if (max > 5.0)
        {
            scale = 1.0f / max;
        }
        if (float.IsPositiveInfinity(scale))
            scale = 1.0f;
        //scale = Mathf.Clamp(scale, 0.0001f, 10000);
        target.transform.localScale = new Vector3(scale, scale, scale);
        if (photoPaths.Count != 0)
        {
            for (int i = 0; i < photoPaths.Count; ++i)
            {
                GameObject gObj = (GameObject)Instantiate(ProjectorPrefab, new Vector3(0.0f, i, 0.0f), Quaternion.identity);
                WWW www = new WWW("file://" + photoPaths[i]);
                gObj.GetComponent<ProjectorBehaviour>().mProjectionTexture = www.texture;
                mProjectors.Add(gObj);
            }
        }
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
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Vector3 newPos = Camera.main.transform.position;
                newPos.y -= mMoveSpeed*Time.deltaTime;
                Camera.main.transform.position = newPos;
                Camera.main.transform.LookAt(target.transform.position);
            }
                
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Vector3 newPos = Camera.main.transform.position;
                newPos.y += mMoveSpeed * Time.deltaTime;
                Camera.main.transform.position = newPos;
                Camera.main.transform.LookAt(target.transform.position);
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

    private T GetMin<T>(T a, T b, T c)
    {
        T retVal = Min(a, b);
        retVal = Min(retVal, c);
        return retVal;
    }
    private T GetMax<T>(T a, T b, T c)
    {
        T retVal = Max(a, b);
        retVal = Max(retVal, c);
        return retVal;
    }
    public static T Max<T>(T x, T y)
    {
        return (Comparer<T>.Default.Compare(x, y) > 0) ? x : y;
    }
    public static T Min<T>(T x, T y)
    {
        return (Comparer<T>.Default.Compare(x, y) < 0) ? x : y;
    }

    private float sign(int []p1, int[] p2, int[] p3)
    {
        return (p1[0] - p3[0]) * (p2[1] - p3[1]) - (p2[0] - p3[0]) * (p1[1] - p3[1]);
    }

    private bool PointInTriangle(int[] pt, int[] v1, int[] v2, int[] v3)
    {
        bool b1, b2, b3;

        b1 = sign(pt, v1, v2) < 0.0f;
        b2 = sign(pt, v2, v3) < 0.0f;
        b3 = sign(pt, v3, v1) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }

    public void BakeTexture()
    {
        // Mesh info
        int[] indices = target.GetComponentInChildren<MeshFilter>().mesh.GetIndices(0);
        Vector3[] vertices = target.GetComponentInChildren<MeshFilter>().mesh.vertices;
        List<Vector2> UVs = new List<Vector2>();
        target.GetComponentInChildren<MeshFilter>().mesh.GetUVs(0, UVs);
        Vector3[] normals = target.GetComponentInChildren<MeshFilter>().mesh.normals;
        

        // Create texure
        int texWidth = 1024, texHeight = 1024;
        Texture2D tex = new Texture2D(texWidth, texHeight);
        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                tex.SetPixel(x, y, Color.magenta);
            }
        }
                for (int i = 0; i < indices.GetLength(0); i += 3)
        {
            int i0 = indices[i+0];
            int i1 = indices[i+1];
            int i2 = indices[i+2];

            Vector3 p0 = vertices[i0];
            Vector3 p1 = vertices[i1];
            Vector3 p2 = vertices[i2];

            Vector3 n0 = normals[i0];
            Vector3 n1 = normals[i1];
            Vector3 n2 = normals[i2];

            Vector2 uv0 = UVs[i0];
            Vector2 uv1 = UVs[i1];
            Vector2 uv2 = UVs[i2];

            int tex0x = (int)(uv0.x * texWidth);
            int tex0y = (int)(uv0.y * texHeight);

            int tex1x = (int)(uv1.x * texWidth);
            int tex1y = (int)(uv1.y * texHeight);

            int tex2x = (int)(uv2.x * texWidth);
            int tex2y = (int)(uv2.y * texHeight);

            int leftBound = GetMin(tex0x, tex1x, tex2x);
            int rightBound = GetMax(tex0x, tex1x, tex2x);
            int upBound = GetMin(tex0y, tex1y, tex2y);
            int downBound = GetMax(tex0y, tex1y, tex2y);

            for (int x = leftBound; x <= rightBound; ++x)
            {
                for (int y = upBound; y <= downBound; ++y)
                {
                    if (PointInTriangle(new int[] { x, y }, new int[] { tex0x, tex0y }, new int[] { tex1x, tex1y }, new int[] { tex2x, tex2y }) ||
                        PointInTriangle(new int[] { x, y }, new int[] { tex0x, tex0y }, new int[] { tex2x, tex2y }, new int[] { tex1x, tex1y }))
                    {
                        float u = (float)x / texWidth;
                        float v = (float)y / texHeight;

                        Vector4 bari = new Vector4();
                        Vector4 a = new Vector4(u, v, 1.0f, 0.0f);
                        Matrix4x4 M = new Matrix4x4();
                        // first row
                        M.m00 = uv0.x;
                        M.m01 = uv1.x;
                        M.m02 = uv2.x;
                        M.m03 = 0.0f;
                        // second row
                        M.m10 = uv0.y;
                        M.m11 = uv1.y;
                        M.m12 = uv2.y;
                        M.m13 = 0.0f;
                        // third row
                        M.m20 = 1.0f;
                        M.m21 = 1.0f;
                        M.m22 = 1.0f;
                        M.m23 = 0.0f;
                        // forth row
                        M.m30 = 0.0f;
                        M.m32 = 0.0f;
                        M.m33 = 0.0f;
                        M.m33 = 1.0f;

                        bari = M.inverse * a;
                        Vector3 textelPos3 = bari.x * p0 + bari.y * p1 + bari.z * p2;
                        Vector3 textelNormal = bari.x * n0 + bari.y * n1 + bari.z * n2;
                        Vector4 textelPos4 = new Vector4(textelPos3.x, textelPos3.y, textelPos3.z, 1.0f);
                        Matrix4x4 W = target.transform.localToWorldMatrix;
                        textelNormal = W * textelNormal;
                        textelNormal.Normalize();
                        Color c = Color.black;

                        float wSum = 0.0f;
                        float[] w = new float[mProjectors.Count];
                        for (int j = 0; j < mProjectors.Count; ++j)
                        {
                            float dot = Vector3.Dot(textelNormal, mProjectors[j].GetComponent<ProjectorBehaviour>().GetDir);
                            if (dot > 0.0f) dot = 0.0f;
                            dot *= -1.0f;
                            wSum += dot;
                            w[j] = dot;
                        }
                        float wSumRec = 1.0f / wSum;

                        for (int j = 0; j < mProjectors.Count; ++j)
                        {
                            Matrix4x4 V = mProjectors[j].GetComponent<ProjectorBehaviour>().GetView;
                            Matrix4x4 P = Camera.main.projectionMatrix;

                            Vector4 p = (P * V * W) * textelPos4;
                            p = p / p.w;
                            p.x = -0.5f * p.x + 0.5f;
                            p.y = -0.5f * p.y + 0.5f;
                            Texture2D texTemp = mProjectors[j].GetComponent<ProjectorBehaviour>().mProjectionTexture;
                            int xCoord = (int)(p.x * texTemp.width), yCoord = (int)(p.y * texTemp.height);
                            if (xCoord < 0 || yCoord < 0 || xCoord > texTemp.width || yCoord > texTemp.height)
                                c += Color.black;
                            else
                                c += texTemp.GetPixel(xCoord, yCoord) * w[j] * wSumRec;
                            tex.SetPixel(x, y, c);
                        }
                    }
                }
            }
        }
        //Fixing texture triangle borders
        Texture2D fixedTex = new Texture2D(tex.width, tex.height);
        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                Color newColor = tex.GetPixel(x, y);
                
                if (newColor == Color.magenta)
                {
                    Color ul = tex.GetPixel(x - 1, y - 1);
                    Color u = tex.GetPixel(x, y - 1);
                    Color ur = tex.GetPixel(x + 1, y - 1);
                    Color l = tex.GetPixel(x - 1, y);
                    Color r = tex.GetPixel(x + 1, y);
                    Color dl = tex.GetPixel(x - 1, y + 1);
                    Color d = tex.GetPixel(x, y + 1);
                    Color dr = tex.GetPixel(x + 1, y + 1);

                    int count = 0;
                    Color mixedColors = Color.black;
                    if(ul != Color.magenta)
                    {
                        mixedColors += ul;
                        count++;
                    }
                    if (u != Color.magenta)
                    {
                        mixedColors += u;
                        count++;
                    }
                    if (ur != Color.magenta)
                    {
                        mixedColors += ur;
                        count++;
                    }
                    if (l != Color.magenta)
                    {
                        mixedColors += l;
                        count++;
                    }
                    if (r != Color.magenta)
                    {
                        mixedColors += r;
                        count++;
                    }
                    if (dl != Color.magenta)
                    {
                        mixedColors += dl;
                        count++;
                    }
                    if (d != Color.magenta)
                    {
                        mixedColors += d;
                        count++;
                    }
                    if (dr != Color.magenta)
                    {
                        mixedColors += dr;
                        count++;
                    }
                    if( count!= 0)
                    {
                        mixedColors /= count;
                    }
                    if(mixedColors == Color.black)
                    {
                        mixedColors = Color.magenta;
                    }
                    newColor = mixedColors;
                }
                fixedTex.SetPixel(x, y, newColor);
            }
        }
        fixedTex.Apply();
        target.GetComponentInChildren<Renderer>().material.mainTexture = fixedTex;
    }

    public void Hide()
    {
        GameObject.Find("Name").GetComponent<Text>().text = "";
        SaveHolder.SetActive(false);
    }

    public void MainMenu()
    {
        GameManager.i.setArray(new List<string>());
        GameManager.i.setModel("");
        GameManager.i.setTexture("");
        GameManager.i.setBoth(false);
        GameManager.i.setType("");
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
            case 2:
                BakeTexture();
                break;
        }
        target.value = 3;
    }

    public static string TextureName(string name)
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        return string.Format(path+"/Oblozitelj/{1}_{2}.png", Application.dataPath, name, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm"));
    }

}

   
