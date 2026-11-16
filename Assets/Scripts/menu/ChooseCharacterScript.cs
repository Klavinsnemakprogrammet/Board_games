using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ChooseCharacterScript : MonoBehaviour
{
    public GameObject[] characters;
    int characterIndex;

    public GameObject inputField;
    string characterName;
    public int playerCount = 3;
    public SceneChangeScript sceneChangeScript;


    void Awake()
    {
        characterIndex = 0;
        foreach (GameObject character in characters)
        {
            character.SetActive(false);

        }
        characters[characterIndex].SetActive(true);
    }

    public void NextCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex++;
        if (characterIndex == -1 ){
            characterIndex = characters.Length - 1;
    }
    characters[characterIndex].SetActive(true);
    }
    public void PreviousCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex--;
        if (characterIndex == -1 ){
            characterIndex = characters.Length - 1;
    }
    characters[characterIndex].SetActive(true);
    }
    public void Play(){
    characterName = inputField.GetComponent<TMP_InputField>().text;
    if(characterName.Length > 2)
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.SetString("PlayerName", characterName);
        PlayerPrefs.SetInt("PlayerCount", playerCount);
        // StartCourtine(
        //     SceneChangeScript.Delay
    
        // )
    }
    }

}
