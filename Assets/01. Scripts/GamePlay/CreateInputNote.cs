using System.Collections.Generic;
using UnityEngine;

public class CreateInputNote : MonoBehaviour
{
    private MapInfo map;

    private List<MapNote> usedNotes = new();

    public Transform noteParent;
    public Transform poolingParent;
    public GameObject notePrefab;

    public PlayerNoteInput playerNoteInput;

    private void Start()
    {
        map = GamePlayManager.instance.mapInfo;
    }

    private void Update()
    {
        CreateNote();
    }

    private void CreateNote()
    {
        for (int i = 0; i < map.trackNote.Count; i++)
        {
            for (int j = 0; j < map.trackNote[i].notes.Count; i++)
            {
                MapNote currentNote = map.trackNote[i].notes[j];

                if (usedNotes.Contains(currentNote)) continue;

                if (currentNote.startTime - 1000 < GamePlayManager.instance.progress)
                {
                    usedNotes.Add(currentNote);
                    playerNoteInput.notes.Add(GetNotePrefab(currentNote).GetComponent<NoteInfo>());
                }
            }
        }
    }

    private GameObject GetNotePrefab(MapNote note)
    {
        GameObject obj;

        if (poolingParent.childCount > 0)
        {
            obj = poolingParent.GetChild(0).gameObject;
            obj.SetActive(true);
            obj.GetComponent<NoteInfo>().Setting(note);
            obj.transform.SetParent(noteParent);

            
        }
        else
        {
            obj = Instantiate(notePrefab, noteParent);
            obj.GetComponent<NoteInfo>().Setting(note);
        }

        return obj;
    }
}
