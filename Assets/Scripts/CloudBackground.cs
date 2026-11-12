using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CloudBackground : MonoBehaviour
{
    [SerializeField] private GameObject[] cloudPrefabs;
    [SerializeField] private int cloudCount = 7;

    private RectTransform canvasRect;

    private void Awake()
    {
        canvasRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (cloudPrefabs == null || cloudPrefabs.Length == 0)
        {
            Debug.LogError("No cloud prefabs have been assigned.", this);
            return;
        }

        float halfWidth = canvasRect.rect.width * 0.5f;
        float halfHeight = canvasRect.rect.height * 0.5f;
        for (int i = 0; i < cloudCount; i++)
        {
            GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
            if (prefab == null)
            {
                Debug.LogError("A cloud prefab slot is empty.", this);
                continue;
            }

            GameObject cloud = Instantiate(prefab, transform, false);
            RectTransform cloudRect = cloud.GetComponent<RectTransform>();
            CloudDrift drift = cloud.GetComponent<CloudDrift>();
            if (cloudRect == null || drift == null)
            {
                Debug.LogError("Cloud prefabs must have a RectTransform and CloudDrift component.", cloud);
                Destroy(cloud);
                continue;
            }

            float scale = Random.Range(0.65f, 1.15f);
            cloudRect.localScale = Vector3.one * scale;
            float cloudHalfWidth = cloudRect.rect.width * scale * 0.5f;
            float positionX = Mathf.Lerp(
                -halfWidth - cloudHalfWidth,
                halfWidth + cloudHalfWidth,
                (i + Random.value) / cloudCount);
            float positionY = Random.Range(-halfHeight * 0.4f, halfHeight * 0.4f);
            cloudRect.anchoredPosition = new Vector2(positionX, positionY);
            cloudRect.SetSiblingIndex(i);
            drift.Initialize(canvasRect, Random.Range(0.55f, 1.25f), Random.value < 0.5f ? -1 : 1);
        }
    }
}
