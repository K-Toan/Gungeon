using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float HP = 100f;
    public float Damage = 10f;
    [SerializeField] private bool IsDead = false;
    [SerializeField] private bool hasGun;
    private float attackTime = 1f;
    private float nextAttackTime = 1f;
    private float distanceToPlayer;

    [Header("Move")]
    public float MoveSpeed = 2f;
    public Vector2 MoveDirection;
    [SerializeField] private bool canMove = true;
    public float StopRange = 0f;

    [Header("Rotation")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 gunDirection;

    [Header("Components")]
    private Collider2D _collider;
    [SerializeField] private bool _hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Scripts")]
    [SerializeField] private FlashEffect _flashEffect;
    [SerializeField] private GunController _gunController;

    [Header("Game Objects")]
    public GameObject Hand;
    public GameObject Target;
    public GameObject GunRoot;
    public GameObject Gun;

    // [Header("Animation Hash IDs")]
    private int _lookXHash;
    private int _lookYHash;
    private int _speedHash;
    private int _speedXHash;
    private int _speedYHash;
    private int _hitHash;
    private int _dieHash;

    private void Start()
    {
        // gun
        Hand = transform.Find("Hand").gameObject;
        GunRoot = transform.Find("GunRoot").gameObject;
        if (GunRoot.transform.childCount > 0)
        {
            Gun = GunRoot.transform.GetChild(0).gameObject;
        }
        if (Gun != null)
        {
            hasGun = true;
            _gunController = Gun.GetComponent<GunController>();
        }

        // components
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _flashEffect = GetComponent<FlashEffect>();

        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _lookXHash = Animator.StringToHash("LookX");
        _lookYHash = Animator.StringToHash("LookY");
        _speedHash = Animator.StringToHash("Speed");
        _speedXHash = Animator.StringToHash("SpeedX");
        _speedYHash = Animator.StringToHash("SpeedY");
        _hitHash = Animator.StringToHash("Hit");
        _dieHash = Animator.StringToHash("Die");
    }

    private void Update()
    {
        if (!IsDead)
        {
            if (TargetExists())
            {
                HandleRotate();
                HandleGun();
                Move();
            }

            HandleAnimations();
            HandleFlipX();
        }
    }

    private bool TargetExists()
    {
        // game objects
        Target = GameManager.Instance.Player;
        if (Target != null)
        {
            return true;
        }
        return false;
    }

    private void HandleRotate()
    {
        // character rotation
        lookDirection = (Target.transform.position - transform.position).normalized;
    }

    private void HandleGun()
    {
        if (!hasGun)
            return;

        bool fire;

        if (nextAttackTime > 0)
        {
            nextAttackTime -= Time.deltaTime;
            fire = false;
        }
        else
        {
            fire = true;
            nextAttackTime = attackTime + Random.Range(0.4f, 0.8f);
        }

        Vector2 directionToPlayer = (Target.transform.position - GunRoot.transform.position).normalized;

        _gunController.HandleInput(fire, false, Target.transform.position);
    }


    private void Move()
    {
        if (!canMove)
            return;

        distanceToPlayer = Vector2.Distance(Target.transform.position, transform.position);
        if (distanceToPlayer > StopRange)
        {
            _rigidbody.velocity = lookDirection * MoveSpeed;
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void HandleAnimations()
    {
        if (_hasAnimator)
        {
            _animator.SetFloat(_lookXHash, lookDirection.x);
            _animator.SetFloat(_lookYHash, lookDirection.y);
            _animator.SetFloat(_speedXHash, _rigidbody.velocity.x);
            _animator.SetFloat(_speedYHash, _rigidbody.velocity.y);
            _animator.SetFloat(_speedHash, _rigidbody.velocity.magnitude);
        }
    }

    private void HandleFlipX()
    {
        if (lookDirection.x > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (lookDirection.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public void TakeDamage(float dmg, Vector2 dmgDir)
    {
        _flashEffect.Flash();
        HP -= dmg;
        StartCoroutine(DisableMove(0.4f));
        StartCoroutine(MoveTowards(dmgDir, 0.05f));
        if (HP > 0)
        {
            _animator.SetTrigger(_hitHash);
        }
        else
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.transform.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(Damage, _rigidbody.velocity * 2);
        }
    }

    public void Die()
    {
        GunRoot.SetActive(false);
        IsDead = true;
        _collider.enabled = false;
        _animator.SetTrigger(_dieHash);
        Destroy(gameObject, 1f);
    }

    public IEnumerator MoveTowards(Vector2 dir, float time)
    {
        _rigidbody.velocity = dir;
        yield return new WaitForSeconds(time);
        _rigidbody.velocity = Vector2.zero;
    }

    public IEnumerator DisableMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
