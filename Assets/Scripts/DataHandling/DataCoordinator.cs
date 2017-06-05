using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataHandling
{
    public class DataCoordinator
    {
        List<GameObject>             _records;
        DataTypes                   _originType;
        List<DataTypes>[]           _columnTypes;
        List<string>                _columnNames;

        public int Count
        {
            get
            {
                return Records.Count;
            }
        }
        public List<DataTypes>[] ColumnTypes
        {
            get
            {
                return _columnTypes;
            }

            set
            {
                _columnTypes = value;
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
        public List<string> ColumnNames
        {
            get
            {
                return _columnNames;
            }

            set
            {
                _columnNames = value;
            }
        }
        public List<GameObject> Records
        {
            get
            {
                return _records;
            }

            set
            {
                _records = value;
            }
        }
        
        public bool DestroyLastAssociation()
        {

            return true;
        }

        public bool DestroyLastDataPoint()
        {
            if (_records.Count == 0)
                return false;

            GameObject removeRecord = _records[Count - 1];
            _records.RemoveAt(Count - 1);
           
            // destroy any cluster links
            NodePhysX recordScript = removeRecord.GetComponent<NodePhysX>();
            recordScript.DestroyCentroidLinks();
            recordScript.DestroyAssociationLinks();
            recordScript.DestroyGeneLinks();

            // destroy the record
            UnityEngine.Object.Destroy(removeRecord);
            return true;
        }

        public void DestroyDataPoint(GameObject record)
        {
            if (!Records.Contains(record))
            {
                Debug.Log("Record not found");
                return;
            }
            _records.Remove(record);
            // destroy any cluster links
            NodePhysX recordScript = record.GetComponent<NodePhysX>();
            recordScript.DestroyCentroidLinks();
            recordScript.DestroyAssociationLinks();
            recordScript.DestroyGeneLinks();

            // destroy the record
            UnityEngine.Object.Destroy(record);
        }

        public void DestroyDataPoint(int index)
        {
            if(index >= Count)
            {
                Debug.Log("index greater than records count");
                return;
            }
            GameObject removeRecord = _records[index];
            _records.RemoveAt(index);

            // destroy any cluster links
            NodePhysX recordScript = removeRecord.GetComponent<NodePhysX>();
            recordScript.DestroyCentroidLinks();
            recordScript.DestroyAssociationLinks();
            recordScript.DestroyGeneLinks();

            // destroy the record
            UnityEngine.Object.Destroy(removeRecord);
        }

        public DataCoordinator(){
            Records = new List<GameObject>();
        }



    }
}
