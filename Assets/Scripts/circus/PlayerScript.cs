using UnityEngine;
using System.IO;
public class PlayerScript : MonoBehaviour
{

    int characterIndex;
    public GameObject[] playerPrefabs;
    public GameObject spawnPoint;
    int[] otherPlayers;
    int otherPlayersIndex;
    private const string playerNameTxt = "playernames"; 
    
    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogError("No player prefabs have been assigned.", this);
            return;
        }

        characterIndex = Mathf.Clamp(characterIndex, 0, playerPrefabs.Length - 1);
        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex]);
        PlaceCharacter(mainCharacter, 0);
        NameScript mainName = mainCharacter.GetComponent<NameScript>();
        if (mainName != null)
        {
            mainName.SetPlayerName(PlayerPrefs.GetString("PlayerName", "PlayerName"));
        }

        otherPlayers = new int[PlayerPrefs.GetInt("PlayerCount")];     
        string[] nameArray = ReadLinesFromFile(playerNameTxt);

        for (int i=0; i<otherPlayers.Length-1; i++)
        {
            otherPlayersIndex = Random.Range(0, playerPrefabs.Length);
            GameObject character = Instantiate(playerPrefabs[otherPlayersIndex]);
            PlaceCharacter(character, i + 1);

            NameScript characterName = character.GetComponent<NameScript>();
            if (characterName != null && nameArray.Length > 0)
            {
                characterName.SetPlayerName(nameArray[Random.Range(0, nameArray.Length)]);
            }
        }
    }

    void PlaceCharacter(GameObject character, int positionIndex)
    {
        RectTransform characterRect = character.GetComponent<RectTransform>();
        if (characterRect != null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                characterRect.SetParent(canvas.transform, false);
                characterRect.anchoredPosition = new Vector2(-300f + (positionIndex * 150f), -20f);
                characterRect.localScale = Vector3.one;
                return;
            }
        }

        if (spawnPoint != null)
        {
            character.transform.position = spawnPoint.transform.position + new Vector3(positionIndex * 0.2f, 0.1f, 0.08f);
        }
        else
        {
            Debug.LogError("The spawn point has not been assigned.", this);
        }
    }

    string[] ReadLinesFromFile(string playerNameTxt)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(playerNameTxt);
        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("Failed to load player names from : " + playerNameTxt);
            return new string[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
