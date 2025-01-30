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
    public KeyList  requireKey;         //�䱸 �Է�Ű
    public float    startTime;          //���� ����
    public float    endTime;            //�� ����
    public bool     isNotMove = false;  //Ű �Է��� ������, ������ �������� ����


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