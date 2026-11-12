using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{

    public void ExitGame()
    {
        StartCoroutine(Exit());
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.8f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
