using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    public Sprite[] Sprites;
    public float FrameRate = 5;
    public int CharacterIndex;

    private float timer;
    public int index = 0;
    public int startIndex;
    public int endIndex;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        CharacterIndex = PlayerPrefs.GetInt("CharIndex", 1);
        startIndex = (CharacterIndex - 1) * 4;
        endIndex = CharacterIndex * 4;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 / FrameRate)
        {
            timer = 0f;
            image.sprite = Sprites[index + startIndex];
            index = (index + 1) % 4;
        }
    }
}
