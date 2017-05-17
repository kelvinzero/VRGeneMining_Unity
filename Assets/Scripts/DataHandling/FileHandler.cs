using Assets.Scripts.DataHandling;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileHandler {


    public FileHandler() { }

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
    public static Dictionary<string, List<string>> LoadAssociations(string filePath, bool hasNames)
    {

        Dictionary<string, List<string>> newDict = new Dictionary<string, List<string>>();
        List<string[]> associationSet = FileHandler.LoadSet(filePath, hasNames);
        List<string> outList;

        foreach (string[] record in associationSet)
        {
            // if the dict doesnt have a key for this record
            if (!newDict.TryGetValue(record[0], out outList))
            {
                List<string> newValueStore = new List<string>();
                newValueStore.Add(record[1]);
                newDict.Add(record[0], newValueStore);
            }
            // if dict has the key, check the values to see if duplicate
            else
            {
                if (!outList.Contains(record[1]))
                    outList.Add(record[1]);
            }
            // reciprocal of above
            if (!newDict.TryGetValue(record[1], out outList))
            {
                List<string> newValueStore = new List<string>();
                newValueStore.Add(record[0]);
                newDict.Add(record[1], newValueStore);
            }
            else
            {
                if (!outList.Contains(record[0]))
                    outList.Add(record[0]);
            }
        }
        return newDict;
    }
}
