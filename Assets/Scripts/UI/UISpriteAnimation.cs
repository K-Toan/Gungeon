using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public float FrameRate = 5;

    private float timer;
    private int index = 0;
    private Toggle toggle;
    private Image image;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (!toggle.isOn)
            return;

        timer += Time.deltaTime;
        if (timer > 1 / FrameRate)
        {
            timer = 0f;
            image.sprite = sprites[index];
            index = (index + 1) % sprites.Length;
        }
    }
}
