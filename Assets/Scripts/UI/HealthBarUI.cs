using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Text healthText;

    void Start()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
        }

        // Set the slider to use whole numbers
        slider.wholeNumbers = true;

        // Set the maximum value to 100
        slider.maxValue = 100;

        // Set the direction to Left To Right
        slider.direction = Slider.Direction.LeftToRight;

        slider.transition = Selectable.Transition.None;
        Navigation navigation = new Navigation { mode = Navigation.Mode.None };
        slider.navigation = navigation;

        // Find and set the Fill Rect dynamically
        if (slider.fillRect == null)
        {
            Transform fillTransform = slider.transform.Find("Fill");
            if (fillTransform != null)
            {
                slider.fillRect = fillTransform.GetComponent<RectTransform>();
            }
        }

        // Ensure the fill Image is assigned
        if (fill == null)
        {
            fill = slider.fillRect.GetComponent<Image>();
            if (fill == null)
            {
                Debug.LogError("Fill Image is not assigned and could not be found in the fillRect.");
                return;
            }
        }

        // Ensure the healthText is assigned
        if (healthText == null)
        {
            healthText = GetComponentInChildren<Text>();
            if (healthText == null)
            {
                Debug.LogError("Text component is not assigned and could not be found in children.");
                return;
            }
        }

        gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];

        colorKeys[0] = new GradientColorKey(ColorUtility.TryParseHtmlString("#FF0000", out Color color) ? color : Color.red, 0f);
        colorKeys[1] = new GradientColorKey(ColorUtility.TryParseHtmlString("#FF8400", out color) ? color : new Color(1f, 0.513f, 0f), 0.33f);
        colorKeys[2] = new GradientColorKey(ColorUtility.TryParseHtmlString("#7AFF00", out color) ? color : new Color(0.478f, 1f, 0f), 0.66f);
        colorKeys[3] = new GradientColorKey(ColorUtility.TryParseHtmlString("#18FF00", out color) ? color : new Color(0.094f, 1f, 0f), 1f);

        alphaKeys[0] = new GradientAlphaKey(1.0f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 0.25f);
        alphaKeys[2] = new GradientAlphaKey(1.0f, 0.75f);
        alphaKeys[3] = new GradientAlphaKey(1.0f, 1f);

        gradient.SetKeys(colorKeys, alphaKeys);

        slider.value = slider.maxValue;
        fill.color = gradient.Evaluate(1f);
        UpdateHealthText(slider.value, slider.maxValue);
    }

    public void SetHealth(int health)
    {
        if (slider != null && fill != null)
        {
            slider.value = health;
            fill.color = gradient.Evaluate(slider.normalizedValue);
            UpdateHealthText(health, slider.maxValue);
        }
    }

    public void SetMaxHealth(int health)
    {
        if (slider != null && fill != null)
        {
            slider.maxValue = health;
            slider.value = health;
            fill.color = gradient.Evaluate(1f);
            UpdateHealthText(health, health);
        }
    }

    private void UpdateHealthText(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + "/" + maxHealth;
        }
    }
}
