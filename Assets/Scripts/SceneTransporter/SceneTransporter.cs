using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransporter : MonoBehaviour
{
    public void Loadlevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
