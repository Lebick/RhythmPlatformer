using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapInfo", menuName = "Custom/MapInfo", order = 2)]
public class MapInfo : ScriptableObject
{
    public float bpm;
    public float offset;

    public List<TrackNote> trackNote = new();

    public AudioClip backgroundMusic;
    
    public void Initialization()
    {
        for(int i=0; i<trackNote.Count; i++)
        {
            trackNote[i].notes.Clear();
        }
    }
}

[Serializable]
public class MapNote
{
    public KeyList  requireKey;         //요구 입력키
    public float    startTime;          //시작 마디
    public float    endTime;            //끝 마디
    public bool     isNotMove = false;  //키 입력은 받지만, 실제로 움직이진 않음


    public MapNote(KeyList requireKey, float startTime, float endTime, bool isNotMove)
    {
        this.requireKey = requireKey;
        this.startTime = startTime;
        this.endTime = endTime;
        this.isNotMove = isNotMove;
    }
}

[Serializable]
public class TrackNote
{
    public List<MapNote> notes = new();
}