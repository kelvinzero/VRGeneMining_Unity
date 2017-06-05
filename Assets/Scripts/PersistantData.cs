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
        // Member variables                 Variable name           /////
        Dictionary<string, List<string[]>>  _associationDict;
        double[][]                          _minMaxData;
        List<DataTypes>[]                   _dataSetTypes;
        List<DataTypes>[]                   _assotiationTypes;
        List<string[]>                      _dataSet;
        List<string>                        _datasetNames;
        List<string>                        _associationNames;
        string                              datasetFilename;
        string                              associationsFilename;
        bool                                _hasAssociations        = false;
        bool                                _hasDatafile            = false;

        // Setters/Getters                      Variable name       /////
        public Dictionary<string, List<string[]>> Associations
        {
            get { return _associationDict; }
            set { _associationDict = value; }
        }
        public List<DataTypes>[]                Types
        {
            get { return _dataSetTypes; }
            set { _dataSetTypes = value; }
        }
        public List<DataTypes>[]                AssotiationTypes
        {
            get
            {
                return _assotiationTypes;
            }

            set
            {
                _assotiationTypes = value;
            }
        }
        public List<string[]>                   RecordValues
        {
            get { return _dataSet; }
            set { _dataSet = value; }
        }
        public List<string>                     DatasetNames
        {
            get { return _datasetNames; }
            set { _datasetNames = value; }
        }
        public List<string>                     AssociationNames
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
        public string                           DatasetFilename
        {
            get
            {
                return datasetFilename;
            }

            set
            {
                datasetFilename = value;
            }
        }
        public string                           AssociationsFilename
        {
            get
            {
                return associationsFilename;
            }

            set
            {
                associationsFilename = value;
            }
        }
        public bool                             HasDatafile
        {
            get
            {
                return _hasDatafile;
            }

            set
            {
                _hasDatafile = value;
            }
        }
        public bool                             HasAssociations
        {
            get
            {
                return _hasAssociations;
            }

            set
            {
                _hasAssociations = value;
            }
        }
        public double[][] MinMaxData
        {
            get
            {
                return _minMaxData;
            }

            set
            {
                _minMaxData = value;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
