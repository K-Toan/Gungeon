using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Stats")]
    public float Damp = 10f;
    public float ShakeDuration = 0.2f;
    public float ShakeMagnitude = 0.1f;
    public float ShakeIntensity = 0.1f;

    [Header("Followed target")]
    public GameObject Player;

    [Header("Debug")]
    [SerializeField] private Vector3 playerPos;
    [SerializeField] private Vector3 mousePos;
    [SerializeField] private Vector3 targetPos;

    private void Start()
    {
        if(Player == null)
            Player = GameObject.Find("Player");
    }

    private void LateUpdate()
    {
        playerPos = Player.transform.position;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // move camera towards player aim position
        targetPos = new Vector3(playerPos.x + (mousePos.x - playerPos.x) / 5,
                                playerPos.y + (mousePos.y - playerPos.y) / 5,
                                -10f);

        transform.position = Vector3.Lerp(transform.position, targetPos, Damp * Time.deltaTime);
    }

    public void Shake(Vector2 shakeDir)
    {
        StartCoroutine(ShakeCoroutine(shakeDir));
    }

    private IEnumerator ShakeCoroutine(Vector2 shakeDir)
    {
        Vector3 shakePos =  new Vector3(targetPos.x + shakeDir.x, targetPos.y + shakeDir.y, targetPos.z);

        transform.position = Vector3.Lerp(transform.position, shakePos, ShakeIntensity);

        yield return new WaitForSeconds(ShakeDuration);
    }
}