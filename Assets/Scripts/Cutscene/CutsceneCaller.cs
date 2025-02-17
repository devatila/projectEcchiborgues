using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCaller : MonoBehaviour
{
    public string cutsceneObjectHandler;
    public void CallerFunction(int index)
    {
        CutsceneHandler c = GameObject.Find(cutsceneObjectHandler).GetComponent<CutsceneHandler>();
        c.CallerMoveToPoint(index);
    }
    
    public void CameraChangerPosition()
    {
        CutsceneHandler c = GameObject.Find(cutsceneObjectHandler).GetComponent<CutsceneHandler>();
        c.StartCameraTransition(1);
    }
}
