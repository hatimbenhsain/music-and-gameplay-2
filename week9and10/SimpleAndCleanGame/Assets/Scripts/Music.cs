using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using StarterAssets;
using UnityEngine;

public class Music : MonoBehaviour
{
    private EventInstance mainTrack;
    private EventInstance glitchyTrack;
    private EventInstance swordTrack;
    private EventInstance crashTrack;

    private int lastTimeMarker;
    private float tempo;
    public bool playStinger;
    private int stingerLength;

    private GameObject player;

    private float minY=-5.16f;
    private float maxY=3.34f;

    // Start is called before the first frame update
    void Start()
    {
        mainTrack=GetComponent<StudioEventEmitter>().EventInstance;
        glitchyTrack=FMODUnity.RuntimeManager.CreateInstance("event:/glitchyEvent");
        swordTrack=FMODUnity.RuntimeManager.CreateInstance("event:/swordSound");
        crashTrack=FMODUnity.RuntimeManager.CreateInstance("event:/crash");
        tempo=88f;
        playStinger=false;
        stingerLength=Mathf.FloorToInt(2000/(tempo/60));
        Debug.Log(stingerLength);

        player=GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(playStinger){
            int position;
            mainTrack.getTimelinePosition(out position);
            int dt=Mathf.FloorToInt(Time.deltaTime*1000);
            if(position+dt>=lastTimeMarker+stingerLength){
                glitchyTrack.start();
                int trim=(position+dt)%stingerLength;
                glitchyTrack.setTimelinePosition(Random.Range(0,64)*stingerLength);
                playStinger=false;
                mainTrack.setParameterByName("voiceMute",0f);
                // crashTrack.setTimelinePosition(0);
                // crashTrack.start();
            }
        }else{
            PLAYBACK_STATE pbs;
            glitchyTrack.getPlaybackState(out pbs);
            if(pbs==PLAYBACK_STATE.PLAYING){
                int position;
                mainTrack.getTimelinePosition(out position);
                if(position>lastTimeMarker+stingerLength*2){
                    glitchyTrack.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    mainTrack.setParameterByName("voiceMute",1f);
                }
            }
        }

        float k=player.transform.position.y;
        k=(k-minY)/(maxY-minY);
        mainTrack.setParameterByName("synth",1-k);
        mainTrack.setParameterByName("instrumental",k);
    }

    public void PlayStinger(){
        int position;
        mainTrack.getTimelinePosition(out position);
        lastTimeMarker=position-(position%stingerLength);
        playStinger=true;
        swordTrack.getTimelinePosition(out position);
        if(position==0 || position>200){
            swordTrack.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            swordTrack.start();
            swordTrack.setTimelinePosition(0);
        }
    }

    
}
