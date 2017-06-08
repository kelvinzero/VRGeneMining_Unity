using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletSharp;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Assets.Scripts.DataHandling
{
    public class Cluster
    {
        private readonly bool verbose = true;
        int     _pointsReported;
        int     _pointsTotal;
        int     _clusterCount;
        int     _focusAttribute = -1;
        bool    _allowPointRecalculate;
        bool    _clusteringOn;

        List<Centroid>      _centroids;
        GraphController     _graphController;
        List<DataTypes>[]   _columnTypes;

        public Cluster() { }

        public Cluster(int k, int pointsCount, GraphController graphcontroller, List<DataTypes>[] columntypes)
        {
            _columnTypes        = columntypes;
            PointsTotal         = pointsCount;
            ClusterCount       = k;
            Centroids           = new List<Centroid>();
            _graphController    = graphcontroller;
            
            if(verbose) Debug.Log("Cluster created");
            
        }

        public int PointsTotal
        {
            get
            {
                return _pointsTotal;
            }

            set
            {
                _pointsTotal = value;
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
        public List<Centroid> Centroids
        {
            get
            {
                return _centroids;
            }

            set
            {
                _centroids = value;
            }
        }
        public int FocusAttribute
        {
            get
            {
                return _focusAttribute;
            }

            set
            {
                _focusAttribute = value;
            }
        }

        public int ClusterCount
        {
            get
            {
                return _clusterCount;
            }

            set
            {
                _clusterCount = value;
            }
        }

        public void SetParams(int k, int pointsCount, GraphController graphcontroller, List<DataTypes>[] columntypes)
        {
            _columnTypes = columntypes;
            PointsTotal = pointsCount;
            ClusterCount = k;
            Centroids = new List<Centroid>();
             _graphController = graphcontroller;
            if (verbose) Debug.Log("Setaparamsdone");
        }

        public void AddCentroid()
        {
            GameObject newRoidObj = _graphController.GenerateCentroid();
            if (verbose) Debug.Log("add centroid Cluster.Centroid created");
            Centroids.Add(new Centroid(newRoidObj, this, _columnTypes, Centroids));
            if (verbose) Debug.Log("add centroid Cluster.Centroid added to list");
            ClusterCount++;
        }

        public void RemoveCentroid()
        {
            if (ClusterCount == 0)
                return;
            GameObject removeRoidObj = Centroids[Centroids.Count - 1].GetGameObject();
            Centroids.RemoveAt(Centroids.Count - 1);
            UnityEngine.Object.Destroy(removeRoidObj);
            ClusterCount--;
        }

        public void InitializeCentroids()
        {
            if (verbose) Debug.Log("In initialize centroids");
            Centroids        = new List<Centroid>();
            _pointsReported  = 0;

            for (int i = 0; i < ClusterCount; i++)
            {
                if (verbose) Debug.Log("Cluster.Creating centroid");
                GameObject newRoidObj = _graphController.GenerateCentroid();
                if (verbose) Debug.Log("Cluster.Centroid created");
                Centroids.Add(new Centroid(newRoidObj, this, _columnTypes, Centroids));
                if (verbose) Debug.Log("Cluster.Centroid added to list");
                if (verbose) Debug.Log("Cluster.Centroid " + i + " initialized");
            }
            
        }

        public bool CanDatapointMove()
        {
            return _allowPointRecalculate;
        }

        int pointsMoved = 0;
        public void ReportDatapoint(bool changed)
        {
            if (changed)
                pointsMoved++;
            _pointsReported++;
            // if all points reported
            if(_pointsReported >= _pointsTotal)
            {
                _allowPointRecalculate = false;
                // pause recalculating
                Debug.Log("Recalculating ");
                // centroids recalculate
                foreach(Centroid centroid in Centroids)
                    centroid.RecalculateCentroid();
                Debug.Log("centroids recalculated done");
                _allowPointRecalculate = true;
                _pointsReported = 0;
                // unpause recalc
                pointsMoved = 0;
            }
            //Debug.Log("Datapoint reported");
        }

        public void SetClusteringOn(bool isOn)
        {

            ClusteringOn = isOn;
            Debug.Log("Turning clustering on" + ClusteringOn);
            if (isOn)
            {
                InitializeCentroids();
                ClusteringOn = true;
                _allowPointRecalculate = true;
                Debug.Log("Clustering turned on in cluster");
            }
            else
            {
                ClusteringOn = false;
                _allowPointRecalculate = false;
                foreach(Centroid centroid in Centroids)               
                    UnityEngine.Object.Destroy(centroid.GetGameObject());
                Debug.Log("Clustering off");
                Centroids = new List<Centroid>();
            }
        }
        public double EuclideanDistanceToCentroid(double[] centroidVals, string[] record)
        {
            double distance = 0.0d;
            for (int i = 0; i < centroidVals.Length; i++)
            {
                if (verbose)
                    ;    // Debug.Log(centroidVals[i] + " - " + record[i]);
                // accumulate distance if type is numeric and non ignore
                if (_columnTypes[i].Contains(DataTypes.NUMERIC) && !_columnTypes[i].Contains(DataTypes.IGNORE))
                {
                    distance += Math.Pow(centroidVals[i] - double.Parse(record[i]), 2);
                }
            }
            return Math.Sqrt(distance);
        }
    }
}
