using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodePhysX : Node {

    private Rigidbody thisRigidbody;

    private float sphRadius;
    private float sphRadiusSqr;
    private float min = -1.5f;
    private float max = 1.5f;

    Color lowColor = Color.red;
    Color midColor = Color.white;
    Color highColor = Color.blue;

    List<double[]> recordValues;
    List<string> attributeNames;
    List<string> associations;


    public List<double[]> RecordValues
    {
        get { return recordValues; }
        set { recordValues = value; }
    }
    public List<string> AttributeNames
    {
        get { return attributeNames; }
        set { attributeNames = value; }
    }
    public List<string> Associations
    {
        get { return associations; }
        set { associations = value; }
    }

    protected override void doGravity()
    {
        // Apply global gravity pulling node towards center of universe
        Vector3 dirToCenter = - this.transform.position;
        Vector3 impulse = dirToCenter.normalized * thisRigidbody.mass * graphControl.GlobalGravityPhysX;
        thisRigidbody.AddForce(impulse);
    }

    protected override void doRepulse()
    {
        // test which node in within forceSphere.
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, sphRadius);

        // only apply force to nodes within forceSphere, with Falloff towards the boundary of the Sphere and no force if outside Sphere.
        foreach (Collider hitCollider in hitColliders)
        {
            Rigidbody hitRb = hitCollider.attachedRigidbody;

            if (hitRb != null && hitRb != thisRigidbody)
            {
                Vector3 direction = hitCollider.transform.position - this.transform.position;
                float distSqr = direction.sqrMagnitude;

                // Normalize the distance from forceSphere Center to node into 0..1
                float impulseExpoFalloffByDist = Mathf.Clamp( 1 - (distSqr / sphRadiusSqr), 0, 1);

                // apply normalized distance
                hitRb.AddForce(direction.normalized * graphControl.RepulseForceStrength * impulseExpoFalloffByDist);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        thisRigidbody = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // updating variable here, as opposed to doing it in Start(), otherwise we won't see runtime updates of forceSphereRadius
        sphRadius = graphControl.NodePhysXForceSphereRadius;
        sphRadiusSqr = sphRadius * sphRadius;
    }
}