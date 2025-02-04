using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System.Windows.Forms;
using TMPro;

public class MapEditor : MonoBehaviour
{
    public MapInfo mapInfo; //수정할 맵 설정

    public TrackEditor trackEditor;

    public List<MapInfo> workHistory;

    public Transform timeLineParent;
    public GameObject timeLinePrefab;

    public TMP_InputField bpmInput;
    private float lastestBPM;

    private AudioClip lastestBGM;

    private List<GameObject> currentTimelines = new();

    private void Update()
    {
        ShortcutKey();

        if(float.Parse(bpmInput.text) != lastestBPM || lastestBGM != mapInfo.backgroundMusic)
        {
            mapInfo.bpm = float.Parse(bpmInput.text);
            lastestBPM = float.Parse(bpmInput.text);
            lastestBGM = mapInfo.backgroundMusic;

            SetTimeLine();
        }
    }

    public void SaveInfo()
    {
        mapInfo.Initialization();

        for (int i = 0; i < trackEditor.tracks.Count; i++)
        {
            for (int j = 0; j < trackEditor.tracks[i].childCount; j++)
            {
                EditorNoteInfo note = trackEditor.tracks[i].GetChild(j).GetComponent<EditorNoteInfo>();
                RectTransform noteRect = note.transform as RectTransform;

                mapInfo.trackNote[i].notes.Add(new MapNote(InputManager.instance.KeyCodeToEnum(note.noteKey), noteRect.offsetMin.x, noteRect.offsetMax.x, note.isNotMove));
            }
        }
    }

    private void LoadInfo()
    {
        trackEditor.Initialization();

        for (int i = 0; i < mapInfo.trackNote.Count; i++)
        {
            for (int j = 0; j < mapInfo.trackNote[i].notes.Count; j++)
            {
                GameObject newNote = Instantiate(trackEditor.notePrefab, trackEditor.tracks[i]);
                newNote.name = "Note";
                newNote.GetComponent<EditorNoteInfo>().noteKey = InputManager.instance.EnumToKeyCode(mapInfo.trackNote[i].notes[j].requireKey);
                newNote.GetComponent<EditorNoteInfo>().isNotMove = mapInfo.trackNote[i].notes[j].isNotMove;
                RectTransform noteRect = newNote.GetComponent<RectTransform>();
                noteRect.offsetMin = new(mapInfo.trackNote[i].notes[j].startTime, noteRect.offsetMin.y);
                noteRect.offsetMax = new(mapInfo.trackNote[i].notes[j].endTime, noteRect.offsetMax.y);
            }
        }
    }

    private void ShortcutKey()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
                OnClickSaveLevel();

            if (Input.GetKeyDown(KeyCode.O))
                OnClickLoadLevel();

            if (Input.GetKeyDown(KeyCode.Z))
                OnClickUndo();

            if (Input.GetKeyDown(KeyCode.R))
                OnClickRedo();
        }
    }

    #region TopButtons

    public void OnClickSaveLevel()
    {
        SaveInfo();

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.DefaultExt = "json"; // JSON 파일만 필터링
        saveFileDialog.Title = "Select a JSON File"; // 탐색기 타이틀
        saveFileDialog.Filter = "JSON files (*.json)|*.json"; // JSON 파일만 필터링

        if (saveFileDialog.ShowDialog() == DialogResult.OK) //저장했을 때
        {
            string filePath = saveFileDialog.FileName; //파일 경로 가져옴

            string json = JsonUtility.ToJson(mapInfo, true); //맵 정보를 JSON으로 변경

            File.WriteAllText(filePath, json); //지정한 경로에 JSON파일 저장
        }
    }

    public void OnClickLoadLevel()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "JSON files (*.json)|*.json"; // JSON 파일만 필터링
        openFileDialog.Title = "Select a JSON File"; // 탐색기 타이틀

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string filePath = openFileDialog.FileName;
                string json = File.ReadAllText(filePath);

                JsonUtility.FromJsonOverwrite(json, mapInfo);

                LoadInfo();
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON 내부 형식이 올바르지 않거나, 다른 오류가 존재합니다.\n{e}");
            }
        }
    }

    public void OnClickUndo()
    {
        
    }

    public void OnClickRedo()
    {

    }

    #endregion

    private void UpdateWorkHistory()
    {
        workHistory.Insert(0, mapInfo);
    }

    private void SetTimeLine()
    {
        if (mapInfo.backgroundMusic == null) {
            Debug.LogError("음악 파일 없음");
            return;
        }

        if(mapInfo.bpm == 0)
        {
            Debug.LogError("BPM 설정 안됨");
            return;
        }

        SoundManager.instance.bgmSource.clip = lastestBGM;

        for(int i=currentTimelines.Count - 1; i>=0; i--)
        {
            Destroy(currentTimelines[i]);
        }

        int timeLineLength = Mathf.FloorToInt(mapInfo.backgroundMusic.length * mapInfo.bpm / 60 / 2);

        for(int i=0; i<timeLineLength; i++)
        {
            if (timeLineParent.childCount > timeLineLength) return;

            if (timeLineParent.childCount > i) continue;

            GameObject newTimeLine = Instantiate(timeLinePrefab, timeLineParent);
            newTimeLine.transform.Find("Index").GetComponent<TMP_Text>().text = $"{i}";

            currentTimelines.Add(newTimeLine);
        }
    }
}
