using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer=0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(timer>5f){
            Destroy(gameObject);
        }
    }
}
