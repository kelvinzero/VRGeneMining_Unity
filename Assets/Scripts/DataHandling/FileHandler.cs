using Assets.Scripts.DataHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileHandler {


    public FileHandler() { }

    public static double[][] GetMinMax(string path, List<DataTypes>[] types, bool hasNames)
    {
        StreamReader readfile = new StreamReader(path);

        double[][] minMax = new double[types.Length][];

        // initialize array
        for(int i = 0; i < minMax.Length; i++)
        {
            minMax[i] = new double[2];
            minMax[i][0] = double.MaxValue;
            minMax[i][1] = double.MinValue;
        }
        int rowNum = 0;
        if (hasNames)
            readfile.ReadLine();

        while (!readfile.EndOfStream)
        {
            string[] record = Regex.Split(readfile.ReadLine(), @"\s+|,\s*");


            for (int i = 0; i < record.Length; i++)
            {
                if (types[i].Contains(DataTypes.NUMERIC))
                {
                   // Debug.Log(record[i]);
                
                    double columnValue = double.Parse(record[i]);
                    if (columnValue < minMax[i][0])
                        minMax[i][0] = columnValue;
                    if (columnValue > minMax[i][1])
                        minMax[i][1] = columnValue;
                }

            }
        }
        readfile.Close();
        return minMax;
    }

    public static List<string> LoadNames(string path, bool hasNames)
    {
        List<string> attributeNames;

        StreamReader readfile = new StreamReader(path);
        string[] rowSplit = Regex.Split(readfile.ReadLine(), @"\s+|,\s*");
        readfile.Close();
        List<string> rowSplitList = new List<string>();

        if (hasNames)
        {
            foreach (string name in rowSplit)
                rowSplitList.Add(name);
            return rowSplitList;
        }
        attributeNames = new List<string>();
        for (int i = 0; i < rowSplit.Length; i++)
        {
            attributeNames.Add("Attribute " + i);
        }
        return attributeNames;
    }

    public static bool InferHasNames(string path)
    {
        StreamReader readfile = new StreamReader(path);
        float floatHolder;

        string[] attributes = Regex.Split(readfile.ReadLine(), @"\s+|,\s*");

        for (int i = 0; i < attributes.Length; i++)
        {
            if (!float.TryParse(attributes[i], out floatHolder))
            {
                // Debug.Log("found non int " + attributes[i]);
                readfile.Close(); 
                return true;
            }
        }

        readfile.Close();
        return false; ;
    }
    /**
    * Determines numeric or non-numeric types for each data column
    */
    public static List<DataTypes>[] InferTypes(string path, bool hasNames)
    { 
        StreamReader readfile = new StreamReader(path);

        if (hasNames)
            readfile.ReadLine();   
            
        string[] attributes = Regex.Split(readfile.ReadLine(), @"\s+|,\s*");
        List<DataTypes>[] types = new List<DataTypes>[attributes.Length];
        float floatHolder;

        for(int i = 0; i < attributes.Length; i++)
        {
            types[i] = new List<DataTypes>();
            if (float.TryParse(attributes[i], out floatHolder))
                types[i].Add(DataTypes.NUMERIC);
            else
                types[i].Add(DataTypes.NONNUMERIC);
           // Debug.Log(types[i][0].ToString());
        }

        readfile.Close();
        return types;
    }

    public static List<string[]> LoadSet(string path, bool hasNames)
    {
        StreamReader readfile = new StreamReader(path);
        List<string[]> dataTable = new List<string[]>();
        int rowNum = 0;

        while (!readfile.EndOfStream)
        {
            
            string[] record = Regex.Split(readfile.ReadLine(), @"\s+|,\s*");
           
            if(rowNum == 0 && !hasNames)
                dataTable.Add(record);
            else if(rowNum > 0)
                dataTable.Add(record);
            rowNum++;
        }
        readfile.Close();
        return dataTable;
    }

    /**  Loads the association list into a map list
     **/
    public static Dictionary<string, List<string[]>> LoadAssociations(string filePath, bool hasNames)
    {

        Dictionary<string, List<string[]>>      newDict         = new Dictionary<string, List<string[]>>();
        List<string[]>                          associationSet  = FileHandler.LoadSet(filePath, hasNames);
        StreamReader                            readfile = new StreamReader(filePath);
        List<string[]>                          outList;
        
        
        while(!readfile.EndOfStream)
        {            
            string[] record                 = Regex.Split(readfile.ReadLine(), @"\s+|,\s*");
            string[] newRecord              = {record[2].ToLower(), record[1].ToLower()};
            string[] existingRecordAssoc    = { record[0].ToLower(), record[1].ToLower() };

            // if the dict doesnt have a key for this record
            if (!newDict.TryGetValue(record[0].ToLower(), out outList))
            {
                List<string[]> newValueStore = new List<string[]>(); 
                newValueStore.Add(newRecord);
                newDict.Add(record[0].ToLower(), newValueStore);
                //Debug.Log("Adding association " + record[0] + " : " + newValueStore[0][0]);
            }
            // if dict has the key, check the values to see if duplicate
            else
            {
                //Debug.Log(record[0] + " Has associations already " + outList[0][0]);
                bool recordExists = false;
                foreach (string[] association in outList)
                {
                    // if list contains this record already
                    if (association[0].ToLower().Equals(record[2].ToLower()))
                    {
                        recordExists = true;
                        break;
                    }
                }
                // add to association list if not found
                if (!recordExists)
                {
                    outList.Add(newRecord);
                   // Debug.Log("Adding association with existing: " + record[0] + " : " + outList[outList.Count - 1][0]);
                }
            }
            outList = null;
            // create reciprocal associations from associated point  [0][1][associated]
            if(!newDict.TryGetValue(record[2].ToLower(), out outList))
            {
                List<string[]> newValueStore = new List<string[]>();
                newValueStore.Add(existingRecordAssoc);
                newDict.Add(record[2].ToLower(), newValueStore);
            }
            else
            {
                //Debug.Log(record[0] + " Has associations already " + outList[0][0]);
                bool recordExists = false;
                foreach (string[] association in outList)
                {
                    // if list contains this record already
                    if (association[0].Equals(record[0].ToLower()))
                    {
                        recordExists = true;
                        break;
                    }
                }
                // add to association list if not found
                if (!recordExists)
                {
                    outList.Add(existingRecordAssoc);
                    // Debug.Log("Adding association with existing: " + record[0] + " : " + outList[outList.Count - 1][0]);
                }
            }
        }
        return newDict;
    }
}
