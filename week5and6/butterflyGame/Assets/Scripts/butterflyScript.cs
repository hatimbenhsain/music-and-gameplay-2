using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using System.Diagnostics.Tracing;
using MidiParser;

public class ButterflyScript : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    TornadoScript tornadoScript;

    public Color dullColor;
    public Color baseColor;
    public Color glowColor;

    public Material wingMaterial;

    Animator animator;

    public StudioEventEmitter wingFlapEmitter;
    public EventInstance wingFlapInstance;

    private EventInstance prevChord;
    private EventInstance currentChord;

    private int flapCount;

    private string[] chords={"Chord 1","Chord 2","Chord 3","Chord 4"};

    private List<MidiEvent> notes;
    private List<Coroutine> coroutines;
    private int ticksPerQuarterNote;

    void Awake()
    {
        inputManager=GetComponent<InputManager>();
        playerLocomotion=GetComponent<PlayerLocomotion>();
        cameraManager=FindObjectOfType<CameraManager>();
        tornadoScript=FindObjectOfType<TornadoScript>();
        animator=GetComponentInChildren<Animator>();

        wingFlapInstance=wingFlapEmitter.EventInstance;
        flapCount=0;

        ParseMidi();
        coroutines=new List<Coroutine>();
    }

    // Update is called once per frame
    void Update()
    {
        inputManager.HandleAllInputs();
        animator.SetBool("diving",(playerLocomotion.diving && !playerLocomotion.jumping));

        float v=Mathf.Abs(tornadoScript.period-tornadoScript.avgPeriod);
        if(v<=tornadoScript.periodTreshold1){
            v=1f;
        }else if(v<=tornadoScript.periodTreshold2*2){
            v=1-(v-tornadoScript.periodTreshold1)/(tornadoScript.periodTreshold2*2-tornadoScript.periodTreshold1);
        }else{
            v=0f;
        }
        if(tornadoScript.avgPeriod<tornadoScript.idealPeriod/2){
            v=Mathf.Pow(v*2*(tornadoScript.idealPeriod/2-tornadoScript.avgPeriod)/tornadoScript.idealPeriod,2f);
        }
        Color tColor=Color.Lerp(dullColor,baseColor,v);
        wingMaterial.color=Color.Lerp(wingMaterial.color,tColor,Time.deltaTime*10f);
    }

    private void FixedUpdate() {
        playerLocomotion.speedModifier=Mathf.Pow(tornadoScript.currentRadius,0.4f);
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate() {
        cameraManager.HandleAllCameraMovement();
    }

    public void Jump(float y){
        tornadoScript.Jumped(y);
        animator.SetTrigger("jump");
        // wingFlapInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // wingFlapInstance.start();
        wingFlapInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Wingflap");
        wingFlapInstance.start();
        wingFlapInstance.release(); 
        int maxFlap=2; //number of flaps before next chord play
        if(flapCount%maxFlap==0){
            prevChord=currentChord;
            prevChord.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentChord=FMODUnity.RuntimeManager.CreateInstance("event:/"+chords[(int)Mathf.Floor(flapCount/maxFlap)%chords.Length]);
            currentChord.start();
            currentChord.release();
        }
        flapCount++;

        foreach(Coroutine c in coroutines){
            StopCoroutine(c);
        }
        coroutines=new List<Coroutine>();

        foreach(MidiEvent m in notes){
            float time=m.Time*60000/((60*2/tornadoScript.period)*ticksPerQuarterNote);
            var note = m.Note;
            var velocity = m.Velocity;
            string eventName="";
            switch(note){
                case 55:
                    eventName="Drum 2";
                    break;
                case 60:
                    eventName="Drum 1";
                    break;
            }
            UnityEngine.Debug.Log(time);
            
            coroutines.Add(StartCoroutine(CreateAction(time/1000,eventName)));
        }
    }

    void ParseMidi(){
        UnityEngine.Debug.Log("parse midi");
        MidiFile midiFile = new MidiFile("drumMidi.mid");

        // 0 = single-track, 1 = multi-track, 2 = multi-pattern
        var midiFileformat = midiFile.Format;

        // also known as pulses per quarter note
        ticksPerQuarterNote = midiFile.TicksPerQuarterNote;

        int bpm=80;

        notes=new List<MidiEvent>();

        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    notes.Add(midiEvent);
                }
            }

            foreach(var textEvent in track.TextEvents){
                if(textEvent.TextEventType==TextEventType.Lyric){
                    var time = textEvent.Time;
                    var text = textEvent.Value;
                }
            }    
        }
    }

    IEnumerator CreateAction(float t, string eventName)
	{
		yield return new WaitForSeconds(t);

    	EventInstance instance = FMODUnity.RuntimeManager.CreateInstance("event:/"+eventName);
        instance.start();
        instance.release(); 
        UnityEngine.Debug.Log(eventName);
        UnityEngine.Debug.Log(t);
        
	}

}
