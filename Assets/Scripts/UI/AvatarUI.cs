using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    public Sprite[] Sprites;
    public float FrameRate = 5;
    public int CharacterIndex = 1;

    private float timer;
    private int index = 0;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 / FrameRate)
        {
            timer = 0f;
            image.sprite = Sprites[index + (CharacterIndex - 1) * 4];
            index = (index + 1 + (CharacterIndex - 1) * 4) % 4;
        }
    }
}
