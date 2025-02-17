using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManagerScene : MonoBehaviour
{
    //[SerializeField] private bool InCutscene = false;

    public static CutSceneManagerScene Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayCutscene()
    {
        TypewriterEffectTMP.stopAll();
    }
}
