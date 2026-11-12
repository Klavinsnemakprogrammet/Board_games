using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class CloudDrift : MonoBehaviour
{
    private const float MaximumSpeed = 160f;

    private RectTransform cloudRect;
    private RectTransform movementBounds;
    private float speedMultiplier;
    private int direction;

    private void Awake()
    {
        cloudRect = GetComponent<RectTransform>();
        GetComponent<Image>().raycastTarget = false;
    }

    public void Initialize(RectTransform bounds, float speedFactor, int travelDirection)
    {
        movementBounds = bounds;
        speedMultiplier = speedFactor;
        direction = travelDirection;
    }

    private void Update()
    {
        if (movementBounds == null)
        {
            return;
        }

        float halfWidth = movementBounds.rect.width * 0.5f;
        float halfHeight = movementBounds.rect.height * 0.5f;
        float cloudHalfWidth = cloudRect.rect.width * cloudRect.localScale.x * 0.5f;
        Vector2 position = cloudRect.anchoredPosition;
        position.x += direction * MaximumSpeed * GameCloudSettings.Speed * speedMultiplier * Time.deltaTime;

        if (direction > 0 && position.x - cloudHalfWidth > halfWidth)
        {
            position.x = -halfWidth - cloudHalfWidth;
            position.y = Random.Range(-halfHeight * 0.4f, halfHeight * 0.4f);
        }
        else if (direction < 0 && position.x + cloudHalfWidth < -halfWidth)
        {
            position.x = halfWidth + cloudHalfWidth;
            position.y = Random.Range(-halfHeight * 0.4f, halfHeight * 0.4f);
        }

        cloudRect.anchoredPosition = position;
    }
}
