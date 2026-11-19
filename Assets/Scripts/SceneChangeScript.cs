using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, "", -1));
    }
   public IEnumerator Delay(string action, int charIndex, string playerName, int playerCount)
    {
        if (string.Equals(action, "quit", StringComparison.OrdinalIgnoreCase))
        {
            yield return new WaitForSeconds(0.8f);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            yield return fadeScript.FadeOut(0.3f);
            Application.Quit();
#endif


        }else if(string.Equals(action, "play", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.3f);
            saveLoadScript.SavePlayerData(charIndex, playerName, playerCount);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        } else if(string.Equals(action, "menu", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.3f);
            SceneManager.LoadScene(0, LoadSceneMode.Single);


        }else if(string.Equals(action, "settings", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.3f);
            saveLoadScript.SavePlayerData(charIndex, playerName, playerCount);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        
    }
    public void ToMainMenu()
    {
        StartCoroutine(Delay("menu", -1, "", -1));
    }

    public void ExitGame()
    {
        StartCoroutine(Exit());
    }

    public void LoadSettings()
    {
        StartCoroutine(LoadSceneAfterFade("SettingsScene"));
    }

    public void LoadLeaderboard()
    {
        StartCoroutine(LoadSceneAfterFade("LeaderboardScene"));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneAfterFade("MainMenu"));
    }

    private IEnumerator LoadSceneAfterFade(string sceneName)
    {
        if (fadeScript == null)
        {
            Debug.LogError("Cannot load scene because no FadeScript is assigned.", this);
            yield break;
        }

        yield return fadeScript.FadeOut(0.3f);
        SceneManager.LoadScene(sceneName);
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
