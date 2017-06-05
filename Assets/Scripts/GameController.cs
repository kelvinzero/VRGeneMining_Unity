using Assets.Scripts;
using Assets.Scripts.DataHandling;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameCtrlHelper))]
[RequireComponent(typeof(GraphController))]

public class GameController : MonoBehaviour {

    [SerializeField]
    GraphController     graphController;
    PersistantData      persistentData;
    NodeSettingsHandler Nodehandler;
    bool                verbose = false;    

    // use BulletUnity or PhysX?
    private bool engineBulletUnity = false;
    
    public bool EngineBulletUnity
    {
        get
        {
            return engineBulletUnity;
        }
        private set
        {
            engineBulletUnity = value;
        }
    }
    private void Update()
    {
        
    }
    void Start()
    {
        graphController = GetComponent<GraphController>();
        Nodehandler = GetComponent<NodeSettingsHandler>();

        persistentData = GameObject.Find("PersistentData").GetComponent<PersistantData>();
        if (persistentData.HasDatafile)
        {
            GameObject.Find("DataFileText").GetComponent<Text>().text = persistentData.DatasetFilename;
            GameObject.Find("AttributeCountText").GetComponent<Text>().text = persistentData.DatasetNames.Count.ToString();
            GameObject.Find("DataRecordCountText").GetComponent<Text>().text = persistentData.RecordValues.Count.ToString();
        }
        if (persistentData.HasAssociations)
        {
            GameObject.Find("AssociationFileText").GetComponent<Text>().text = persistentData.AssociationsFilename;
            GameObject.Find("AssociationAttributeCountText").GetComponent<Text>().text = persistentData.AssociationNames.Count.ToString();
            GameObject.Find("AssociationsCountText").GetComponent<Text>().text = persistentData.Associations.Count.ToString();
        }
        Nodehandler.Persistantdata = persistentData;
        Nodehandler.InitializeDataSettings();
        Debug.Log("NodeHandler activated");

    }
}
