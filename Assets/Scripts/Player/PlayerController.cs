using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float HP = 2f;

    [Header("Move")]
    public float MoveSpeed = 3f;
    public float Acceleration = 20f;
    public Vector2 MoveDirection;
    public Vector2 LastMoveDirection;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool canMove = true;

    [Header("Dash/Dodge")]
    public float DashSpeed = 3f;
    public float DashTime = 0.75f;
    public float DashCooldownTime = 0.25f;
    [SerializeField] private Vector2 dashDirection;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;

    [Header("Rotation")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 gunDirection;

    [Header("Game Objects")]
    [SerializeField] private bool _hasGun;
    public GameObject Gun;
    public GameObject GunRoot;
    // public GameObject OneHand;
    // public GameObject TwoHands;
    // public GameObject Visual;
    public GameObject ParticleRoot;
    private GameObject hand;

    [Header("Components")]
    [SerializeField] private bool _hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GhostEffect _ghostEffect;
    [SerializeField] private FlashEffect _flashEffect;
    [SerializeField] private WeaponSystem _weaponSystem;
    [SerializeField] private InputSystem _input;
    [SerializeField] private Rigidbody2D _rigidbody;
    private Camera _mainCamera;

    [Header("Animation Hash IDs")]
    private int _speedHash;
    private int _speedXHash;
    private int _speedYHash;
    private int _dashHash;
    private int _dieHash;

    private void Start()
    {
        // game objects
        _mainCamera = Camera.main;
        // Visual = transform.Find("Visual").gameObject;
        // OneHand = transform.Find("OneHand").gameObject;
        // TwoHands = transform.Find("TwoHands").gameObject;

        // components
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostEffect = GetComponent<GhostEffect>();
        _flashEffect = GetComponent<FlashEffect>();
        _weaponSystem = GetComponent<WeaponSystem>();
        _input = GetComponent<InputSystem>();
        _rigidbody = GetComponent<Rigidbody2D>();

        AssignGun();
        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _speedHash = Animator.StringToHash("Speed");
        _speedXHash = Animator.StringToHash("SpeedX");
        _speedYHash = Animator.StringToHash("SpeedY");
        _dashHash = Animator.StringToHash("Dash");
        _dieHash = Animator.StringToHash("Die");
    }

    private void AssignGun()
    {
        Gun = _weaponSystem.GetWeapon();
        if (Gun != null)
        {
            _hasGun = true;
            if (Gun.GetComponent<GunController>().IsOneHand())
            {
                // OneHand.SetActive(true);
                // TwoHands.SetActive(false);
                // hand = OneHand;
                _animator.SetLayerWeight(1, 1f);
            }
            else if (Gun.GetComponent<GunController>().IsTwoHands())
            {
                // OneHand.SetActive(false);
                // TwoHands.SetActive(true);
                // hand = TwoHands;
                _animator.SetLayerWeight(1, 1f);
            }
            hand.transform.SetParent(transform);
            hand.SetActive(true);
            GunRoot = hand.transform.Find("GunRoot").gameObject;
            Gun.transform.SetParent(GunRoot.transform);
            Gun.transform.localPosition = Vector3.zero;
        }
        else
        {
            _hasGun = false;
        }
    }

    private void Update()
    {
        // handle logic parts
        HandleRotation();
        HandleDash();
        HandleGun();
        Move();

        // handle visuals
        HandleAnimations();
        HandleFlipX();
    }

    private void HandleRotation()
    {
        // mouse position
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // character rotation
        lookDirection = (mousePos - transform.position).normalized;

        // gun rotation
        if (_hasGun)
        {
            gunDirection = (mousePos - Gun.transform.position).normalized;
            hand.transform.up = gunDirection;
        }
    }

    private void HandleDash()
    {
        if (!canDash)
            return;

        if (_input.dash && _input.move != Vector2.zero)
        {
            // recalculate dash direction
            dashDirection = _input.move.normalized;

            StartCoroutine(DisableMovementRoutine(DashTime));
            StartCoroutine(DashRoutine(dashDirection));

            // animations
            if (_hasAnimator)
            {
                _animator.SetTrigger(_dashHash);
            }
        }
    }

    private void HandleGun()
    {
        if (!_hasGun)
            return;

        // if(Input.GetKeyDown(KeyCode.Mouse0))
        Gun.GetComponent<GunController>().SetFiring(_input.fire);
        // if (_input.reload)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Gun.GetComponent<GunController>().Reload();
        }
    }

    private void Move()
    {
        if (!canMove)
            return;

        // store the last move direction if player does move
        if (_input.move.x != 0 || _input.move.y != 0)
            LastMoveDirection = MoveDirection;

        MoveDirection = _input.move.normalized;

        // move char by rigidbody
        // _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, MoveDirection * MoveSpeed, Acceleration * Time.deltaTime);
        _rigidbody.velocity = MoveDirection * MoveSpeed;

        // current magnitude speed
        currentSpeed = _rigidbody.velocity.magnitude;
    }

    private void HandleAnimations()
    {
        if (_hasAnimator)
        {
            _animator.SetFloat(_speedHash, currentSpeed);
            _animator.SetFloat(_speedXHash, lookDirection.x);
            _animator.SetFloat(_speedYHash, lookDirection.y);
        }
    }

    private void HandleFlipX()
    {
        // handle player animation facing direction
        // and switch gun to another hand if player is facing left
        if (lookDirection.x > 0)
        {
            // _spriteRenderer.flipX = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (lookDirection.x < 0)
        {
            // _spriteRenderer.flipX = true;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    IEnumerator DisableMovementRoutine(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator DashRoutine(Vector2 dashDir)
    {
        // start ghost effect
        // _ghostEffect.Play(DashTime);

        // start dash
        canDash = false;
        isDashing = true;
        _rigidbody.velocity = dashDir.normalized * DashSpeed;

        yield return new WaitForSeconds(DashTime);

        // end dash
        isDashing = false;
        _rigidbody.velocity = Vector2.zero;

        // dash cooldown
        yield return new WaitForSeconds(DashCooldownTime);
        canDash = true;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        Debug.Log("HP: " + HP);
        if (HP > 0)
        {
            _flashEffect.Flash();
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        // _rigidbody.velocity = Vector2.zero;
        // _animator.SetTrigger(_dieHash);
        // canMove = false;
        // canDash = false;
        // hand.SetActive(false);
        // _animator.SetLayerWeight(1, 0f);
        // _animator.SetLayerWeight(0, 1f);
    }
}
