using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboTextMove : MonoBehaviour
{
    private TMP_Text textMesh;

    private Mesh mesh;

    private Vector3[] vertices;

    private void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Wobble(Time.time + i);

            vertices[i] = vertices[i] + offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 9.9f), Mathf.Cos(time * 7.5f));
    }
}
