﻿#define thread //comment out this line if you would like to disable multi-threaded search
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if thread
using System.Threading;
#endif



public class FileBrowser{
//public 
	//Optional Parameters
	public string name = "File Browser"; //Just a name to identify the file browser with
	//GUI Options
	public GUISkin guiSkin; //The GUISkin to use
	public int layoutType{	get{	return layout;	}	} //returns the current Layout type
	public Texture2D fileTexture,directoryTexture,backTexture,driveTexture; //textures used to represent file types
	public GUIStyle backStyle,cancelStyle,selectStyle; //styles used for specific buttons
	public Color selectedColor = new Color(0.0f,0.6f,0.0f); //the color of the selected file
	public bool isVisible{	get{	return visible;	}	} //check if the file browser is currently visible
	//File Options
	public string searchPattern = "*", path = ""; //search pattern used to find files
	//Output
	public string outputFile; //the selected output file
	//Search
	public bool showSearch = false; //show the search bar
	public bool searchRecursively = false; //search current folder and sub folders
//Protected	
	//GUI
	public Vector2 fileScroll=Vector2.zero,folderScroll=Vector2.zero,driveScroll=Vector2.zero;
	public Color defaultColor;
	public int layout;
	public Rect guiSize;
	public GUISkin oldSkin;
	public bool visible = false;
	//Search
	public string searchBarString = ""; //string used in search bar
	public bool isSearching = false; //do not show the search bar if searching
	//File Information
	public DirectoryInfo currentDirectory;
	public FileInformation[] files;
	public DirectoryInformation[] directories,drives;
	public DirectoryInformation parentDir;
	public bool getFiles = true,showDrives=false;
	public int selectedFile = -1;
    //Double clicking
    private bool one_click = false;
    private int which = -1;
    private float timer_for_double_click;
    private float delay = 0.7f;
    //Threading
    public float startSearchTime = 0f;
	#if thread
	public Thread t;
    private int thisOne = -1;
#endif

    //Constructors
    public FileBrowser(string directory,int layoutStyle,Rect guiRect){	currentDirectory = new DirectoryInfo(directory);	layout = layoutStyle;	guiSize = guiRect;	}
	#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8)
		public FileBrowser(string directory,int layoutStyle):this(directory,layoutStyle,new Rect(0,0,Screen.width,Screen.height)){}
		public FileBrowser(string directory):this(directory,1){}
	#else
		public FileBrowser(string directory,int layoutStyle):this(directory,layoutStyle,new Rect(Screen.width*0.125f,Screen.height*0.125f,Screen.width*0.75f,Screen.height*0.75f)){}
		public FileBrowser(string directory):this(directory,0){}
	#endif
	public FileBrowser(Rect guiRect):this(){	guiSize = guiRect;	}
	public FileBrowser(int layoutStyle):this(Directory.GetCurrentDirectory(),layoutStyle){}
	public FileBrowser():this(Directory.GetCurrentDirectory()){}
	
	//set variables
	public void setDirectory(string dir){	currentDirectory=new DirectoryInfo(dir);	}
	public void setLayout(int l){	layout=l;	}
	public void setGUIRect(Rect r){	guiSize=r;	}
	
	
	//gui function to be called during OnGUI
	public int draw(){
		if(getFiles){
			getFileList(currentDirectory); 
			getFiles=false;
		}
		if(guiSkin){
			oldSkin = GUI.skin;
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(guiSize);
		GUILayout.BeginVertical("box");
		switch(layout){
			case 0:
				GUILayout.BeginHorizontal("box");
					GUILayout.FlexibleSpace();
                path = currentDirectory.FullName;
                    GUILayout.Label(currentDirectory.FullName);
					GUILayout.FlexibleSpace();
					if(showSearch){
						drawSearchField();
						GUILayout.Space(10);
					}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box");
					GUILayout.BeginVertical(GUILayout.MaxWidth(300));
						folderScroll = GUILayout.BeginScrollView(folderScroll);
						if(showDrives){
							foreach(DirectoryInformation di in drives){
								if(di.button()){	getFileList(di.di);	}
							}
						}else{
							if((backStyle != null)?parentDir.button(backStyle):parentDir.button())
								getFileList(parentDir.di);
						}
						foreach(DirectoryInformation di in directories){
							if(di.button()){	getFileList(di.di);	}
						}
						GUILayout.EndScrollView();
					GUILayout.EndVertical();
					GUILayout.BeginVertical("box");
						if(isSearching){
							drawSearchMessage();
						}else{
							fileScroll = GUILayout.BeginScrollView(fileScroll);
							for(int fi=0;fi<files.Length;fi++){
								if(selectedFile==fi)
                                {
                                    defaultColor = GUI.color;
                                    if (thisOne == fi)
                                    {
                                        GUI.color = selectedColor;
                                    }
                                }
#if UNITY_EDITOR
                        if (files[fi].button())
                        {
                            outputFile = files[fi].fi.ToString();
                            selectedFile = fi;
                            if (!one_click) // first click no previous clicks
                            {
                                
                                thisOne = -1;
                                
                                one_click = true;
                                timer_for_double_click = Time.time; // save the current time
                                                                    // do one click things;
                                which = fi;
                            }
                            else
                            {
                                one_click = false; // found a double click, now reset
                                if ( which == fi && (Time.time - timer_for_double_click) < delay)
                                {
                                    thisOne = fi;
                                    which = -1;
                                    return 1;
                                }
                                else if (which != fi)
                                {
                                    thisOne = -1;
                                    which = fi;
                                    one_click = true;
                                    timer_for_double_click = Time.time;
                                }
                                
                            }
                        }
#else
                        if (files[fi].button())
                        {
#if UNITY_STANDALONE_OSX
                        
                        outputFile = path+"/"+files[fi].fi.ToString();
#else
                        outputFile = path+"\\"+files[fi].fi.ToString();
#endif
                            selectedFile = fi;
                            if (!one_click) // first click no previous clicks
                            {
                                
                                thisOne = -1;
                                
                                one_click = true;
                                timer_for_double_click = Time.time; // save the current time
                                                                    // do one click things;
                                which = fi;
                            }
                            else
                            {
                                one_click = false; // found a double click, now reset
                                if ( which == fi && (Time.time - timer_for_double_click) < delay)
                                {
                                    thisOne = fi;
                                    which = -1;
                                    return 1;
                                }
                                else if (which != fi)
                                {
                                    thisOne = -1;
                                    which = fi;
                                    one_click = true;
                                    timer_for_double_click = Time.time;
                                }
                                
                            }
                        }
#endif

                        if (selectedFile==fi)
									GUI.color = defaultColor;
							}
							GUILayout.EndScrollView();
						}
						GUILayout.BeginHorizontal("box");
						GUILayout.FlexibleSpace();
						if((cancelStyle == null)?GUILayout.Button("Odustani"):GUILayout.Button("Odustani",cancelStyle)){
                            SceneManager.LoadScene("LoadObject");
                            return 0;
						}
						GUILayout.FlexibleSpace();
                        if ((selectStyle == null) ? GUILayout.Button("Završi odabir") : GUILayout.Button("Završi odabir", selectStyle)) { return 2; }
                        
                        GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				break;
			case 1: //mobile preferred layout
			default:
				if(showSearch){
					GUILayout.BeginHorizontal("box");
						GUILayout.FlexibleSpace();
						drawSearchField();
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				fileScroll = GUILayout.BeginScrollView(fileScroll);
				
				if(isSearching){
					drawSearchMessage();
				}else{
					if(showDrives){
						GUILayout.BeginHorizontal();
						foreach(DirectoryInformation di in drives){
							if(di.button()){	getFileList(di.di);	}
						}
						GUILayout.EndHorizontal();
					}else{
						if((backStyle != null)?parentDir.button(backStyle):parentDir.button())
							getFileList(parentDir.di);
					}
					
					
					foreach(DirectoryInformation di in directories){
						if(di.button()){	getFileList(di.di);	}
					}
					for(int fi=0;fi<files.Length;fi++){
						if(selectedFile==fi){
							defaultColor = GUI.color;
							GUI.color = selectedColor;
						}
#if UNITY_EDITOR
                        if (files[fi].button())
                        {
                            outputFile = files[fi].fi.ToString();
                            selectedFile = fi;
                        }
#else
#if UNITY_STANDALONE_OSX
                        if (files[fi].button())
                        {
                            outputFile = path+"/"+files[fi].fi.ToString();
                            selectedFile = fi;
                        }
#else
                        if (files[fi].button())
                        {
                            outputFile = path+"\\"+files[fi].fi.ToString();
                            selectedFile = fi;
                        }
#endif
#endif
                        if (selectedFile==fi)
							GUI.color = defaultColor;
					}
				}
				GUILayout.EndScrollView();
				
				//if((selectStyle == null)?GUILayout.Button("Odaberi"):GUILayout.Button("Odaberi",selectStyle)){	return 1;	}
				if((cancelStyle == null)?GUILayout.Button("Odustani"):GUILayout.Button("Odustani",cancelStyle)){
                    SceneManager.LoadScene("LoadObject");
                    return 0;
				}
                if ((selectStyle == null) ? GUILayout.Button("Završi odabir") : GUILayout.Button("Završi odabir", selectStyle))
                {
                    return 2;
                }
                break;
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		if(guiSkin){GUI.skin = oldSkin;}
		return 0;
	}
	
	public void drawSearchField(){
		if(isSearching){
			GUILayout.Label("Searching For: \""+searchBarString+"\"");
		}else{
			searchBarString = GUILayout.TextField(searchBarString,GUILayout.MinWidth(150));
			if(GUILayout.Button("search")){
				if(searchBarString.Length > 0){
					isSearching = true;
#if thread
					startSearchTime = Time.time;
					t = new Thread(threadSearchFileList);
					t.Start(true);
#else
					searchFileList(currentDirectory);
#endif
				}else{
					getFileList(currentDirectory);
				}
			}
		}
	}
	
	public void drawSearchMessage(){
		float tt = Time.time-startSearchTime;
		if(tt>1)
			GUILayout.Button("Searching");
		if(tt>2)
			GUILayout.Button("For");
		if(tt>3)
			GUILayout.Button("\""+searchBarString+"\"");
		if(tt>4)
			GUILayout.Button(".....");
		if(tt>5)
			GUILayout.Button("It's");
		if(tt>6)
			GUILayout.Button("Taking");
		if(tt>7)
			GUILayout.Button("A");
		if(tt>8)
			GUILayout.Button("While");
		if(tt>9)
			GUILayout.Button(".....");
	}
	
	public void getFileList(DirectoryInfo di){
		//set current directory
		currentDirectory = di;
		//get parent
		if(backTexture)
			parentDir = (di.Parent==null)?new DirectoryInformation(di,backTexture):new DirectoryInformation(di.Parent,backTexture);
		else
			parentDir = (di.Parent==null)?new DirectoryInformation(di):new DirectoryInformation(di.Parent);
		showDrives = di.Parent==null;
		
		//get drives
		string[] drvs = System.IO.Directory.GetLogicalDrives();
		drives = new DirectoryInformation[drvs.Length];
		for(int v=0;v<drvs.Length;v++){
			drives[v]= (driveTexture==null)?new DirectoryInformation(new DirectoryInfo(drvs[v])):new DirectoryInformation(new DirectoryInfo(drvs[v]),driveTexture);
		}
		
		//get directories
		DirectoryInfo[] dia = di.GetDirectories();
		directories = new DirectoryInformation[dia.Length];
		for(int d=0;d<dia.Length;d++){
			if(directoryTexture)
				directories[d] = new DirectoryInformation(dia[d],directoryTexture);
			else
				directories[d] = new DirectoryInformation(dia[d]);
		}

        //get files
        FileInfo[] fia;
        if (GameManager.i.getType().Equals("Model"))
        {
            searchPattern = "*.obj";
            fia = di.GetFiles(searchPattern);
        }
        else
        {
            searchPattern = "*.jpg";
            List<FileInfo> backup = new List<FileInfo>(di.GetFiles(searchPattern));
            searchPattern = "*.png";
            FileInfo[] find = di.GetFiles(searchPattern);
            for (int i = 0; i < find.Length; i++)
            {
                backup.Add(find[i]);
            }
            searchPattern = "*.gif";
            find = di.GetFiles(searchPattern);
            for (int i = 0; i < find.Length; i++)
            {
                backup.Add(find[i]);
            }
            fia = backup.ToArray();
        }
		
		//FileInfo[] fia = searchDirectory(di,searchPattern);
		files = new FileInformation[fia.Length];
		for(int f=0;f<fia.Length;f++){
			if(fileTexture)
				files[f] = new FileInformation(fia[f],fileTexture);
			else
				files[f] = new FileInformation(fia[f]);
		}
	}
	
	
	public void searchFileList(DirectoryInfo di){
		searchFileList(di,fileTexture!=null);
	}
	
	public void searchFileList(DirectoryInfo di,bool hasTexture){
		//(searchBarString.IndexOf("*") >= 0)?searchBarString:"*"+searchBarString+"*"; //this allows for more intuitive searching for strings in file names
		FileInfo[] fia = di.GetFiles((searchBarString.IndexOf("*") >= 0)?searchBarString:"*"+searchBarString+"*",(searchRecursively)?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
		files = new FileInformation[fia.Length];
		for(int f=0;f<fia.Length;f++){
			if(hasTexture)
				files[f] = new FileInformation(fia[f],fileTexture);
			else
				files[f] = new FileInformation(fia[f]);
		}
#if thread
#else
		isSearching = false;
#endif
	}
	
	public void threadSearchFileList(object hasTexture){
		searchFileList(currentDirectory,(bool)hasTexture);
		isSearching = false;
	}
	
	//search a directory by a search pattern, this is optionally recursive
	public static FileInfo[] searchDirectory(DirectoryInfo di,string sp,bool recursive){
		return di.GetFiles(sp,(recursive)?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
	}
	public static FileInfo[] searchDirectory(DirectoryInfo di,string sp){
		return searchDirectory(di,sp,false);
	}
	
	public float brightness(Color c){	return	c.r*.3f+c.g*.59f+c.b*.11f;	}
	
	//to string
	public override string ToString(){
		return "Name: "+name+"\nVisible: "+isVisible.ToString()+"\nDirectory: "+currentDirectory+"\nLayout: "+layout.ToString()+"\nGUI Size: "+guiSize.ToString()+"\nDirectories: "+directories.Length.ToString()+"\nFiles: "+files.Length.ToString();
	}
}

public class FileInformation{
	public FileInfo fi;
	public GUIContent gc;
	
	public FileInformation(FileInfo f){
		fi=f;
		gc = new GUIContent(fi.Name);
	}
	
	public FileInformation(FileInfo f,Texture2D img){
		fi = f;
		gc = new GUIContent(fi.Name,img);
	}
	
	public bool button(){return GUILayout.Button(gc);}
	public void label(){	GUILayout.Label(gc);	}
	public bool button(GUIStyle gs){return GUILayout.Button(gc,gs);}
	public void label(GUIStyle gs){	GUILayout.Label(gc,gs);	}
}

public class DirectoryInformation{
	public DirectoryInfo di;
	public GUIContent gc;
	
	public DirectoryInformation(DirectoryInfo d){
		di=d;
		gc = new GUIContent(d.Name);
	}
	
	public DirectoryInformation(DirectoryInfo d,Texture2D img){
		di=d;
		gc = new GUIContent(d.Name,img);
	}
	
	public bool button(){return GUILayout.Button(gc);}
	public void label(){	GUILayout.Label(gc);	}
	public bool button(GUIStyle gs){return GUILayout.Button(gc,gs);}
	public void label(GUIStyle gs){	GUILayout.Label(gc,gs);	}
}