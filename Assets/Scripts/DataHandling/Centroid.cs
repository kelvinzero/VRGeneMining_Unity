using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataHandling
{
    public class Centroid
    {
        private readonly bool verbose = false;
        int                 _recordsInCluster;
        GameObject          _centroidObject;
        List<Centroid>      _allCentroids;
        List<DataTypes>[]   _columnTypes;
        Cluster             _cluster;
        double[]            _columnTotals;
        double[]            _centroidValues;
        bool                _firstRun = true;

        public double[] CentroidValues
        {
            get
            {
                return _centroidValues;
            }

            set
            {
                _centroidValues = value;
            }
        }
        public double[] ColumnTotals
        {
            get
            {
                return _columnTotals;
            }

            set
            {
                _columnTotals = value;
            }
        }
        public bool FirstRun
        {
            get
            {
                return _firstRun;
            }

            set
            {
                _firstRun = value;
            }
        }

        public Centroid(GameObject centroidObj, Cluster clusterParent, List<DataTypes>[] columntypes, List<Centroid> allCentroids)
        {
            _recordsInCluster   = 0;
            _centroidObject     = centroidObj;
            _allCentroids       = allCentroids;
            _columnTypes        = columntypes;
            _cluster            = clusterParent;
            ColumnTotals        = new double[columntypes.Length];
            CentroidValues      = new double[columntypes.Length];
            for (int i = 0; i < ColumnTotals.Length; i++)
            {
                ColumnTotals[i] = 0.0f;
                CentroidValues[i] = 0.0f;
            }
            if (verbose) Debug.Log("Centroid.Centroid created");
        }

        public void SetNewCentroid(string[] reportvals)
        {
            if (verbose) Debug.Log("in Centroid.SetNewCentroid");

            // accumulate the reported values to the totals
            for (int i = 0; i < reportvals.Length; i++)
            {
                if (verbose) Debug.Log("report val " + i + " = " + reportvals[i]);
                // if this attribute is numeric and not ignored, add to totals and make it the centroid
                if (_columnTypes[i].Contains(DataTypes.NUMERIC) && !_columnTypes[i].Contains(DataTypes.IGNORE))
                {
                    _columnTotals[i]    = double.Parse(reportvals[i]);
                    _centroidValues[i]  = ColumnTotals[i];
                }
            }
            FirstRun = false;          
             // report to cluster 
            _cluster.ReportDatapoint(true);
            _recordsInCluster++;
        }

        public void ReportIn(string[] reportvals, bool changed)
        {
            // accumulate the reported values to the totals
            for(int i = 0; i < reportvals.Length; i++)
            {
                // if this attribute is numeric and not ignored, add to totals
                if (_columnTypes[i].Contains(DataTypes.NUMERIC) && !_columnTypes[i].Contains(DataTypes.IGNORE))
                    _columnTotals[i] += double.Parse(reportvals[i]);
            }
            FirstRun = false;

            // tell the cluster a datapoint reported
            _cluster.ReportDatapoint(changed);
            _recordsInCluster++;
        }

        public GameObject GetGameObject()
        {
            return _centroidObject;
        }

        public void RecalculateCentroid()
        {
            // calculate centroid and reset totals
            for(int i = 0; i < _columnTotals.Length; i++)
            {
                 if (_columnTypes[i].Contains(DataTypes.NUMERIC) && !_columnTypes[i].Contains(DataTypes.IGNORE))
                {
                    _centroidValues[i] = _columnTotals[i] / _recordsInCluster;
                    _columnTotals[i] = 0.0d;
                }
            }
            // reset record count
            _recordsInCluster = 0;
        }
        

    }
    

}
