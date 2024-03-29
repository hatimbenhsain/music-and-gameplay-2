using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBody : MonoBehaviour
{
    public FauxGravityAttractor attractor;
    private Rigidbody body;
    private Transform myTransform;
    public bool halveGravity;
    // Start is called before the first frame update
    void Start()
    {
        myTransform=transform;
        body=GetComponent<Rigidbody>();
        body.constraints=RigidbodyConstraints.FreezeRotation;
        body.useGravity=false;
        halveGravity=false;
    }

    // Update is called once per frame
    void Update()
    {
        attractor.Attract(myTransform);
    }
}
