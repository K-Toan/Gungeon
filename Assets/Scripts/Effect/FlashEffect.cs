using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashEffect : MonoBehaviour
{
    [Header("Stats")]
    public float FlashDuration = 0.05f;

    [Header("Color")]
    public Color FlashColor = Color.red;
    private Color _originalColor;

    [Header("Components")]
    public SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _originalColor = _spriteRenderer.color;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Flash();
        }
    }

    public void Flash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        _spriteRenderer.color = FlashColor;

        yield return new WaitForSeconds(FlashDuration);

        _spriteRenderer.color = _originalColor;
    }
}
