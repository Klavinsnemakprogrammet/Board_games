
using UnityEngine;

public class JiggleScript : MonoBehaviour
{
    [Range(0, 1)]
    public float power = 0.1f;

    [Header("Position jiggle")]
    public bool jigPosition = true; 
    public Vector3 positionJigAmount;
    [Range(0, 120)]
    public float positionFrequency = 10f;
    float positionTime;

    [Header("Rotation Jiggler")]
    public bool jigRotation = true; 
    public Vector3 rotationJigAmount;
    [Range(0, 120)]
    public float rotationFrequency = 10f;
     
     [Header("Scale Jiggler")]
    public bool jigScale = true; 
    public Vector3 ScaleJigAmount;
    [Range(0, 120)]
    public float ScaleFrequency = 10f;
    float ScaleTime;
    float rotationTime;

    Vector3 basePosition;
    Quaternion baseRotation;
    Vector3 baseScale;

    void Start()
    {
        basePosition = transform.localPosition;     
        baseRotation = transform.localRotation;    
        baseScale = transform.localScale;
    }

    
    void Update()
    {
        var dt = Time.unscaledDeltaTime;
        positionTime += dt * positionFrequency;

        if(jigPosition)
        {
            transform.localPosition = basePosition + positionJigAmount * Mathf.Sin(positionTime) * power;
        }
        rotationTime += dt * rotationFrequency;
        if(jigRotation)
        {
            transform.localRotation = baseRotation * Quaternion.Euler(rotationJigAmount * Mathf.Sin(rotationTime) * power);
        }
        ScaleTime += dt * ScaleFrequency;
        if(jigScale)
        {
             transform.localScale = baseScale + ScaleJigAmount * Mathf.Sin(ScaleTime) * power;
        }


    }
}
