using Assets.Scripts;
using Assets.Scripts.DataHandling;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameCtrlHelper))]
[RequireComponent(typeof(GraphController))]

public class GameController : MonoBehaviour {

    [SerializeField]
    private bool verbose = false;
    GraphController graphController;
    PersistantData persistentData;


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

    void Start()
    {
        graphController = GetComponent<GraphController>();
        persistentData = GameObject.Find("PersistentData").GetComponent<PersistantData>();
        if (persistentData.RecordValues != null)
        {
            GameObject.Find("DataFileText").GetComponent<Text>().text = persistentData.DatasetFilename;
            GameObject.Find("AttributeCountText").GetComponent<Text>().text = persistentData.DatasetNames.Count.ToString();
            GameObject.Find("DataRecordCountText").GetComponent<Text>().text = persistentData.RecordValues.Count.ToString();
        }
        if(persistentData.Associations != null)
        {
            GameObject.Find("AssociationFileText").GetComponent<Text>().text = persistentData.AssociationsFilename;
            GameObject.Find("AssociationAttributeCountText").GetComponent<Text>().text = persistentData.AssociationNames.Count.ToString();
            GameObject.Find("AssociationCountText").GetComponent<Text>().text = persistentData.AssociationNames.Count.ToString();
        }
        /*
        for (int i = 0; i < 10; i++)
            graphController.GenerateNode();
        for (int i = 0; i < 5; i++)
            graphController.GenerateLink("random");
            */
    }

    void Update()
    {
    }
}
