using System.Collections;
using UnityEngine;

public class PlayerController : Damageable
{
    [Header("Stats")]
    public string Name = "";
    public float HP = 100;

    [Header("Move")]
    public float MoveSpeed = 3f;
    public float Acceleration = 20f;
    public Vector2 MoveDirection;
    public Vector2 LastMoveDirection;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool canMove = true;

    [Header("Dodge")]
    public float DodgeSpeed = 3f;
    public float DodgeTime = 0.75f;
    public float DodgeCooldownTime = 0.25f;
    [SerializeField] private bool isDodging = false;
    [SerializeField] private bool canDodge = true;

    [Header("Dash")]
    public float DashSpeed = 15f;
    public float DashTime = 0.25f;
    public float DashCooldownTime = 0.25f;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;

    [Header("Rotation")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 gunDirection;

    [Header("Guns")]
    [SerializeField] private bool hasGun;
    public GameObject GunRoot;
    public GameObject Hand;
    public GameObject Gun;

    [Header("Components")]
    [SerializeField] private bool hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider2D _hitboxCollider;
    [SerializeField] private Collider2D _environmentCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PlayerInputSystem _input;
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Scripts")]
    [SerializeField] private GhostEffect _ghostEffect;
    [SerializeField] private FlashEffect _flashEffect;
    [SerializeField] private GunSystem _gunSystem;
    [SerializeField] private GunController _gunController;

    [Header("Animation Hash IDs")]
    private int _speedHash;
    private int _lookXHash;
    private int _lookYHash;
    private int _speedXHash;
    private int _speedYHash;
    private int _dodgeHash;
    private int _dieHash;

    private void Start()
    {
        // game objects
        Hand = transform.Find("Hand").gameObject;
        GunRoot = transform.Find("GunRoot").gameObject;

        // components
        hasAnimator = TryGetComponent<Animator>(out _animator);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostEffect = GetComponent<GhostEffect>();
        _flashEffect = GetComponent<FlashEffect>();
        _gunSystem = GetComponent<GunSystem>();
        _input = GetComponent<PlayerInputSystem>();
        _rigidbody = GetComponent<Rigidbody2D>();

        AssignGun(_gunSystem.GetFirstGun());
        AssignAnimationHashes();
    }

    private void AssignGun(GameObject gun)
    {
        if(gun != null)
        {
            hasGun = true;
            Gun = gun;
            _gunController = Gun.GetComponent<GunController>();
        }
    }

    private void AssignAnimationHashes()
    {
        _speedHash = Animator.StringToHash("Speed");
        _lookXHash = Animator.StringToHash("LookX");
        _lookYHash = Animator.StringToHash("LookY");
        _speedXHash = Animator.StringToHash("SpeedX");
        _speedYHash = Animator.StringToHash("SpeedY");
        _dodgeHash = Animator.StringToHash("Dodge");
        _dieHash = Animator.StringToHash("Die");
    }

    private void Update()
    {
        // handle logic parts
        HandleRotation();
        HandleDodge();
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // rotation
        lookDirection = (mousePos - transform.position).normalized;
    }

    private void HandleDodge()
    {
        if (!canDodge || isDodging || isDashing)
            return;

        if (_input.dodge && _input.move != Vector2.zero)
        {
            // recalculate Dodge direction
            Vector2 dodgeDir = _input.move.normalized;

            StartCoroutine(DisableMovementRoutine(DodgeTime));
            StartCoroutine(DodgeRoutine(dodgeDir));

            // animations
            if (hasAnimator)
            {
                _animator.SetTrigger(_dodgeHash);
            }
        }
    }

    private void HandleDash()
    {
        if (!canDash || isDodging || isDashing)
            return;

        if (_input.dash && _input.move != Vector2.zero)
        {
            // recalculate Dodge direction
            Vector2 dashDir = _input.move.normalized;

            StartCoroutine(DisableMovementRoutine(DashTime));
            StartCoroutine(DashRoutine(dashDir));
        }
    }

    private void HandleGun()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            AssignGun(_gunSystem.GetGun(1));
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            AssignGun(_gunSystem.GetGun(2));
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            AssignGun(_gunSystem.GetGun(3));

        if(hasGun)
        {
            _gunController.HandleInput(_input.fire, _input.reload, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private void Move()
    {
        if (!canMove || isDashing || isDodging)
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
        if (hasAnimator)
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
        if(!isDodging)
        {
            if (lookDirection.x > 0)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else if (lookDirection.x < 0)
                transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            if (_rigidbody.velocity.x > 0)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else if (_rigidbody.velocity.x < 0)
                transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        
    }

    IEnumerator DisableMovementRoutine(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator DodgeRoutine(Vector2 moveDir)
    {
        // start Dodge
        canDodge = false;
        isDodging = true;
        _hitboxCollider.enabled = false;
        GunRoot.SetActive(false);
        _rigidbody.velocity = moveDir.normalized * DodgeSpeed;

        yield return new WaitForSeconds(DodgeTime);

        // end Dodge
        isDodging = false;
        GunRoot.SetActive(true);
        _hitboxCollider.enabled = true;
        _rigidbody.velocity = Vector2.zero;

        // Dodge cooldown
        yield return new WaitForSeconds(DodgeCooldownTime);
        canDodge = true;
    }
    
    IEnumerator DashRoutine(Vector2 moveDir)
    {
        // start ghost effect
        _ghostEffect.Play(DodgeTime);

        // start Dodge
        canDash = false;
        isDashing = true;
        _rigidbody.velocity = moveDir.normalized * DashSpeed;

        yield return new WaitForSeconds(DashTime);

        // end Dodge
        isDashing = false;
        _rigidbody.velocity = Vector2.zero;

        // Dodge cooldown
        yield return new WaitForSeconds(DashCooldownTime);
        canDash = true;
    }
}
