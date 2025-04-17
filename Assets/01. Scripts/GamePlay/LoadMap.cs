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
            openFileDialog.Filter = "JSON files (*.json)|*.json"; // JSON ���ϸ� ���͸�
            openFileDialog.Title = "Select a JSON File"; // Ž���� Ÿ��Ʋ

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
                    Debug.LogError($"JSON ���� ������ �ùٸ��� �ʰų�, �ٸ� ������ �����մϴ�.\n{e}");
                }
            }
        }
    }
}
