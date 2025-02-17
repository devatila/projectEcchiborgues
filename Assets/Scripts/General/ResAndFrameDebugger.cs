using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResAndFrameDebugger : MonoBehaviour
{
    private float fps;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateFPS), 1f, 1f); // Atualiza o FPS a cada 1 segundo
    }

    private void UpdateFPS()
    {
        fps = 1.0f / Time.deltaTime;
    }

    private void OnGUI()
    {
        GUILayout.Label("Resolução: " + Screen.width + "x" + Screen.height);
        GUILayout.Label("FPS: " + fps.ToString("F2"));
    } // Fui descansar um poko...



    /*private void OnGUI()
    {
        GUILayout.Label("Resolução: " + Screen.width + "x" + Screen.height);
        GUILayout.Label("FPS: " + (1.0f / Time.deltaTime).ToString("F2"));
    }*/
}
