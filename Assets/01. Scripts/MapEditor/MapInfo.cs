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

public enum NoteType
{
     SingleNote,
     LongNote
}

[System.Serializable]
public class MapNote
{
    public KeyCode  requireKey;         //요구 입력키
    public float    requireInputTime;   //요구 입력시간 (해당 시간 이상 눌러야함)
    public NoteType noteType;           //단일노트, 롱노트
}
