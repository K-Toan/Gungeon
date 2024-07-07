using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Bullet Stats")]
    public float BulletDamage = 1f;
    public float BulletSpeed = 5f;
    public float BulletExistTime = 5f;

    [Header("Gun Stats")]
    [Space] // ammo
    public int CurrentMagazine = 0;
    public int MagazineCapacity = 30;
    [Space] // firerate & recoil
    public float FireRate = 300;
    public float SpreadRate = 0.15f;
    public float ReloadTime = 3f;
    [SerializeField] private float holdTime = 0f;
    private float nextFireTime = 0f;
    private float reloadTimeLeft = 0f;

    [Header("States")]
    [SerializeField] private bool isReloading = false;
    [SerializeField] private bool allowScreenShake = false;

    [Header("Modes/Types")]
    [SerializeField] private bool hasInfinityAmmo = false;
    [SerializeField] private FireMode fireMode = FireMode.Semi;
    [SerializeField] private BulletType bulletType = BulletType.Projectile;

    [Header("Game Objects")]
    public GameObject Bullet;
    public GameObject Muzzle;
    [Space]
    public GameObject GunRoot;
    public GameObject Hand;
    private GameObject primaryHand;
    private GameObject secondaryHand;
    public Transform PrimaryHand;
    public Transform SecondaryHand;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] private Transform MuzzlePosition;

    [Header("Components")]
    [SerializeField] private bool _hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private GunInputSystem _input;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioSource audioSource;

    private int baseSpriteOrder = 0;

    // private
    private enum FireMode { Semi, Auto }
    private enum BulletType { Projectile, Raycast, Laser }

    // [Header("Animation Hash IDs")]
    private int _shootHash;
    private int _reloadHash;

    private void Start()
    {
        // GOs
        PrimaryHand = transform.Find("PrimaryHand");
        SecondaryHand = transform.Find("SecondaryHand");
        ShootPosition = transform.Find("ShootPosition");
        MuzzlePosition = transform.Find("MuzzlePosition");
        GunRoot = transform.parent.gameObject;

        // Components
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _input = GetComponent<GunInputSystem>();

        // SFX
        audioSource = gameObject.AddComponent<AudioSource>();

        AssignHands();
        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _shootHash = Animator.StringToHash("Shoot");
        _reloadHash = Animator.StringToHash("Reload");
    }

    private void AssignHands()
    {
        if (Hand != null)
        {
            primaryHand = Instantiate(Hand, PrimaryHand.transform);
            primaryHand.SetActive(true);
            primaryHand.transform.SetParent(PrimaryHand);

            secondaryHand = Instantiate(Hand, SecondaryHand.transform);
            secondaryHand.SetActive(true);
            secondaryHand.transform.SetParent(SecondaryHand);
        }
    }

    private void Update()
    {
        HandleRotate();
        HandleFire();
        HandleReload();
    }

    private void HandleFire()
    {
        if(nextFireTime > 0)
        {
            nextFireTime -= Time.deltaTime;
        }

        if (isReloading)
            return;

        if (_input.fire)
        {
            // auto reload
            if (CurrentMagazine <= 0)
                Reload();

            switch (fireMode)
            {
                case FireMode.Semi:
                    if (holdTime == 0)
                        Fire();
                    break;

                case FireMode.Auto:
                    Fire();
                    break;
            }
            holdTime += Time.deltaTime;
        }
        else
        {
            holdTime = 0f;
        }
    }

    private void HandleRotate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = mousePos - (Vector2)GunRoot.transform.position;

        float distanceGun2Mouse = aimDir.magnitude;
        if (distanceGun2Mouse < 0.5)
            return;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        GunRoot.transform.eulerAngles = new Vector3(0, 0, angle);

        if(angle <= 90 && angle >= -90)
        {
            GunRoot.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            GunRoot.transform.localScale = new Vector3(-1f, -1f, 1f);
        }

        if (angle < 0)
        {
            _spriteRenderer.sortingOrder = baseSpriteOrder + 1;
            primaryHand.GetComponent<SpriteRenderer>().sortingOrder = baseSpriteOrder + 2;
            secondaryHand.GetComponent<SpriteRenderer>().sortingOrder = baseSpriteOrder + 2;
        }
        else
        {
            _spriteRenderer.sortingOrder = baseSpriteOrder - 2;
            primaryHand.GetComponent<SpriteRenderer>().sortingOrder = baseSpriteOrder - 1;
            secondaryHand.GetComponent<SpriteRenderer>().sortingOrder = baseSpriteOrder - 1;
        }
    }

    private void Fire()
    {
        if (nextFireTime > 0)
        {
            return;
        }

        switch (bulletType)
        {
            case BulletType.Projectile:
                FireProjectile();
                break;

            case BulletType.Raycast:
                FireRaycast();
                break;
        }

        if (!hasInfinityAmmo)
        {
            CurrentMagazine--;
        }

        nextFireTime = 60 / FireRate;

        // sfx
        if (audioSource != null && shotSound != null)
        {
            audioSource.PlayOneShot(shotSound);
        }

        // screen shake
        if (allowScreenShake)
        {
            if (Camera.main.gameObject.TryGetComponent<CameraController>(out var mainCameraController))
            {
                mainCameraController.Shake(ShootPosition.right.normalized * -1);
            }
        }

        if (_hasAnimator)
        {
            _animator.SetTrigger(_shootHash);
        }
    }

    private void FireProjectile()
    {
        // recoil/spread
        float spread = Random.Range(-SpreadRate, SpreadRate);
        Vector2 shootDir = ShootPosition.right + new Vector3(0f, spread);

        GameObject bullet = Instantiate(Bullet, ShootPosition.position, ShootPosition.rotation);
        GameObject muzzle = Instantiate(Muzzle, MuzzlePosition.position, MuzzlePosition.rotation);

        Destroy(bullet, BulletExistTime);
        Destroy(muzzle, 0.5f);

        bullet.GetComponent<Rigidbody2D>().velocity = shootDir * BulletSpeed;
    }

    private void FireRaycast()
    {

    }

    private void HandleReload()
    {
        if (reloadTimeLeft > 0)
        {
            reloadTimeLeft -= Time.deltaTime;
        }
        else
        {
            if (isReloading)
            {
                isReloading = false;
                CurrentMagazine = MagazineCapacity;
            }
            else
            {
                if (CurrentMagazine == MagazineCapacity)
                    return;

                if (_input.reload)
                {
                    Reload();
                }
            }
        }
    }

    public void Reload()
    {
        reloadTimeLeft = ReloadTime;
        isReloading = true;

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        if (_hasAnimator)
        {
            _animator.SetTrigger(_reloadHash);
        }
    }
}
