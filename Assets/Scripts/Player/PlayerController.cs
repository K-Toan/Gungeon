using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [Space]
    public string Name = "";
    public float MaxHP = 100;
    public float CurrentHP;
    public bool canHit = true;

    [Space]
    public int CurrentLevel = 1;
    public int MaxLevel = 4;
    public int CurrentExp = 0;
    public int MaxExp = 100;

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

    [Header("Ability")]
    public Ability PlayerAbility;
    public float AbilityCooldownTime = 1f;

    [Header("Shield")]
    public float ShieldTime = 5f;

    [Header("Dash")]
    public float DashSpeed = 15f;
    public float DashTime = 0.25f;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;

    [Header("Sandevistan")]
    [SerializeField] private bool canSlowDownTime = true;
    [SerializeField] private bool isSlowedDown = false;
    public float SlowTime = 5f;
    public float SlowFactor = 0.5f;

    [Header("Rotation")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 gunDirection;

    [Header("Guns")]
    [SerializeField] private bool hasGun;
    public GameObject GunRoot;
    public GameObject Hand;
    public GameObject Gun;
    public Image ImageGun;

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

    [Header("HealthBar UI")]
    public HealthBarUI healthBar;
    public enum Ability { Shield, Dash, Sandevistan }

    private void Start()
    {

        // Set HP
        healthBar = FindObjectOfType<HealthBarUI>();
        CurrentHP = MaxHP;
        healthBar.SetMaxHealth((int)MaxHP);

        // game objects
        Hand = transform.Find("Hand").gameObject;
        GunRoot = transform.Find("GunRoot").gameObject;
        ImageGun = GameObject.Find("GunImage").GetComponent<Image>();
        if (ImageGun == null)
        {
            Debug.LogError("ImageGun not found!");
        }
        // components
        hasAnimator = TryGetComponent<Animator>(out _animator);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostEffect = GetComponent<GhostEffect>();
        _flashEffect = GetComponent<FlashEffect>();
        _gunSystem = GetComponent<GunSystem>();
        _input = GetComponent<PlayerInputSystem>();
        _rigidbody = GetComponent<Rigidbody2D>();

        GameObject firstGun = _gunSystem.GetFirstGun();
        AssignGun(firstGun);
        UpdateImageGun(firstGun, gunSizes[1]);
        AssignGun(_gunSystem.GetFirstGun());
        AssignAnimationHashes();
    }

    private void AssignGun(GameObject gun)
    {
        if (gun != null)
        {
            Debug.Log("Assigning gun: " + gun.name);
            hasGun = true;
            Gun = gun;
            _gunController = Gun.GetComponent<GunController>();
        }
        else
        {
            Debug.LogWarning("Gun is null!");
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
        HandleAbility();
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

    private void HandleAbility()
    {
        if (isDodging || isDashing)
            return;

        if (_input.ability)
        {
            switch (PlayerAbility)
            {
                case Ability.Dash:
                    if (canDash && _input.move != Vector2.zero)
                    {
                        // recalculate Dodge direction
                        Vector2 dashDir = _input.move.normalized;
                        StartCoroutine(DashRoutine(dashDir));
                    }
                    break;

                case Ability.Shield:

                    break;

                case Ability.Sandevistan:
                    if (canSlowDownTime)
                    {
                        StartCoroutine(SandevistanRoutine(SlowTime, SlowFactor));
                    }
                    break;
            }
        }
    }

    private Dictionary<int, Vector2> gunSizes = new Dictionary<int, Vector2>
    {
    { 1, new Vector2(110, 50) },
    { 2, new Vector2(90, 90) },
    { 3, new Vector2(90, 90) }
    };
    private void HandleGun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AssignGun(_gunSystem.GetGun(1));
            UpdateImageGun(_gunSystem.GetGun(1), gunSizes[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AssignGun(_gunSystem.GetGun(2));
            UpdateImageGun(_gunSystem.GetGun(2), gunSizes[2]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AssignGun(_gunSystem.GetGun(3));
            UpdateImageGun(_gunSystem.GetGun(3), gunSizes[3]);
        }

        if (hasGun)
        {
            _gunController.HandleInput(_input.fire, _input.reload, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    private void UpdateImageGun(GameObject gun, Vector2 size)
    {
        if (ImageGun != null && gun != null)
        {
            SpriteRenderer gunSpriteRenderer = gun.GetComponent<SpriteRenderer>();
            if (gunSpriteRenderer != null)
            {
                ImageGun.sprite = gunSpriteRenderer.sprite;
                ImageGun.rectTransform.sizeDelta = size; // Thay đổi kích thước của ImageGun
            }
            else
            {
                Debug.LogWarning("Gun does not have a SpriteRenderer component!");
            }
        }
    }


    private void Move()
    {
        if (!canMove || isDashing || isDodging)
            return;

        _rigidbody.velocity = _input.move.normalized * MoveSpeed;

        currentSpeed = _rigidbody.velocity.magnitude * (isSlowedDown ? 2 / SlowFactor : 1);
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
        if (!isDodging)
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
        StartCoroutine(DisableMovementRoutine(DashTime));

        // start ghost effect
        _ghostEffect.enabled = true;

        // start Dodge
        canDash = false;
        isDashing = true;
        _rigidbody.velocity = moveDir.normalized * DashSpeed;

        yield return new WaitForSeconds(DashTime);

        // end ghost effect
        _ghostEffect.enabled = false;

        // end Dodge
        isDashing = false;
        _rigidbody.velocity = Vector2.zero;

        // Dodge cooldown
        yield return new WaitForSeconds(AbilityCooldownTime);
        canDash = true;
    }

    IEnumerator SandevistanRoutine(float time, float factor)
    {
        StartCoroutine(GameManager.Instance.SlowDownGameRoutine(time, factor));

        // start ghost effect
        _ghostEffect.enabled = true;
        isSlowedDown = true;
        canSlowDownTime = false;

        yield return new WaitForSecondsRealtime(time);

        // end ghost effect
        _ghostEffect.enabled = false;
        isSlowedDown = false;

        // Ability cooldown
        yield return new WaitForSeconds(AbilityCooldownTime);
        canSlowDownTime = true;
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        if (!canHit)
            return;

        CurrentHP -= damage;
        healthBar.SetHealth((int)CurrentHP);
        if (CurrentHP > 0)
        {
            StartCoroutine(TakeDamageRoutine(direction));
        }
        else
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        healthBar.SetHealth((int)CurrentHP);
    }

    public void HandleExpChange(int exp)
    {
        CurrentExp += exp;
        if (CurrentExp > MaxExp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        MaxHP += 10;
        CurrentHP += 10;

        CurrentLevel++;

        CurrentExp = 0;
        MaxExp += 20;
    }

    private IEnumerator TakeDamageRoutine(Vector2 dir)
    {
        _rigidbody.velocity = dir;
        canHit = false;
        yield return new WaitForSeconds(1f);
        _rigidbody.velocity = Vector2.zero;
        canHit = true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        ExpManager.Instance.OnExpChange += HandleExpChange;
    }

    private void OnDisable()
    {
        ExpManager.Instance.OnExpChange -= HandleExpChange;
    }
}