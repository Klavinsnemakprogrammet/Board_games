using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{

    public void ExitGame()
    {
        StartCoroutine(Exit());
    }
    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.8f);
        if(UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
