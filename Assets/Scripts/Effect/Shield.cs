using UnityEngine;

public class Shield : MonoBehaviour
{
    public float ExistTime = 0.85f;
    public float StartRadius = 0.5f;
    public float MaxRadius = 4.5f;

    private CircleCollider2D _collider;

    private float timer = 0f;

    private void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.radius = StartRadius;
        _collider.isTrigger = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = timer / ExistTime;

        _collider.radius = Mathf.Lerp(StartRadius, MaxRadius, t);

        if (timer >= ExistTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            var bullet = other.gameObject.GetComponent<BulletProjectile>();
            if (bullet != null)
            {
                bullet.Hit();
            }
        }
    }
}
