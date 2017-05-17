using Assets.Scripts;
using Assets.Scripts.DataHandling;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GameCtrlHelper))]
[RequireComponent(typeof(GraphController))]

public class GameController : MonoBehaviour {

    [SerializeField]
    private bool verbose = false;
    GraphController graphController;
    PersistantData persistentData;
    private readonly string VALSPATH = "C:\\Users\\Josh\\AppData\\LocalLow\\DefaultCompany\\BioAnalizeVR\\galExpData.vals";
    private readonly string ASSOCPATH = "C:\\Users\\Josh\\AppData\\LocalLow\\DefaultCompany\\BioAnalizeVR\\galFiltered.assoc";


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
        
        List<string[]> dataSet = FileHandler.LoadSet(VALSPATH, true);
        List<DataTypes>[] types = FileHandler.InferTypes(VALSPATH, true);
        List<string[]> associationSet = FileHandler.LoadSet(ASSOCPATH, false);
        
        
        for (int i = 0; i < 10; i++)
            graphController.GenerateNode();
        for (int i = 0; i < 5; i++)
            graphController.GenerateLink("random");
    }

    void Update()
    {
    }
}
