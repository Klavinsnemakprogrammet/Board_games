using TMPro;
using UnityEngine;

public class NameScript : MonoBehaviour
{
    TMP_Text nameText;

    void Awake()
    {
        nameText = transform.Find("NameField")?.GetComponent<TMP_Text>();
        if (nameText == null)
        {
            Debug.LogError("NameField was not found on this player.", this);
        }
    }

    public void SetPlayerName(string name)
    {
        if (nameText == null)
        {
            return;
        }

        nameText.text = name;
        nameText.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }
}
