using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatapointInfoPanelScript : MonoBehaviour {

    private List<DataInfoAttributeHolderObject> attributeInfoScripts;
    public GameObject AttributeInfoHolder;

    private void Start()
    {
        PersistantData pdata = GameObject.Find("PersistentData").GetComponent<PersistantData>();
        attributeInfoScripts = new List<DataInfoAttributeHolderObject>();

        AddAttributes(pdata.DatasetNames.ToArray());
    }
    public void AddAttributes(string[] names)
    {
        foreach(string name in names)
        {
            GameObject newAttribute = Instantiate(AttributeInfoHolder, this.transform);
            DataInfoAttributeHolderObject attributeInfo = newAttribute.GetComponent<DataInfoAttributeHolderObject>();
            attributeInfo.AttributeNameText.text = name;
            attributeInfoScripts.Add(attributeInfo);
            newAttribute.SetActive(true);
        }
    }

    public void SetValues(string[] values)
    {
        for(int i = 0; i < values.Length; i++)
            attributeInfoScripts[i].AttributeValueText.text = values[i];
    } 
}
