using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataHandling;
using System;
using UnityEngine.UI;

public class NodePhysX : Node {

    public Text linkCount;
    public Text geneCount;

    private readonly bool verbose = true;

    private Rigidbody   thisRigidbody;
    public  GameObject  LinkPrefab;
    private DataTypes   _originType;

    private float sphRadius;
    private float sphRadiusSqr;

    Color lowColor      = Color.red;
    Color midColor      = Color.white;
    Color highColor     = Color.green;
    Color geneColor     = Color.magenta;
    Color addedColor    = Color.cyan;

    bool _hasLink;
    bool _isCentroid = true;
    bool _firstRun   = true;
    
    double[][]  _minMaxAttributes;
    double      _colorgradient;
    double      _last = 0;

    int  _expressionAttribute;
    int  _centroidIndex;
    int  _focus      = -1;
    double _lastTime = 0;

    Dictionary<string, List<string[]>>   _associations;
    List<DataTypes>[]                    _columnDataTypes;
    Cluster                              _DataCluster;
    Centroid                             _myCentroid;
    GameObject                           _linkToCentroid;
    List<GameObject>                     _linksToGenes;
    NodeSettingsHandler                  _nodeHandler;
    List<GameObject>                     _linksToAssociations;
    List<string>                         _attributeNames;
    string                               _id;
    string[]                             _recordValues;
    int                                  _geneAssociatedRecords = 0;

    public Cluster DataCluster
    {
        get
        {
            return _DataCluster;
        }

        set
        {
            _DataCluster = value;
        }
    }
    public bool HasLink
    {
        get
        {
            return _hasLink;
        }

        set
        {
            _hasLink = value;
        }
    }
    public Centroid MyCentroid
    {
        get
        {
            return _myCentroid;
        }

        set
        {
            _myCentroid = value;
        }
    }
    public GameObject LinkToCentroid
    {
        get
        {
            return _linkToCentroid;
        }

        set
        {
            _linkToCentroid = value;
        }
    }
    public string ID
    {
        get
        {
            return _id.ToLower();
        }

        set
        {
            _id = value.ToLower();
        }
    }
    public List<GameObject> LinksToAssociations
    {
        get
        {
            return _linksToAssociations;
        }

        set
        {
            _linksToAssociations = value;
        }
    }
    public List<GameObject> LinksToGenes
    {
        get
        {
            return _linksToGenes;
        }

        set
        {
            _linksToGenes = value;
        }
    }
    public DataTypes OriginType
    {
        get
        {
            return _originType;
        }

        set
        {
            _originType = value;
        }
    }
    public string[] RecordValues
    {
        get
        {
            return _recordValues;
        }

        set
        {
            _recordValues = value;
        }
    }

    public int GeneAssociatedRecords
    {
        get
        {
            return _geneAssociatedRecords;
        }

        set
        {
            _geneAssociatedRecords = value;
        }
    }

    public void DestroyCentroidLinks()
    {
        if (LinkToCentroid != null)
            Destroy(LinkToCentroid);
    }

    public void DestroyAssociationLinks()
    {
        foreach (GameObject link in LinksToAssociations)
            if (link != null)
                Destroy(link);
        LinksToAssociations = new List<GameObject>();
    }

    public void setDataPoint(DataTypes origintype, double[][] minMax, string[] recordVals, List<string> attributeNames, List<DataTypes>[] datatypes, Cluster cluster, NodeSettingsHandler nodehandler)
    {
        OriginType         = origintype;
        RecordValues       = recordVals;
        _attributeNames     = attributeNames;
        _columnDataTypes    = datatypes;
        DataCluster         = cluster;
        _minMaxAttributes   = minMax;
        HasLink             = false;
        _colorgradient      = 0.0d;
        _isCentroid         = false;
        LinksToAssociations = new List<GameObject>();
        LinksToGenes        = new List<GameObject>();
        _associations       = nodehandler.Persistantdata.Associations;
        _nodeHandler        = nodehandler;

        //Debug.Log("Datatypes length " + _columnDataTypes.Length);
        for(int i =0; i < _columnDataTypes.Length; i++)
        {
            Debug.Log(_columnDataTypes[i][0].ToString());
            if (_columnDataTypes[i].Contains(DataTypes.ID))
            {
                ID = RecordValues[i];
                break;
            }
        }

        if (origintype.Equals(DataTypes.ADDED_DATA))
            GetComponent<Renderer>().material.color = addedColor;

        if (origintype.Equals(DataTypes.FILE_DATA))
            GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Diffuse");
    }

    protected override void doGravity()
    {
        // Apply global gravity pulling node towards center of universe
        Vector3 dirToCenter = - this.transform.position;
        Vector3 impulse = dirToCenter.normalized * thisRigidbody.mass * graphControl.GlobalGravityPhysX;
        thisRigidbody.AddForce(impulse);
    }

    protected override void doRepulse()
    {
        // test which node in within forceSphere.
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, sphRadius);

        // only apply force to nodes within forceSphere, with Falloff towards the boundary of the Sphere and no force if outside Sphere.
        foreach (Collider hitCollider in hitColliders)
        {
            Rigidbody hitRb = hitCollider.attachedRigidbody;

            if (hitRb != null && hitRb != thisRigidbody)
            {
                Vector3 direction = hitCollider.transform.position - this.transform.position;
                float distSqr = direction.sqrMagnitude;

                // Normalize the distance from forceSphere Center to node into 0..1
                float impulseExpoFalloffByDist = Mathf.Clamp( 1 - (distSqr / sphRadiusSqr), 0, 1);

                // apply normalized distance
                hitRb.AddForce(direction.normalized * graphControl.RepulseForceStrength * impulseExpoFalloffByDist);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        thisRigidbody = this.GetComponent<Rigidbody>();
    }
    private void Awake()
    {
        linkCount = GameObject.Find("LinksCountText").GetComponent<Text>();
        geneCount = GameObject.Find("GenesCountText").GetComponent<Text>();

    }
    void Update()
    {
        // updating variable here, as opposed to doing it in Start(), otherwise we won't see runtime updates of forceSphereRadius
        sphRadius = graphControl.NodePhysXForceSphereRadius;
        sphRadiusSqr = sphRadius * sphRadius;

        if (_isCentroid)
            return;


        // if this is a datapoint (not gene or centroid)
        if (OriginType.Equals(DataTypes.FILE_DATA) || OriginType.Equals(DataTypes.ADDED_DATA) && OriginType != DataTypes.GENE)
        {
            HandleDataPointOperations();

            // if links are active and need to be added/checked (new datapoints added or initial check)
            if (_nodeHandler.PPLinksActive && _nodeHandler.PPLinksUpdate) { 
                UpdateDataDataAssociations();
                _nodeHandler.DataPointReportLinksUpdated();
            }

            // if linksactive is turned off, destroy any links to this point that may exist
            if (!_nodeHandler.PPLinksActive && LinksToAssociations.Count > 0)
            {
                Debug.Log("Destroying links");
                DestroyAssociationLinks();
            }
            if(_nodeHandler.PGLinksActive && _nodeHandler.PGLinksUpdate)
            {
                UpdateDataGeneAssociations();
                _nodeHandler.DataPointReportGeneLinksUpdated();
            }
            if(!_nodeHandler.PGLinksActive && LinksToGenes.Count > 0)
            {
                DestroyGeneLinks();
            }
        }
    }

    public void DestroyGeneLinks()
    {
        foreach(GameObject geneLink in LinksToGenes)
        {
            Link gLink = geneLink.GetComponent<Link>();
            NodePhysX gLinkTargetGene = gLink.target.GetComponent<NodePhysX>();
           // Debug.Log("Destroying gene link " + ID);
           // Debug.Log("Target gene count " + gLinkTargetGene.GeneAssociatedRecords.ToString());
            gLinkTargetGene.GeneAssociatedRecords--;
            if (gLinkTargetGene.GeneAssociatedRecords == 0)
            {
             //   Debug.Log("Destroying gene " + gLinkTargetGene.ID);
                Destroy(gLinkTargetGene.gameObject);
                _nodeHandler.GenePointsCoordinator.Records.Remove(gLinkTargetGene.gameObject);
                geneCount.text = (int.Parse(geneCount.text) - 1).ToString();
            }
                
            Destroy(geneLink);
            linkCount.text = (int.Parse(linkCount.text) - 1).ToString();
        }
        LinksToGenes = new List<GameObject>();
    }

    public void UpdateDataGeneAssociations()
    {
        if(ID != null && _associations != null)
        {
            List<string[]> matches;

            // see if associations exist for this datapoint
            if (_associations.TryGetValue(ID, out matches))
            {
                bool foundAssociatedGene = false;
                List<string> foundGeneIds = new List<string>();
                // check all gene records to see if exists already
                foreach(GameObject geneRecordObject in _nodeHandler.GenePointsCoordinator.Records)
                {
                    NodePhysX genePoint = geneRecordObject.GetComponent<NodePhysX>();
               //     Debug.Log("Existing gene id " + genePoint.ID);

                    // check if check gene is in this association's matches
                    foreach (string[] match in matches)
                    {
                        // if current check gene is a match to association
                        if (genePoint.ID.ToLower() == match[0].ToLower())
                        {
                            foundGeneIds.Add(genePoint.ID.ToLower());
                            foundAssociatedGene = true;
                            bool exists = false;
                            // check if link already exists in this point
                            foreach(GameObject geneLink in LinksToGenes)
                            {
                                Link assLink = geneLink.GetComponent<Link>();
                                if (assLink.source == gameObject && assLink.target == geneRecordObject)
                                    exists = true;
                                if (assLink.target == gameObject && assLink.source == geneRecordObject)
                                    exists = true;
                            }
                            if (!exists)
                            {
                                GameObject newLink = Instantiate(LinkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                                Link linkScr = newLink.GetComponent<Link>();
                                linkScr.source = gameObject;
                                linkScr.target = geneRecordObject;
                                linkScr.lineColor = Color.green;
                                LinksToGenes.Add(newLink);
                                genePoint.GeneAssociatedRecords++;
                                linkCount.text = (int.Parse(linkCount.text) + 1).ToString();
                                break;
                            }
                        }
                    }
                }
                if (!foundAssociatedGene)
                {   

                    foreach(string[] match in matches)
                    {
                        if (!foundGeneIds.Contains(match[0].ToLower()) && match[1] == "pd")
                        {
                            GameObject newGeneObject = graphControl.GenerateNode();
                            NodePhysX newGeneScript = newGeneObject.GetComponent<NodePhysX>();
                            newGeneScript.OriginType = DataTypes.GENE;
                            GameObject newLink = Instantiate(LinkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            Link linkScr = newLink.GetComponent<Link>();

                            linkScr.source = gameObject;
                            linkScr.target = newGeneObject;
                            linkScr.lineColor = Color.green;
                            LinksToGenes.Add(newLink);
                            newGeneScript.LinksToGenes = new List<GameObject>();
                            newGeneScript._originType = DataTypes.GENE;
                            newGeneScript._associations = _associations;
                            newGeneScript._nodeHandler = _nodeHandler;
                            newGeneScript.ID = match[0].ToLower();
                            newGeneScript.GeneFindAssociations();
                            newGeneScript.GeneAssociatedRecords++;
                            _nodeHandler.GenePointsCoordinator.Records.Add(newGeneObject);
                           // Debug.Log("Count genepoints: " + _nodeHandler.GenePointsCoordinator.Count);
                            linkCount.text = (int.Parse(linkCount.text) + 1).ToString();
                            geneCount.text = (int.Parse(geneCount.text) + 1).ToString();
                            //Debug.Log(ID + " creating new gene and link to " + match[0].ToLower());
                        }
                    }
                }
            }
        }
    }

    public void GeneFindAssociations()
    {
     //   Debug.Log("Finding gene to record associations " + ID + " " + _associations.Count);
        if(ID != null && _associations != null)
        {
            // create new list (also prevents repeat search)
            List<string[]> matches;

            // if associations has a match for this record, search all other datapoints
            if(_associations.TryGetValue(ID.ToLower(), out matches))
            {
           //     Debug.Log("Found match in associations");
                // check each record for a match
                foreach(GameObject record in _nodeHandler.DataPointsCoordinator.Records)
                {
                    NodePhysX dataPoint = record.GetComponent<NodePhysX>();
                    // check all found associations to this point for match to checked record
                    foreach(string[] match in matches)
                    {
                        // if current check record is a match to association
                        if (match[1].ToLower() == "pd" && match[0].ToLower() == dataPoint.ID.ToLower())
                        {
                            Debug.Log("Found gene to record match " + match[0].ToLower() + " : " + dataPoint.ID.ToLower());

                             // make sure link doesn't alread exist
                            bool exists = false;
                            foreach(GameObject linkToGene in dataPoint.LinksToGenes)
                            {
                                Link assLink = linkToGene.GetComponent<Link>();
                                if (assLink.source == gameObject && assLink.target == record)
                                    exists = true;
                                if (assLink.target == gameObject && assLink.source == record)
                                    exists = true;
                            }

                            // if it doesn't exist, make a new one
                            if (!exists)
                            {
                                GameObject newLink = Instantiate(LinkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                                Link linkScr = newLink.GetComponent<Link>();
                                linkScr.source = record;
                                linkScr.target = gameObject;
                                linkScr.lineColor = Color.green;
                                dataPoint.LinksToGenes.Add(newLink);
                                linkCount.text = (int.Parse(linkCount.text) + 1).ToString();
                                GeneAssociatedRecords++;
                                break;
                            }   
                        }
                    }
                }
            }
                    
        }
    }

    public void UpdateDataDataAssociations()
    {
        // check to see if associations need to be found
        if (ID != null && _associations != null)
        {
            // create new list (also prevents repeat search)
            List<string[]> matches;

            // if associations has a match for this record, search all other datapoints
            if(_associations.TryGetValue(ID, out matches))
            {
                // check each record for a match
                foreach(GameObject record in _nodeHandler.DataPointsCoordinator.Records)
                {
                    NodePhysX dataPoint = record.GetComponent<NodePhysX>();
                    // check all found associations to this point for match to checked record
                    foreach(string[] match in matches)
                    {
                        // if current check record is a match to association
                        if (match[0].ToLower() == dataPoint.ID.ToLower())
                        {
                             // make sure link doesn't alread exist
                            bool exists = false;
                            foreach(GameObject linkToAssociation in LinksToAssociations)
                            {
                                Link assLink = linkToAssociation.GetComponent<Link>();
                                if (assLink.source == gameObject && assLink.target == record)
                                    exists = true;
                                if (assLink.target == gameObject && assLink.source == record)
                                    exists = true;
                            }

                            // if it doesn't exist, make a new one
                            if (!exists)
                            {
                                GameObject newLink = Instantiate(LinkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                                Link linkScr = newLink.GetComponent<Link>();
                                linkScr.source = gameObject;
                                linkScr.target = record;
                                linkScr.lineColor = Color.red;
                                LinksToAssociations.Add(newLink);
                                linkCount.text = (int.Parse(linkCount.text) + 1).ToString();
                               

                                bool targetHasLink = false;

                                // check if target has this link already add if not
                                foreach (GameObject linkToAssociation in dataPoint.LinksToAssociations)
                                {
                                    Link assLink = linkToAssociation.GetComponent<Link>();
                                    if (assLink.source == gameObject && assLink.target == record)
                                        targetHasLink = true;
                                    if (assLink.target == gameObject && assLink.source == record)
                                        targetHasLink = true;
                                }
                                if (!targetHasLink)
                                    dataPoint.LinksToAssociations.Add(newLink);
                                break;
                            }   
                        }
                    }
                }
            }            
        }
       
    }

    void HandleDataPointOperations()
    {
        double timecheck = Time.time - _lastTime;
       

        if (_isCentroid)
            return;
        bool isClusteringOn = DataCluster.ClusteringOn;
        if (!isClusteringOn)
        {
            DestroyCentroidLinks();
        }


        
        if ((OriginType == DataTypes.FILE_DATA || OriginType == DataTypes.ADDED_DATA) && _DataCluster.ClusteringOn)
        {
          //  Debug.Log("Clustering on " + DataCluster.ClusteringOn + " canmove " + DataCluster.CanDatapointMove() + " timecheck " + timecheck);

            //Debug.Log("Finding closest cluster");
            if (timecheck > 3.5)
            {
                _lastTime = Time.time;
            //    Debug.Log("Canmove " + DataCluster.CanDatapointMove() + " Time " + (Time.time - _lastTime));
                if (!_nodeHandler.ClusteringPaused && _DataCluster.CanDatapointMove())
                {
              //      Debug.Log("Clustering on " + DataCluster.ClusteringOn + " canmove " + DataCluster.CanDatapointMove());
                    FindClosestCluster();
                }
            }
        }
        _focus = DataCluster.FocusAttribute;
        // only change focus color on file data
        if (OriginType == DataTypes.FILE_DATA && timecheck > 2  && _focus != -1)
        {
            AdjustColorToGradient();
        }
    }

    void AdjustColorToGradient()
    {
        //Debug.Log("updating color");  
        double focusValue = double.Parse(RecordValues[_focus]);
        // set the color of this datapoint based on attribute focus
        if (focusValue == 0)
        {
            GetComponent<Renderer>().material.color = midColor;
        }
        else if (focusValue < 0)
        {
            double gradient = focusValue / _minMaxAttributes[_focus][0];
            GetComponent<Renderer>().material.color = Color.Lerp(midColor, lowColor, (float)gradient);
        }
        else
        {
            double gradient = focusValue / _minMaxAttributes[_focus][1];
            GetComponent<Renderer>().material.color = Color.Lerp(midColor, highColor, (float)gradient);
        }
    }
    public void FindClosestCluster()
    {
       // Debug.Log("Finding closest cluster size " + DataCluster.Centroids.Count);
        bool done = false;
        if (MyCentroid == null && DataCluster.Centroids.Count > 0)
        {
            foreach (Centroid centroid in DataCluster.Centroids)
            {
         //       Debug.Log("Centroid first run: " + centroid.FirstRun);
                if (centroid.FirstRun)
                {
           //         Debug.Log("First run");
                    centroid.SetNewCentroid(RecordValues);
                    //centroid.centroid = Values;  handles above
                    int i = 0;
                    //CentroidAccumulate(centroid);  handled above
                    MyCentroid = centroid;
                    centroid.FirstRun = false;
                    if (LinkToCentroid != null)
                    {
                        Destroy(LinkToCentroid);
                        Debug.Log("Destroying link");
                    }
                    LinkToCentroid = Instantiate(LinkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    Link LinkScr = LinkToCentroid.GetComponent<Link>();
                    LinkScr.source = MyCentroid.GetGameObject();
                    LinkScr.target = gameObject;
                    //Coordinator.RecordRecalculated(true); handles above
                    done = true;
                    break;
                }
            }
        }
        if (!done)
        {
            //Debug.Log("Not yet done");
            //Debug.Log("cluster count " + DataCluster.Centroids.Count);
        
            Centroid closestCentroid = DataCluster.Centroids[0];
            double closestDist = double.MaxValue;
            foreach (Centroid centroid in DataCluster.Centroids)
            {
                if (centroid.FirstRun)
                {
                    MyCentroid = null;
                    return;
                }
                double thisDist = DataCluster.EuclideanDistanceToCentroid(centroid.CentroidValues, RecordValues);
                if (thisDist < closestDist)
                {
                    closestCentroid = centroid;
                    closestDist = thisDist;
                }
            }
            if (!closestCentroid.Equals(MyCentroid))
            {
                MyCentroid = closestCentroid;
                if (LinkToCentroid != null)
                    Destroy(LinkToCentroid);

                LinkToCentroid = Instantiate(LinkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                Link LinkScr = LinkToCentroid.GetComponent<Link>();
                LinkScr.source = MyCentroid.GetGameObject();
                LinkScr.target = gameObject;
                MyCentroid.ReportIn(RecordValues, true);
                // CentroidAccumulate(MyCentroid);
                // Coordinator.RecordRecalculated(true);
            }
            else
            {
                MyCentroid.ReportIn(RecordValues, false);
               // CentroidAccumulate(MyCentroid);
                // Coordinator.RecordRecalculated(false);
            }

        }
    }
}