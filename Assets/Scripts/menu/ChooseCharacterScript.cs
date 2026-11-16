using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacterScript : MonoBehaviour
{
    public GameObject[] characters;
    int characterIndex;

    public GameObject inputField;
    string characterName;
    public int playerCount = 3;
    public SceneChangeScript sceneChangeScript;
    [SerializeField] private Slider playerCountSlider;
    [SerializeField] private TMP_Text playerCountText;


    void Awake()
    {
        characterIndex = 0;
        foreach (GameObject character in characters)
        {
            character.SetActive(false);

        }
        characters[characterIndex].SetActive(true);

        if (playerCountSlider == null)
        {
            Debug.LogError("The player count slider has not been assigned.", this);
        }
        else
        {
            playerCountSlider.onValueChanged.AddListener(SetPlayerCount);
            SetPlayerCount(playerCountSlider.value);
        }

        if (playerCountText == null)
        {
            Debug.LogError("The player count text has not been assigned.", this);
        }
    }

    void OnDestroy()
    {
        if (playerCountSlider != null)
        {
            playerCountSlider.onValueChanged.RemoveListener(SetPlayerCount);
        }
    }

    void SetPlayerCount(float value)
    {
        playerCount = Mathf.RoundToInt(value);
        if (playerCountText != null)
        {
            playerCountText.text = $"Player Count: {playerCount}";
        }
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
