using Assets.Scripts;
using Assets.Scripts.DataHandling;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    GameObject          FilePanel;
    GameObject          FileOptionsPanel;
    GameObject          TypesListPanel;
    GameObject          MainMenu;
    GameObject typeBar;
    PersistantData      Datapersistent;
    List<GameObject>    _typeBarsList;
    string              _lastFileName;

    // Use this for initialization
    void Start()
    {
        FileOptionsPanel.SetActive(false);
        TypesListPanel.SetActive(false);
        MainMenu.SetActive(false);
        PopulateFilesList();
    }

    private void Awake()
    {
        FilePanel           = GameObject.Find("ListComponents");
        FileOptionsPanel    = GameObject.Find("OptionsPanel");
        Datapersistent      = GameObject.Find("PersistentData").GetComponent<PersistantData>();
        TypesListPanel      = GameObject.Find("TypesListPanel");
        MainMenu            = GameObject.Find("MainMenuPanel");
    }

    public void FileSelected(Text fileText)
    {
        string filename = fileText.text;
        _lastFileName = filename;
        Debug.Log(filename);
        bool possibleNames = FileHandler.InferHasNames(Application.persistentDataPath + "/" +  filename);

        FilePanel.SetActive(false);
        FileOptionsPanel.SetActive(true);
        Text optionsHeader = GameObject.Find("OptionsHeader").GetComponent<Text>();
        optionsHeader.text = filename;
        if (!possibleNames)
            GameObject.Find("HasNamesToggle").GetComponent<Toggle>().isOn = false;
    }

    bool _lastFileWasData;
    public void LoadFileOptionsComplete()
    {
        Toggle hasNamesToggle = GameObject.Find("HasNamesToggle").GetComponent<Toggle>();
        Toggle isTestData = GameObject.Find("TestDataToggle").GetComponent<Toggle>();
        string filename = GameObject.Find("OptionsHeader").GetComponent<Text>().text;

        _lastFileWasData = isTestData.isOn;

        // Debug.Log("toggles and filename found: " + filename);
        List<string> names = FileHandler.LoadNames(Application.persistentDataPath + "/" + filename, hasNamesToggle.isOn);       
        
        if (isTestData.isOn)
        {
            Datapersistent.RecordValues = FileHandler.LoadSet(Application.persistentDataPath + "/" + filename, hasNamesToggle.isOn);
            Datapersistent.DatasetNames = names;
            Datapersistent.Types = FileHandler.InferTypes(Application.persistentDataPath + "/" + filename, hasNamesToggle.isOn);
            Datapersistent.DatasetFilename = filename;
            Datapersistent.HasDatafile = true;
            Datapersistent.MinMaxData = FileHandler.GetMinMax(Application.persistentDataPath + "/" + filename, Datapersistent.Types, hasNamesToggle.isOn);
            Text fileNameText = GameObject.Find("DataFileNameText").GetComponent<Text>();
            Text recordCount = GameObject.Find("DataRecordCount").GetComponent<Text>();
            fileNameText.text = "Data File: " + _lastFileName;
            fileNameText.color = Color.black;
            recordCount.text = "Records: " + Datapersistent.RecordValues.Count;
            recordCount.color = Color.black;        
            FileOptionsPanel.SetActive(false);
            TypesSelectionSetup();
        }
        else
        {
            Datapersistent.Associations = FileHandler.LoadAssociations(Application.persistentDataPath + "/" + filename, hasNamesToggle.isOn);
            Datapersistent.AssociationsFilename = filename;
            Text fileNameText = GameObject.Find("AssociationFileNameText").GetComponent<Text>();
            Text recordCount = GameObject.Find("AssociationRecordCount").GetComponent<Text>();
            fileNameText.text = "Association File: " + _lastFileName;
            fileNameText.color = Color.black;
            recordCount.text = "Associations: " + Datapersistent.Associations.Count;
            recordCount.color = Color.black;
            Datapersistent.AssociationNames = names;
            Datapersistent.HasAssociations = true;
            FileOptionsPanel.SetActive(false);
            MainMenu.SetActive(true);
        }   
    }

    public void LoadReloadFilePressed()
    {
        MainMenu.SetActive(false);
        FilePanel.SetActive(true);
    }

    public void AnalyzeDataPressed()
    {
        Application.LoadLevel("NodesScene");
    }

    // when done button is pressed on types options menu
    public void TypesSelectionComplete()
    {
        for(int i = 0; i < _typeBarsList.Count; i++)
        {
            TypeMenuObjects togglesHolder = _typeBarsList[i].GetComponent<TypeMenuObjects>();
            if(togglesHolder.IDtoggle.isOn)
                Datapersistent.Types[i].Add(DataTypes.ID);
            if (togglesHolder.IgnoreToggle.isOn)
                Datapersistent.Types[i].Add(DataTypes.IGNORE);
        }
        TypesListPanel.SetActive(false);
        MainMenu.SetActive(true);
    }

    // Handles the type selection menu after file is loaded
    public void TypesSelectionSetup() 
    {
        TypesListPanel.SetActive(true);
        if(!typeBar)
            typeBar  = GameObject.Find("TypeBar");

        GameObject parent   = GameObject.Find("TypesHolderPanel");
        int         count   = 0;

        if (_typeBarsList != null)
        {
            foreach (GameObject go in _typeBarsList)
                Destroy(go);
        }
        _typeBarsList = new List<GameObject>();

        typeBar.SetActive(false);

        foreach(string name in Datapersistent.DatasetNames)
        {
            GameObject      newTypeBar      = Instantiate(typeBar);
            TypeMenuObjects objectsHolder   = newTypeBar.GetComponent<TypeMenuObjects>();
            Text            numericType     = objectsHolder.NumericTypeText.GetComponent<Text>();
            Text            attributeName   = objectsHolder.AttributeNameText.GetComponent<Text>();

            newTypeBar.transform.SetParent(parent.transform);            
            newTypeBar.transform.rotation = parent.transform.rotation;
            newTypeBar.transform.localScale = parent.transform.localScale;
            newTypeBar.transform.localPosition = parent.transform.localPosition;

            attributeName.text = name;

            if (Datapersistent.Types[count][0].Equals(DataTypes.NUMERIC))
                numericType.text = "Numeric Type";

            newTypeBar.SetActive(true);
            _typeBarsList.Add(newTypeBar);
            count++;
        }
        _typeBarsList[0].GetComponentInChildren<Toggle>().isOn = true;
    }

    public void AssociationDataClicked(Toggle clicker)
    {
        Toggle namesToggle = GameObject.Find("HasNamesToggle").GetComponent<Toggle>();
        if (clicker.isOn)
            namesToggle.isOn = false;
        else
            namesToggle.isOn = true;
    }

    public void PopulateFilesList()
    {
       // Debug.Log("populating files list");

        List<string> files = new List<string>();
        List<GameObject> filePathObjects = new List<GameObject>();
       
        GameObject newPath;
        GameObject parentButton = GameObject.Find("FileListContent");
        parentButton.SetActive(false);

        GameObject listFrame = GameObject.Find("FileContentHolder");
        Text uiText = GameObject.Find("FilePathText").GetComponent<Text>(); // get filepath text object

        string datapath = Application.persistentDataPath;  // get the application datapath
        // Debug.Log(datapath);
        uiText.text = datapath; // set the path text

        foreach (string file in System.IO.Directory.GetFiles(datapath))
        {
           // Debug.Log(file);
            newPath = Instantiate(parentButton); // create new butotn
            newPath.transform.SetParent(listFrame.transform); // attach the button to the frame
            newPath.transform.rotation = parentButton.transform.rotation; // set rotation 
            newPath.transform.localScale = parentButton.transform.localScale; // set scale
            newPath.transform.localPosition = new Vector3(newPath.transform.localPosition.x, newPath.transform.localPosition.y, 0);

            string textout = Path.GetFileName(file);
           // Debug.Log(textout);
            newPath.GetComponentInChildren<Text>().text = textout.ToString();
            newPath.SetActive(true);
        }
    }
}