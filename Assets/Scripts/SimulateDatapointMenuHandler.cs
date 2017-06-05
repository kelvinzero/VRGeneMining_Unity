using Assets.Scripts.DataHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateDatapointMenuHandler : MonoBehaviour {

    public GameObject           AttributeCreatorHolder;
    public GameObject           AttributeCreatorInfo;
    public GameObject           AddedRecordItemHolder;
    public GameObject           AddedRecordItem;
    public NodeSettingsHandler  NodeHandler;
    List<CreateAttributeItemController> createdRecordAttributes;

    private void Start()
    {
        AttributeCreatorInfo.SetActive(false);
        AddedRecordItem.SetActive(false);
        createdRecordAttributes = new List<CreateAttributeItemController>();
    }
    public void RandomizeAllAttributeValues()
    {
        foreach(CreateAttributeItemController thisAttribute in createdRecordAttributes)
        {
            if (thisAttribute.AttributeType.Contains(DataTypes.NUMERIC))
                thisAttribute.RandomizeAttributeValuePressed();
        }
    }

    public void GeneratePointPressed()
    {
        string[] newRecord = new string[createdRecordAttributes.Count];
        for(int i = 0; i < createdRecordAttributes.Count; i++)
        {
            newRecord[i] = createdRecordAttributes[i].AttributeValue.text;
        }
        GameObject newNode = NodeHandler.Graphcontroller.GenerateNode();
        NodePhysX nodeScript = newNode.GetComponent<NodePhysX>();
        // Debug.Log("found script: " + nodeScript.name);

        // adds the next record from persistantdata loaded data file
        nodeScript.setDataPoint(DataTypes.ADDED_DATA, NodeHandler.Persistantdata.MinMaxData, newRecord,
            NodeHandler.Persistantdata.DatasetNames, NodeHandler.Persistantdata.Types, NodeHandler.DataCluster, NodeHandler);
        NodeHandler.AddedDataPointsCoordinator.OriginType = DataTypes.ADDED_DATA;
        NodeHandler.AddedDataPointsCoordinator.Records.Add(newNode);

        if (NodeHandler.ClusteringOn) // if in cluster, inform cluster
            NodeHandler.DataCluster.PointsTotal++;

        string newID = "Generated_" + (NodeHandler.AddedDataPointsCoordinator.Count + 1);
        foreach(CreateAttributeItemController controller in createdRecordAttributes)
        {
            if (controller.AttributeType.Contains(DataTypes.ID))
                controller.AttributeValue.text = newID;
        }
        RandomizeAllAttributeValues();
        AddSimulatedToList(newRecord, newNode);
    }

    public void DeleteSimulatedPoint(GeneratedDatapointsObjects deleteObjectInfo)
    {
        NodeHandler.AddedDataPointsCoordinator.DestroyDataPoint(deleteObjectInfo.MyRecordObject);
        Destroy(deleteObjectInfo.gameObject);
    }

    public void AddSimulatedToList(string[] record, GameObject newNode)
    {
        GameObject newRecordListItem = Instantiate(AddedRecordItem, AddedRecordItemHolder.transform);
        GeneratedDatapointsObjects genPointObjects = newRecordListItem.GetComponent<GeneratedDatapointsObjects>();
        genPointObjects.Record = record;
        genPointObjects.DataTypes = NodeHandler.Persistantdata.Types;
        genPointObjects.AddAttributesToScroller();
        genPointObjects.MyRecordObject = newNode;
        newRecordListItem.SetActive(true);
    }

    public void GetRandomDatapointValues()
    {
        int randomIndex = Random.Range(0, NodeHandler.Persistantdata.RecordValues.Count);

        string[] record = NodeHandler.Persistantdata.RecordValues[randomIndex];
        for (int i = 0; i < record.Length; i++)
        {
            if (!createdRecordAttributes[i].AttributeType.Contains(DataTypes.ID))
            {
                createdRecordAttributes[i].AttributeValue.text = record[i];
            }
        }
    }

    public void PopulateAttributeCreatorObjects()
    {
        for(int i = 0; i < NodeHandler.Persistantdata.DatasetNames.Count; i++)
        {
            GameObject newCreatorInfo = Instantiate(AttributeCreatorInfo);
            CreateAttributeItemController attributeObjectController = newCreatorInfo.GetComponent<CreateAttributeItemController>();
            createdRecordAttributes.Add(attributeObjectController);
            newCreatorInfo.transform.SetParent(AttributeCreatorHolder.transform);
            newCreatorInfo.transform.localPosition = AttributeCreatorHolder.transform.localPosition;
            newCreatorInfo.transform.localScale = AttributeCreatorHolder.transform.localScale;
            newCreatorInfo.transform.localRotation = AttributeCreatorHolder.transform.localRotation;
            attributeObjectController.AttributeName.text = NodeHandler.Persistantdata.DatasetNames[i];
            attributeObjectController.AttributeNumber = i;
            attributeObjectController.AttributeType = NodeHandler.Persistantdata.Types[i];
            attributeObjectController.AttributeType.Add(DataTypes.ADDED_DATA);

           

            if (NodeHandler.Persistantdata.Types[i].Contains(DataTypes.ID))
            {
                string newID = "Generated_" + (NodeHandler.AddedDataPointsCoordinator.Count+1);
                attributeObjectController.AttributeValue.text = newID;
                attributeObjectController.UpArrow.SetActive(false);
                attributeObjectController.DownArrow.SetActive(false);
                attributeObjectController.RandomButton.SetActive(false);
            }
            else if (!NodeHandler.Persistantdata.Types[i].Contains(DataTypes.NUMERIC))
            {
                attributeObjectController.AttributeValue.text = "NonNumeric";
                attributeObjectController.UpArrow.SetActive(false);
                attributeObjectController.DownArrow.SetActive(false);
                attributeObjectController.RandomButton.SetActive(false);
                
            }
            else
            {
                attributeObjectController.MinMax = NodeHandler.Persistantdata.MinMaxData[i];
                attributeObjectController.AttributeValue.text = "0.00000";
            }
            newCreatorInfo.SetActive(true);

        }
    }
}
