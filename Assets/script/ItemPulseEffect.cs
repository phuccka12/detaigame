using UnityEngine;

public class ItemPulseEffect : MonoBehaviour
{
    public float pulseSpeed = 1f;
    public float minBrightness = 0.8f;
    public float maxBrightness = 1f;

    private SpriteRenderer spriteRenderer;
    private float currentBrightness;
    private float pulseDirection = 1f; // 1 là tăng, -1 là giảm

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentBrightness = maxBrightness;
    }

    void Update()
    {
        currentBrightness += pulseDirection * pulseSpeed * Time.deltaTime;

        if (currentBrightness > maxBrightness)
        {
            currentBrightness = maxBrightness;
            pulseDirection = -1f;
        }
        else if (currentBrightness < minBrightness)
        {
            currentBrightness = minBrightness;
            pulseDirection = 1f;
        }

        Color currentColor = spriteRenderer.color;
        spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentBrightness);
    }
}