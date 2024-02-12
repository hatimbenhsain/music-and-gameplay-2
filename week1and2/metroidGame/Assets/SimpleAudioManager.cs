using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class SimpleAudioManager : MonoBehaviour
{
    enum MusicPart {PART1,PART2,PART3}
    [SerializeField] MusicPart musicPart=MusicPart.PART1;
    private EventInstance musicInstance;
    public EventReference fmodEvent;
    // Start is called before the first frame update
    void Start()
    {
        musicInstance=FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        musicInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        musicInstance.setParameterByNameWithLabel("MusicPart",musicPart.ToString());
    }

    void OnDestroy(){
        musicInstance.release();
    }
}
