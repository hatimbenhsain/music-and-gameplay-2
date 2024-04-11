using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartlessGenerator : MonoBehaviour
{
    public float generationTime=5f;
    public float timer=0f;
    public GameObject heartlessPrefab;
    public float generationDistance=10f;

    private Transform generationCenter;
    // Start is called before the first frame update
    void Start()
    {
        generationCenter=transform;
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(timer>generationTime){
            timer=0f;
            Vector3 pos=Random.onUnitSphere*generationDistance;
            Instantiate(heartlessPrefab,pos,Quaternion.Euler(0,0,0));
        }
    }
}
