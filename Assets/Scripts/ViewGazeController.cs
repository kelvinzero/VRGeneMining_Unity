using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VR;

public class ViewGazeController : MonoBehaviour {

    bool _vrActivated;
    bool _waitForButtonUp;
    bool _waitForDeselect;
    GameObject _targetSelected;
    GameObject _lastTargetSelected;
    public Camera vrcam;
    public GameObject LayoutPanel;


    // Use this for initialization
    void Start()
    {
        _vrActivated = VRSettings.isDeviceActive;
        _waitForButtonUp = false;
        _waitForDeselect = false;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        doRayCast();
        updateReticleDist();
    }

    void updateReticleDist()
    {
        float               distance;
        GameObject          targetSelected;

        PointerEventData    pointer         = new PointerEventData(EventSystem.current);
        List<RaycastResult> targetsHit      = GetRaycastTargets(pointer);
        Ray ray = vrcam.ScreenPointToRay(pointer.position);
        RaycastHit hit;

        if (targetsHit.Count > 0)
        {
            
            targetSelected = targetsHit[0].gameObject;
            distance = Vector3.Distance(vrcam.transform.position, targetSelected.transform.position) - 0.5f;
            if (distance > 4)
                distance = 4;
        }
        else if (Physics.Raycast(ray, out hit))
        {
            distance = Vector3.Distance(vrcam.transform.position, hit.transform.position) - 0.5f;
            if (distance > 4)
                distance = 4;
        }
        else
        {
            distance = 3.0f;
        }
        transform.position = vrcam.transform.position + vrcam.transform.rotation * Vector3.forward * distance;
        transform.LookAt(vrcam.transform);
    }

    void doRayCast()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> targetsHit = GetRaycastTargets(pointer);

        if (targetsHit.Count > 0)
        {
            _targetSelected = targetsHit[0].gameObject;

            if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
            {
                if (_targetSelected.GetComponent<IPointerClickHandler>() != null)
                    _targetSelected.GetComponent<IPointerClickHandler>().OnPointerClick(pointer);

                else if (_targetSelected.GetComponentInParent<IPointerClickHandler>() != null)
                    _targetSelected.GetComponentInParent<IPointerClickHandler>().OnPointerClick(pointer);
            }
        }
        Ray ray = vrcam.ScreenPointToRay(pointer.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "node")
            {
                NodePhysX nodescript = hit.collider.GetComponentInParent<NodePhysX>();
                if (nodescript != null)
                    LayoutPanel.GetComponent<DatapointInfoPanelScript>().SetValues(nodescript.RecordValues);
            }
        }
    }

    List<RaycastResult> GetRaycastTargets(PointerEventData pointer)
    {
        List<RaycastResult> targetsHit = new List<RaycastResult>();

        if (_vrActivated)
            pointer.position = new Vector2(VRSettings.eyeTextureWidth / 2, VRSettings.eyeTextureWidth / 2);
        else
            pointer.position = new Vector2(Screen.width / 2, Screen.height / 2);

        pointer.button = PointerEventData.InputButton.Left;

        EventSystem.current.RaycastAll(pointer, targetsHit);
        return targetsHit;
    }
}
