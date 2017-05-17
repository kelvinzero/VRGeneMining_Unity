using Assets.Scripts.DataHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class PersistantData : MonoBehaviour
    {
        List<DataTypes>[] _types;
        List<string[]> _dataSet;
        Dictionary<string, List<string>> _associationDict;
        List<string> _datasetNames;
        List<string> _associationNames;

        public List<string> DatasetNames
        {
            get { return _datasetNames; }
            set { _datasetNames = value; }
        }
        public List<string[]> RecordValues
        {
            get { return _dataSet; }
            set { _dataSet = value; }
        }
        public List<DataTypes>[] Types
        {
            get { return _types; }
            set { _types = value; }
        }
        public Dictionary<string, List<string>> Associations
        {
            get { return _associationDict; }
            set { _associationDict = value; }
        }

        public List<string> AssociationNames
        {
            get
            {
                return _associationNames;
            }

            set
            {
                _associationNames = value;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
