using UnityEngine;
using UnityEngine.UI;

public class ExpUI : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Text expText;
    
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

        // Ensure the expText is assigned
        if (expText == null)
        {
            expText = GetComponentInChildren<Text>();
            if (expText == null)
            {
                Debug.LogError("Text component is not assigned and could not be found in children.");
                return;
            }
        }

        gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];

        colorKeys[0] = new GradientColorKey(ColorUtility.TryParseHtmlString("#478CCF", out Color color) ? color : Color.red, 0f);
        colorKeys[1] = new GradientColorKey(ColorUtility.TryParseHtmlString("#478CCF", out color) ? color : new Color(1f, 0.513f, 0f), 0.33f);
        colorKeys[2] = new GradientColorKey(ColorUtility.TryParseHtmlString("#478CCF", out color) ? color : new Color(0.478f, 1f, 0f), 0.66f);
        colorKeys[3] = new GradientColorKey(ColorUtility.TryParseHtmlString("#478CCF", out color) ? color : new Color(0.094f, 1f, 0f), 1f);

        alphaKeys[0] = new GradientAlphaKey(1.0f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 0.25f);
        alphaKeys[2] = new GradientAlphaKey(1.0f, 0.75f);
        alphaKeys[3] = new GradientAlphaKey(1.0f, 1f);

        gradient.SetKeys(colorKeys, alphaKeys);

        slider.value = 0;
        fill.color = gradient.Evaluate(1f);
        UpdateExpText(1, slider.value, slider.maxValue);
    }

    public void SetExp(int level, float currentExp, float maxExp)
    {
        if (slider != null && fill != null)
        {
            slider.maxValue = maxExp;
            slider.value = currentExp;
            UpdateExpText(level, currentExp, slider.maxValue);
        }
    }

    public void SetMaxExp(int level, float currentExp, float maxExp)
    {
        if (slider != null && fill != null)
        {
            slider.maxValue = maxExp;
            slider.value = currentExp;
            // fill.color = gradient.Evaluate(1f);
            UpdateExpText(level, currentExp, slider.maxValue);
        }
    }

    private void UpdateExpText(int level, float currentExp, float maxExp)
    {
        if (expText != null)
        {
            expText.text = "Lvl " + level + ": " + currentExp + "/" + maxExp;
        }
    }
}
