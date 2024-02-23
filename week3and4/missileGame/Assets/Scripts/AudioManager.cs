using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using System.Text;
using FMOD;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update

    // class TimelineInfo
    // {
    //     public int currentMusicBeat = 0;
    //     public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    // }

    //TimelineInfo timelineInfo;
    private StudioEventEmitter emitter;
    private EventInstance instance;
    public int targetLoop;
    EVENT_CALLBACK cb;
    void Start()
    {
        emitter=GetComponent<StudioEventEmitter>();
        targetLoop=1;

        instance=emitter.EventInstance;
        //cb = new EVENT_CALLBACK(StudioEventCallback);
        //instance.setCallback(cb, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        instance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLoop(int i){
        string name="LOOP1";
        switch(i){
            case 1:
                name="LOOP1";
                break;
            case 2:
                name="LOOP2";
                break;
            case 3:
                name="LOOP3";
                break;
        }
        instance.setParameterByNameWithLabel("LOOP",name);
        targetLoop=i;
    }

    public int GetTargetLoop(){
        return targetLoop;
    }

    public void SetSynth(float n){
        instance.setParameterByName("playingSynth",Mathf.Clamp(n,0f,1f));
    }

    public void Stop(){
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

// //code from https://qa.fmod.com/t/sync-game-events-to-music-beats/12111/2
//     public RESULT StudioEventCallback(EVENT_CALLBACK_TYPE type, IntPtr eventInstance, IntPtr parameters)
//     {
//         if (type == EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
//         {
//             var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(TIMELINE_MARKER_PROPERTIES));
//             timelineInfo.lastMarker = parameter.name;
//             UnityEngine.Debug.Log(timelineInfo.lastMarker);
//         }
//         if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
//         {
//             TIMELINE_BEAT_PROPERTIES beat = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(TIMELINE_BEAT_PROPERTIES));
//             //UnityEngine.Debug.Log("beat");
//         }
//         return FMOD.RESULT.OK;
//     }
}
