using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test2 : MonoBehaviour
{
    public void LoaddsLevel()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }
}
