using System.Collections;
using UnityEngine;

public class PlayerController : Damageable
{
    [Header("Move")]
    public float MoveSpeed = 3f;
    public float Acceleration = 20f;
    public Vector2 MoveDirection;
    public Vector2 LastMoveDirection;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool canMove = true;

    [Header("Dash/Dodge")]
    public float DodgeSpeed = 3f;
    public float DodgeTime = 0.75f;
    public float DodgeCooldownTime = 0.25f;
    [SerializeField] private Vector2 DodgeDirection;
    [SerializeField] private bool isDodging = false;
    [SerializeField] private bool canDodge = true;

    [Header("Rotation")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 gunDirection;

    [Header("Game Objects")]
    [SerializeField] private bool _hasGun;
    public GameObject Gun;
    public GameObject GunRoot;
    
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
    private int _lookXHash;
    private int _lookYHash;
    private int _speedXHash;
    private int _speedYHash;
    private int _DodgeHash;
    private int _dieHash;

    private void Start()
    {
        // game objects
        _mainCamera = Camera.main;

        // components
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostEffect = GetComponent<GhostEffect>();
        _flashEffect = GetComponent<FlashEffect>();
        _weaponSystem = GetComponent<WeaponSystem>();
        _input = GetComponent<InputSystem>();
        _rigidbody = GetComponent<Rigidbody2D>();

        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _speedHash = Animator.StringToHash("Speed");
        _lookXHash = Animator.StringToHash("LookX");
        _lookYHash = Animator.StringToHash("LookY");
        _speedXHash = Animator.StringToHash("SpeedX");
        _speedYHash = Animator.StringToHash("SpeedY");
        _DodgeHash = Animator.StringToHash("Dodge");
        _dieHash = Animator.StringToHash("Die");
    }

    private void Update()
    {
        // handle logic parts
        HandleRotation();
        HandleDodge();
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
            Gun.transform.up = gunDirection;
        }
    }

    private void HandleDodge()
    {
        if (!canDodge)
            return;

        if (_input.dodge && _input.move != Vector2.zero)
        {
            // recalculate Dodge direction
            DodgeDirection = _input.move.normalized;

            StartCoroutine(DisableMovementRoutine(DodgeTime));
            StartCoroutine(DodgeRoutine(DodgeDirection));

            // animations
            if (_hasAnimator)
            {
                _animator.SetTrigger(_DodgeHash);
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
            _animator.SetFloat(_lookXHash, lookDirection.x);
            _animator.SetFloat(_lookYHash, lookDirection.y);
            _animator.SetFloat(_speedXHash, _rigidbody.velocity.x);
            _animator.SetFloat(_speedYHash, _rigidbody.velocity.y);
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

    IEnumerator DodgeRoutine(Vector2 DodgeDir)
    {
        // start ghost effect
        // _ghostEffect.Play(DodgeTime);

        // start Dodge
        canDodge = false;
        isDodging = true;
        _rigidbody.velocity = DodgeDir.normalized * DodgeSpeed;

        yield return new WaitForSeconds(DodgeTime);

        // end Dodge
        isDodging = false;
        _rigidbody.velocity = Vector2.zero;

        // Dodge cooldown
        yield return new WaitForSeconds(DodgeCooldownTime);
        canDodge = true;
    }
}
