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
    private int lastTimeMarker2;
    private float tempo;
    public bool playStinger;
    public bool playCrash;
    private int stingerLength;

    private GameObject player;

    private float minY=-5.16f;
    private float maxY=3.34f;

    public bool end=false;
    public float endTimer=0f;

    private int prevPosition;
    public float loops=0f;

    // Start is called before the first frame update
    void Start()
    {
        mainTrack=GetComponent<StudioEventEmitter>().EventInstance;
        glitchyTrack=FMODUnity.RuntimeManager.CreateInstance("event:/glitchyEvent");
        swordTrack=FMODUnity.RuntimeManager.CreateInstance("event:/swordSound");
        crashTrack=FMODUnity.RuntimeManager.CreateInstance("event:/crash");
        tempo=88f;
        playStinger=false;
        playCrash=false;
        stingerLength=Mathf.FloorToInt(2000/(tempo/60));
        Debug.Log(stingerLength);

        player=GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        int position;
        mainTrack.getTimelinePosition(out position);

        if(playStinger){
            int dt=Mathf.FloorToInt(Time.deltaTime*1000);
            if(position+dt>=lastTimeMarker+stingerLength || position<lastTimeMarker){
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
                if(position>lastTimeMarker+stingerLength*2){
                    glitchyTrack.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    mainTrack.setParameterByName("voiceMute",1f);
                }
            }
        }

        if(playCrash){
            
            int dt=Mathf.FloorToInt(Time.deltaTime*1000);
            if(position+dt>=lastTimeMarker2+stingerLength/2 || position<lastTimeMarker2){
                crashTrack.setTimelinePosition(0);
                crashTrack.start();
                playCrash=false;
            }
            
        }

        float k=player.transform.position.y;
        k=(k-minY)/(maxY-minY);
        mainTrack.setParameterByName("synth",1-k);
        mainTrack.setParameterByName("instrumental",k);

        if(end){
            mainTrack.setParameterByName("keys",1f);
            if(position<prevPosition){
                loops+=1;
            }
            if(loops>=4){
                Application.Quit();
            }
        }


        mainTrack.getTimelinePosition(out prevPosition);
    }

    public void PlayStinger(){
        if(!playStinger){
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

    public void PlayCrash(){
        if(!playCrash){
            int position;
            mainTrack.getTimelinePosition(out position);
            lastTimeMarker2=position-(position%(stingerLength/2));
            playCrash=true;
        }
    }

    
}
