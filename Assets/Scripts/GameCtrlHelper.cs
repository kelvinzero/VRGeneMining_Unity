using UnityEngine;
using BulletSharp;

public class GameCtrlHelper : MonoBehaviour {

    [SerializeField]
    private bool verbose = true;

    [SerializeField]
    
    GameController gameControl;
    GraphController graphControl;

    private GameObject hitGo;


    public GameObject ScreenPointToRaySingleHitNative(Camera cam, Vector3 pointerCoords)
    {
        RaycastHit hitInfo;
        Ray ray = cam.ScreenPointToRay(pointerCoords);

        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject hitGo = hitInfo.collider.gameObject;

            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": GetMouseButtonDown: Ray did hit. On Object: " + hitGo.name);

            return hitGo;
        }
        else
        {
            return null;
        }
    }

    public GameObject ScreenPointToRaySingleHitWrapper(Camera cam, Vector3 pointerCoords)
    {
        
            hitGo = ScreenPointToRaySingleHitNative(cam, pointerCoords);
        

        if (hitGo != null)
            return hitGo;
        else
            return null;
    }

    public float GetRepulseSphereDiam()
    {
        float sphereDiam;

        
            sphereDiam = graphControl.NodePhysXForceSphereRadius * 2;
        

        return sphereDiam;
    }

    // Use this for initialization
    void Start ()
    {
        gameControl = GetComponent<GameController>();
        graphControl = GetComponent<GraphController>();
    }
}
