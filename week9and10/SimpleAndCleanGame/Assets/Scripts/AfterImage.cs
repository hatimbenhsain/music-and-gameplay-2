using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AfterImage : MonoBehaviour
{
    public Transform targetTransform;
    public float moveSpeed=1f;
    public float rotationSpeed=1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position=Vector3.Lerp(transform.position,targetTransform.position,moveSpeed*Time.deltaTime);
        transform.rotation=Quaternion.Lerp(transform.rotation,targetTransform.rotation,rotationSpeed*Time.deltaTime);
    }
}
