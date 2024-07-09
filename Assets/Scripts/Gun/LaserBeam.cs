using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Laser pieces")]
    public GameObject LaserStart;
    public GameObject LaserMiddle;
    public GameObject LaserEnd;
    
    private float laserLength = 20f;

    private void Update()
    {
        float distance = laserLength;
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + (Vector2)transform.right * laserLength;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserLength);

        if (hit.collider != null)
        {
            endPosition = hit.point;
            distance = Vector2.Distance(startPosition, endPosition);
        }

        Vector2 middlePosition = (startPosition + endPosition) / 2;

        LaserMiddle.GetComponent<SpriteRenderer>().size = new Vector2(distance, 1);

        LaserStart.transform.position = startPosition;
        LaserMiddle.transform.position = middlePosition;
        LaserEnd.transform.position = endPosition;
    }
}