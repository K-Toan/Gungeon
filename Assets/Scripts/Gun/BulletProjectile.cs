using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [Header("Stats")]
    public float Damage = 0f;

    [Header("Components")]
    [SerializeField] private bool _hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _hitbox;

    private int _hitHash;

    private void Start()
    {
        _hitbox = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _hasAnimator = TryGetComponent<Animator>(out _animator);

        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _hitHash = Animator.StringToHash("Hit");
    }

    private void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemy = other.transform.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(Damage, _rigidbody.velocity.normalized);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.transform.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(Damage, _rigidbody.velocity.normalized);
        }
        Hit();
    }

    public void Hit()
    {
        _hitbox.enabled = false;
        _rigidbody.velocity = Vector2.zero;
        if (_hasAnimator)
        {
            _animator.SetTrigger(_hitHash);
            Destroy(gameObject, 0.5f);
        }
    }
    
    public void IgnoreCollision(Collider2D collider)
    {
        Physics2D.IgnoreCollision(transform.GetComponent<Collider2D>(), collider);
    }
}
