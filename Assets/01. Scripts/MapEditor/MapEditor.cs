using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using System.Windows.Forms;
using TMPro;

public class MapEditor : MonoBehaviour
{
    public MapInfo mapInfo; //������ �� ����

    public List<MapInfo> workHistory;

    public Transform timeLineParent;
    public GameObject timeLinePrefab;

    public TMP_InputField bpmInput;
    private float lastestBPM;

    private void Update()
    {
        ShortcutKey();

        if(float.Parse(bpmInput.text) != lastestBPM)
        {
            mapInfo.bpm = float.Parse(bpmInput.text);

            SetTimeLine();
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
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.DefaultExt = "json"; // JSON ���ϸ� ���͸�
        saveFileDialog.Title = "Select a JSON File"; // Ž���� Ÿ��Ʋ
        saveFileDialog.Filter = "JSON files (*.json)|*.json"; // JSON ���ϸ� ���͸�

        if (saveFileDialog.ShowDialog() == DialogResult.OK) //�������� ��
        {
            string filePath = saveFileDialog.FileName; //���� ��� ������

            string json = JsonUtility.ToJson(mapInfo, true); //�� ������ JSON���� ����

            File.WriteAllText(filePath, json); //������ ��ο� JSON���� ����
        }
    }

    public void OnClickLoadLevel()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "JSON files (*.json)|*.json"; // JSON ���ϸ� ���͸�
        openFileDialog.Title = "Select a JSON File"; // Ž���� Ÿ��Ʋ

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string filePath = openFileDialog.FileName;
                string json = File.ReadAllText(filePath);

                JsonUtility.FromJsonOverwrite(json, mapInfo);
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON ���� ������ �ùٸ��� �ʰų�, �ٸ� ������ �����մϴ�.\n{e}");
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
            Debug.LogError("���� ���� ����");
            return;
        }

        if(mapInfo.bpm == 0)
        {
            Debug.LogError("BPM ���� �ȵ�");
            return;
        }

        int timeLineLength = Mathf.FloorToInt(mapInfo.backgroundMusic.length * mapInfo.bpm / 60 / 4);

        for(int i=0; i<timeLineLength; i++)
        {
            if (timeLineParent.childCount > timeLineLength) return; //���� �ִ� ������� �䱸 ��������� ������

            if (timeLineParent.childCount > i) continue;

            GameObject newTimeLine = Instantiate(timeLinePrefab, timeLineParent);
            newTimeLine.transform.Find("Index").GetComponent<TMP_Text>().text = $"{i}";
        }
    }
}
