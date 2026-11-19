using System.IO;
using UnityEngine;

public class SaveLoadScript : MonoBehaviour
{
    public string saveFileName = "playerData.json";

    [System.Serializable]
    public class GameData
    {
        public int characterIndex;
        public string characterName;
        public int playerCount;
    }

    private GameData gameData = new GameData();

    public void SavePlayerData(int characterIndex,
    string characterName, int playerCount)
    {
        gameData.characterIndex = characterIndex;
        gameData.characterName = characterName;
        gameData.playerCount = playerCount;
        string json = JsonUtility.ToJson(gameData);
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(filePath, json);
        Debug.Log("PlayerData saved to " + filePath);
    }

    public void LoadPlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if(File.Exists(filePath))
        {
             string json = File.ReadAllText(filePath);
             gameData = JsonUtility.FromJson<GameData>(json);
             Debug.Log("Player data loaded from " + filePath);
             Debug.Log(
                gameData.characterIndex + ", "
                + gameData.characterName + ", "
                + gameData.playerCount);
             

        }
        else
        {
            Debug.LogWarning("Save file not found at " + filePath);
        }
    }

}
