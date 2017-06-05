using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataHandling
{
    [RequireComponent(typeof(GraphController))]
   
    public class DataSpaceHandler : MonoBehaviour
    {
        
        PersistantData _programData;
        GraphController _graphController;
        bool verbose = false;

        private int _maxNodes;

        int associationsSpawned;
        int valuesSpawned;

        public PersistantData ProgramData
        {
            get
            {
                return _programData;
            }

            set
            {
                _programData = value;
            }
        }
        public int MaxNodes
        {
            get
            {
                return _maxNodes;
            }

            set
            {
                _maxNodes = value;
            }
        }

        public void LoadNames(string filePath, bool hasNames)
        {
            _programData.DatasetNames = FileHandler.LoadNames(filePath, hasNames);
        }       
        public void InferTypes(string filePath, bool hasNames)
        {
            _programData.Types = FileHandler.InferTypes(filePath, hasNames);
        } 
        public void LoadDataSet(string filePath, bool hasNames)
        {
            _programData.RecordValues = FileHandler.LoadSet(filePath, hasNames);
        }

        public void SpawnAssociations()
        {

        }

        public void SpawnValues()
        {

        }

        public void SpawnValuesWithAssociations()
        {

        }
        private GameObject InstObj(Vector3 createPos)
        {
            return Instantiate(_graphController.nodePrefabPhysX, createPos, Quaternion.identity) as GameObject;
        }

        public GameObject GenerateNode()
        {
            // Method for creating a Node on random coordinates, e.g. when spawning multiple new nodes

            GameObject nodeCreated = null;

            Vector3 createPos = new Vector3(UnityEngine.Random.Range(0, _graphController.nodeVectorGenRange), UnityEngine.Random.Range(0, _graphController.nodeVectorGenRange), UnityEngine.Random.Range(0, _graphController.nodeVectorGenRange));

            nodeCreated = InstObj(createPos);

            if (nodeCreated != null)
            {
                GameObject debugObj = nodeCreated.transform.FindChild("debugRepulseObj").gameObject;
                _graphController.debugObjects.Add(debugObj);
                debugObj.SetActive(false);

                if (verbose)
                    Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Node created: " + nodeCreated.gameObject.name);

            }
            else
            {
                if (verbose)
                    Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Something went wrong, did not get a Node Object returned.");
            }

            return nodeCreated.gameObject;
        }
        private void Start()
        {
            _graphController = GetComponent<GraphController>();
        }
    }
}
