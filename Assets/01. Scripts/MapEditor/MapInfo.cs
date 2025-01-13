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
    public KeyCode  requireKey;         //�䱸 �Է�Ű
    public float    startTime;          //���� ����
    public float    endTime;            //�� ����
}
