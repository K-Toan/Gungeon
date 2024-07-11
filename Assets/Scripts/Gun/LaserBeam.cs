using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Stats")]
    public float Damage = 0f;
    public LayerMask CollisionLayer;

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserLength, CollisionLayer);

        if (hit.collider != null)
        {
            endPosition = hit.point;
            distance = Vector2.Distance(startPosition, endPosition);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemy = hit.collider.transform.gameObject.GetComponent<EnemyController>();
                enemy.TakeDamage(Damage * Time.deltaTime, transform.position);
            }
        }

        Vector2 middlePosition = (startPosition + endPosition) / 2;

        LaserMiddle.GetComponent<SpriteRenderer>().size = new Vector2(distance, 1);

        LaserStart.transform.position = startPosition;
        LaserMiddle.transform.position = middlePosition;
        LaserEnd.transform.position = endPosition;
    }
}