using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteTest : MonoBehaviour
{
    private MapInfo map;

    private HashSet<MapNote> usedNote = new();

    private void Start()
    {
        map = GamePlayManager.instance.mapInfo;
    }

    private void Update()
    {
        if (map == null)
        {
            if (GamePlayManager.instance.mapInfo != null)
                map = GamePlayManager.instance.mapInfo;
            else
                return;
        }
           

        foreach (TrackNote track in map.trackNote)
        {
            foreach(MapNote note in track.notes)
            {
                if(!usedNote.Contains(note))
                    UpdateCheckCorrect(note);
            }
        }
    }

    private void UpdateCheckCorrect(MapNote note)
    {
        float value = note.startTime - GamePlayManager.instance.progress - 50;

        if(Mathf.Abs(value) <= 32f && Input.GetKey(InputManager.instance.EnumToKeyCode(note.requireKey)))
            CheckCorrect(value, note);

        if(value < -32f)
        {
            GamePlayManager.instance.ResetCombo();
            GamePlayManager.instance.DecreaseLife(1);
            usedNote.Add(note);
        }
            
    }

    private void CheckCorrect(float value, MapNote note)
    {
        if(Mathf.Abs(value) <= 16f)
        {
            print("しし");
        }
        else if (Mathf.Abs(value) <= 32f)
        {
            print("いい");
        }

        GamePlayManager.instance.AddCombo();

        usedNote.Add(note);
    }
}
