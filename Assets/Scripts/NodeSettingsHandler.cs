 using Assets.Scripts;
using Assets.Scripts.DataHandling;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// data cluster resets on every toggle
// 

public class NodeSettingsHandler  : MonoBehaviour
{

    private readonly bool verbose = true;

    public SimulateDatapointMenuHandler SimulateDataHandler;
    public GraphController  Graphcontroller;
    public PersistantData   Persistantdata;
    public GameObject       OnScreenWorldMenu;
    public Toggle           ClusterDataToggle;
    public Toggle           IncludeDependentAssociationsToggle;
    public Toggle           IncludeIndependentAssociationsToggle;
    public Toggle           IncludeFileDatasetToggle;
    public Toggle           IncludeDatapointsToggle;
    public Toggle           IncludeAAGeneGeneToggle;
    public Toggle           IncludeAAProteinGeneToggle;
    public Toggle           IncludeAAProteinProteinToggle;
    public Toggle           IncludeIAGeneGeneToggle;
    public Toggle           IncludeIAProteinGeneToggle;
    public Toggle           IncludeIAProteinProteinToggle;

    public Camera           MainViewCamera;
    public Text             MaxDatapointsText;
    public Text             MaxAssociationsText;

    public bool     RenderFileDataPoints        = false;
    public bool     RenderFileAssociationPints  = false;

    private bool    pPLinksActive               = false;
    private bool    pGLinksActive               = false;
    bool            pPLinksUpdate               = false;
    bool            pGLinksUpdate               = false;
    bool            clusteringPaused = false;

    public DataCoordinator GenePointsCoordinator;
    public DataCoordinator DataPointsCoordinator;
    public DataCoordinator AddedDataPointsCoordinator;

    GameObject      InGameSettingsPanel;
    Cluster         _dataCluster;
    List<Toggle>    _focusListObjects;
    int             MaxRenderGenes;
    int             MaxRenderDatapoints;
    Double[][]      MinMaxAttributes;
    bool            CheckForRenderDataUpdate;
    bool            CheckForRenderAssociationUpdate;
    bool            _clusteringOn;
    int             FocusNumber;
    int             RecordLinksUpdated = 0;
    int             RecordGeneLinksUpdated = 0;

    public bool PPLinksActive
    {
        get
        {
            return pPLinksActive;
        }

        set
        {
            pPLinksActive = value;
        }
    }
    public bool PPLinksUpdate
    {
        get
        {
            return pPLinksUpdate;
        }

        set
        {
            pPLinksUpdate = value;
        }
    }
    public bool PGLinksUpdate
    {
        get
        {
            return pGLinksUpdate;
        }

        set
        {
            pGLinksUpdate = value;
        }
    }
    public bool PGLinksActive
    {
        get
        {
            return pGLinksActive;
        }

        set
        {
            pGLinksActive = value;
        }
    }
    public bool ClusteringPaused
    {
        get
        {
            return clusteringPaused;
        }

        set
        {
            clusteringPaused = value;
        }
    }
    public Cluster DataCluster
    {
        get
        {
            return _dataCluster;
        }

        set
        {
            _dataCluster = value;
        }
    }
    public bool ClusteringOn
    {
        get
        {
            return _clusteringOn;
        }

        set
        {
            _clusteringOn = value;
        }
    }

    // DataPointReportLinksUpdated() - Report callback for datapoint checking links
    // Turned on when datapoints added or initially turned on
    // Turns off checking when all datapoints have updated once
    public void DataPointReportLinksUpdated()
    {
        RecordLinksUpdated++;
        if (RecordLinksUpdated == DataPointsCoordinator.Records.Count)
        {
            PPLinksUpdate = false;
            RecordLinksUpdated = 0;
        }
    }

    // DataPointReportLinksUpdated() - Report callback for datapoint checking links
    // Turned on when datapoints added or initially turned on
    // Turns off checking when all datapoints have updated once
    public void DataPointReportGeneLinksUpdated()
    {
        RecordGeneLinksUpdated++;
        if (RecordGeneLinksUpdated == DataPointsCoordinator.Records.Count)
        {
            PGLinksUpdate = false;
            RecordGeneLinksUpdated = 0;
        }
    }

    // Update() - Frame cycle update
    // Checks for render and association updates
    private void Update()
    {
        if (CheckForRenderDataUpdate)
        {
            UpdateRenderFileDataPoints();
            //UpdateRenderAllDataPoints();
        }
    }


    // Update file data points on toggle clicks.
    // This only updates records loaded from data file.
    void UpdateRenderFileDataPoints()
    {
        MinMaxAttributes = Persistantdata.MinMaxData;

        Debug.Log("Updating points in nodehandler");
        // if file data points rendering turned off, destroy all the points and links
        if (!RenderFileDataPoints && DataPointsCoordinator.Count > 0)
        {
            while (DataPointsCoordinator.DestroyLastDataPoint())
            {
                if (ClusteringOn) // if clustering, inform cluster
                    DataCluster.PointsTotal--;
            }
            CheckForRenderDataUpdate = false;
        }
        // if points to render is lower than points already rendered, destroy last point
        else if (RenderFileDataPoints && DataPointsCoordinator.Count > MaxRenderDatapoints)
        {
            DataPointsCoordinator.DestroyLastDataPoint();
            
            if (ClusteringOn) // if in cluster, inform cluster
                DataCluster.PointsTotal--;

            if (DataPointsCoordinator.Count <= MaxRenderDatapoints)
                CheckForRenderDataUpdate = false;
        }
        // if points rendered is below max, create a point
        else if (RenderFileDataPoints && DataPointsCoordinator.Count < MaxRenderDatapoints)
        {
            // if no more file records
            if (Persistantdata.RecordValues.Count <= DataPointsCoordinator.Count)
                CheckForRenderDataUpdate = false;

            // else add a new record
            else
            {
                GameObject newNode      = Graphcontroller.GenerateNode();
                NodePhysX nodeScript    = newNode.GetComponent<NodePhysX>();
                // Debug.Log("found script: " + nodeScript.name);

                // adds the next record from persistantdata loaded data file
                nodeScript.setDataPoint(DataTypes.FILE_DATA, MinMaxAttributes, Persistantdata.RecordValues[DataPointsCoordinator.Count], 
                    Persistantdata.DatasetNames, Persistantdata.Types, DataCluster, this);

                DataPointsCoordinator.Records.Add(newNode);
                
                if (ClusteringOn) // if in cluster, inform cluster
                    DataCluster.PointsTotal++;

               // Debug.Log("Rendered: " + DataPointsCoordinator.Count + " of " + RenderDatapointsCount);

                if (DataPointsCoordinator.Count >= MaxRenderDatapoints)
                    CheckForRenderDataUpdate = false;

                if (PPLinksActive)
                    nodeScript.UpdateDataDataAssociations();

                if (PGLinksActive)
                    nodeScript.UpdateDataGeneAssociations();
            }
        }
        // if no change needed, stop updating
        else
            CheckForRenderDataUpdate = false;
    }



    // Awake() - Sets initial values and initialized coordinators
    private void Awake()
    {
        MaxRenderDatapoints             = int.Parse(MaxDatapointsText.text);
        FocusNumber                     = -1;
        CheckForRenderAssociationUpdate = false;
        CheckForRenderDataUpdate        = false;
        ClusteringOn                    = false;
        GenePointsCoordinator           = new DataCoordinator();
        DataPointsCoordinator           = new DataCoordinator();
        AddedDataPointsCoordinator      = new DataCoordinator();
        DataCluster                     = new Cluster();
        Graphcontroller                 = GetComponent<GraphController>();
    }

    // DeactivateMainMenu() - Turn off the main menu panels
    public void DeactivateMainMenu()
    {
        OnScreenWorldMenu.SetActive(false);
    }

    // ActivateMainMenu() - Turn on the main menu panels
    public void ActivateMainMenu()
    {
        OnScreenWorldMenu.SetActive(true);
    }

    // InitializeDataSettings() - Initializes DataCoordinator objects.
    // Uses the persistant data loaded from file to build DataCoordinators.
    // Creates one for file data and one for file associations.
    // Sets toggles for file data and association data.
    public void InitializeDataSettings()
    {
        //TODO: Restrict camera movement so menu always near
        Debug.Log("Initializing data settings");

        // if has records file but no associations
        if (Persistantdata.HasDatafile && !Persistantdata.HasAssociations)
        {
            // disable fileassociations options
            
            Debug.Log("Initializing data settings");
            //initialize file datapoints coordinator
            DataPointsCoordinator.ColumnTypes = Persistantdata.Types;
            DataPointsCoordinator.OriginType = DataTypes.FILE_DATA;
            DataPointsCoordinator.ColumnNames = Persistantdata.DatasetNames;
            PopulateFocusAttributeList();
            SimulateDataHandler.NodeHandler = this;
            SimulateDataHandler.PopulateAttributeCreatorObjects();
         }
        // if has associations but no records
        else if (!Persistantdata.HasDatafile && Persistantdata.HasAssociations)
        {
            // turn off dataset options
            IncludeFileDatasetToggle.isOn = false;
            IncludeFileDatasetToggle.interactable = false;

            // initialize file association datacoordinator
            GenePointsCoordinator.ColumnTypes = Persistantdata.AssotiationTypes;
            GenePointsCoordinator.OriginType = DataTypes.FILE_ASSOCIATION;
            GenePointsCoordinator.ColumnNames = Persistantdata.AssociationNames;

        }
        else
        {
            PopulateFocusAttributeList();
            SimulateDataHandler.NodeHandler = this;
            SimulateDataHandler.PopulateAttributeCreatorObjects();
        }
    }

    // PopulateFocusAttributeList() - Fills the attribute focus selector menu
    // Adds attribute names of numeric values to the list
    // Instantiate and set position for each attribute
    public void PopulateFocusAttributeList()
    {
        Debug.Log("Populating list");
        _focusListObjects = new List<Toggle>();

        GameObject nameHolderPanel  = GameObject.Find("AttributeComponent");
        GameObject parent           = GameObject.Find("AttributeComponentHolder");
        int count = 0;

        nameHolderPanel.SetActive(false);

        for(int i = 0; i < Persistantdata.DatasetNames.Count; i++)
        {
            Debug.Log(Persistantdata.Types[i][0]);
            if (Persistantdata.Types[i].Contains(DataTypes.NUMERIC))
            {
                GameObject          newHolderPanel  = Instantiate(nameHolderPanel);
                Toggle              attributeToggle = newHolderPanel.GetComponentInChildren<Toggle>();
                Text                attributeName   = attributeToggle.GetComponentInChildren<Text>();
                AttributeToggleInfo toggleInfo      = attributeToggle.GetComponent<AttributeToggleInfo>();

                toggleInfo.AttributeNumber = i;
                newHolderPanel.transform.SetParent(parent.transform);
                newHolderPanel.transform.rotation = parent.transform.rotation;
                newHolderPanel.transform.localScale = parent.transform.localScale;
                newHolderPanel.transform.localPosition = parent.transform.localPosition;
                attributeName.text = Persistantdata.DatasetNames[i];
                newHolderPanel.SetActive(true);
                _focusListObjects.Add(newHolderPanel.GetComponentInChildren<Toggle>());

            }
        }
    }

    // FocusItemClicked(Toggle triggerToggle) - Callback for the focus attribute menu items
    // Sets the focus attribute to the checked toggle
    // Updates cluster and member variables for focus attribute
    public void FocusItemClicked(Toggle triggerToggle)
    {
        if (!triggerToggle.isOn)
        {
            DataCluster.FocusAttribute = -1;
            FocusNumber = -1;
        }
        int triggerAttributeNumber = triggerToggle.GetComponent<AttributeToggleInfo>().AttributeNumber;
        DataCluster.FocusAttribute = triggerAttributeNumber;
        FocusNumber                = triggerAttributeNumber;
        if (verbose)
            Debug.Log("Focus item clicked set to " + triggerAttributeNumber);
    }

    // IncludeFileDatapointsPressed() - Callback for include file datapoints toggle
    // Turns on/off datapoint rendering from file
    public void IncludeFileDatapointsPressed()
    {
        if (verbose)
            Debug.Log("Include file data pressed");
        RenderFileDataPoints = IncludeFileDatasetToggle.isOn;
        CheckForRenderDataUpdate = true;
        if (verbose)
            Debug.Log("Values changes");
    }

    // MaxDataCountArrowPressed() - Callback for arrow pressed max datapoints
    public void MaxDataCountArrowPressed()
    {
        MaxRenderDatapoints = int.Parse(MaxDatapointsText.text);
        CheckForRenderDataUpdate = true;
    }

    public void MaxAssociationCountPressed()
    {
        MaxRenderGenes = int.Parse(MaxAssociationsText.text);
        CheckForRenderDataUpdate = true;
    }

    public void ClusterButtonPressed()
    {
        if (verbose)
            Debug.Log("ClusterButtonPressed");

        Text clusterCount   = GameObject.Find("ClusterCountText").GetComponent<Text>();
        int totalRecords = DataPointsCoordinator.Count;

        if (ClusterDataToggle.isOn)
        {
            int clusterCountInt = int.Parse(clusterCount.text);
            DataCluster.SetParams(clusterCountInt, totalRecords, Graphcontroller, Persistantdata.Types);
            DataCluster.SetClusteringOn(ClusterDataToggle.isOn);
        }
        else
        {
            if (verbose)
                Debug.Log("Turning clustering off");
            DataCluster.SetClusteringOn(ClusterDataToggle.isOn);
        }       
        if (verbose)
            Debug.Log("Cluster init");
    }

    public void ClusterCountChanged(Text numberText)
    {
        int newCount = int.Parse(numberText.text);
        if (verbose)
            Debug.Log("Clsuter count changed to " + newCount );
        if (newCount > DataCluster.Centroids.Count)
            DataCluster.AddCentroid();
        else
            DataCluster.RemoveCentroid();
    }

    public void PGLinksToggleChanged(Toggle pgToggle)
    {
        PGLinksActive = pgToggle.isOn;
        PGLinksUpdate = pgToggle.isOn;
    }
    public void PPLinksToggleChanged(Toggle ppToggle)
    {
        PPLinksActive = ppToggle.isOn;
        PPLinksUpdate = ppToggle.isOn;
    }

    public void PauseClusteringPressed(Toggle clusterPauseToggle)
    {
        ClusteringPaused = clusterPauseToggle.isOn;
    } 
}
