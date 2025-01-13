using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapInfo", menuName = "Custom/MapInfo", order = 2)]
public class MapInfo : ScriptableObject
{
    public float bpm;

    public List<MapNote> track1Note = new();
    public List<MapNote> track2Note = new();
    public List<MapNote> track3Note = new();
    public List<MapNote> track4Note = new();
    public List<MapNote> track5Note = new();

    public AudioClip backgroundMusic;
    
}

[System.Serializable]
public class MapNote
{
    public KeyCode  requireKey;         //요구 입력키
    public float    startTime;          //시작 마디
    public float    endTime;            //끝 마디
}
