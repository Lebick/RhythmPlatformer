using UnityEngine;
using Alchemy.Inspector;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class LoadMap : MonoBehaviour
{
    [DisableIf(nameof(isScriptableObject))] public bool isJson;

    [DisableIf(nameof(isJson))] public bool isScriptableObject;
    [ShowIf(nameof(isScriptableObject))] public MapInfo mapInfo;

    public PlayerPosition player;

    private void Awake()
    {
        if (isJson)
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

                    MapInfo tempInfo = ScriptableObject.CreateInstance<MapInfo>();

                    JsonUtility.FromJsonOverwrite(json, tempInfo);

                    GamePlayManager.instance.mapInfo = tempInfo;

                    string prefabPath = $"Assets/03. MapDatas/Enviorments/{Path.GetFileNameWithoutExtension(openFileDialog.FileName)}.prefab";
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    GameObject newEnviorment = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                    player.SetRenderering();
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON 내부 형식이 올바르지 않거나, 다른 오류가 존재합니다.\n{e}");
                }
            }
        }
    }
}
