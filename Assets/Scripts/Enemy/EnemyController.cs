using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float HP = 1f;

    [Header("Move")]
    public float MoveSpeed = 1f;
    public Vector2 MoveDirection;
    [SerializeField] private bool canMove = true;

    [Header("Rotation")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 gunDirection;

    [Header("Visuals")]
    [SerializeField] private bool _hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Gun")]
    [SerializeField] private bool _hasGun;

    [Header("Effects")]
    [SerializeField] private FlashEffect _flashEffect;
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Game Objects")]
    public GameObject Target;
    public GameObject Visual;
    public GameObject OneHand;
    public GameObject TwoHands;
    public GameObject Gun;
    public GameObject ParticleRoot;

    // [Header("Animation Hash IDs")]
    private int _speedHash;
    private int _speedXHash;
    private int _speedYHash;
    private int _hitHash;
    private int _dieHash;

    private void Start()
    {
        // game objects
        if(!Target)
        {
            Target = GameObject.Find("player");
        }
        Visual = transform.Find("Visual").gameObject;
        OneHand = transform.Find("OneHand").gameObject;
        TwoHands = transform.Find("TwoHands").gameObject;
        Gun = OneHand.transform.Find("Gun").gameObject ?? TwoHands.transform.Find("Gun").gameObject;

        // components
        _hasAnimator = Visual.TryGetComponent<Animator>(out _animator);
        _rigidbody = GetComponent<Rigidbody2D>();
        _flashEffect = GetComponent<FlashEffect>();

        // gun
        if (Gun != null)
        {
            _hasGun = true;
        }

        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _speedHash = Animator.StringToHash("Speed");
        // _speedXHash = Animator.StringToHash("SpeedX");
        _speedYHash = Animator.StringToHash("SpeedY");
        _hitHash = Animator.StringToHash("Hit");
        _dieHash = Animator.StringToHash("Die");
    }

    private void Update()
    {
        HandleRotation();
        HandleGun();
        Move();

        HandleAnimations();
        HandleFlipX();
    }

    private void HandleRotation()
    {
        if (Target != null)
        {
            // character rotation
            lookDirection = (Target.transform.position - transform.position).normalized;

            // gun rotation
            if (_hasGun)
            {
                gunDirection = (Target.transform.position - Gun.transform.position).normalized;
                Gun.transform.right = gunDirection;
            }
        }
    }

    private void HandleGun()
    {
        if (!_hasGun)
            return;

        if (Target != null)
        {
            Gun.GetComponent<GunController>().SetFiring(true);
        }
    }

    private void Move()
    {
        if (!canMove)
            return;

        if (Target != null)
        {
            float distance = (Target.transform.position - transform.position).magnitude;
            float currentMoveSpeed = 0f;
            if (distance > 4)
            {
                currentMoveSpeed = MoveSpeed;
            }
            _rigidbody.velocity = lookDirection * currentMoveSpeed;
        }
    }

    private void HandleAnimations()
    {
        if (_hasAnimator)
        {
            // _animator.SetFloat(_speedXHash, lookDirection.x);
            _animator.SetFloat(_speedYHash, lookDirection.y);
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

    public void Die()
    {
        OneHand.SetActive(false);
        _animator.SetTrigger(_dieHash);
        // canMove = false;
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
