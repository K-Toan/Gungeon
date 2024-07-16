using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Stats")]
    public string Name = "";
    [Space] // ammo
    public float CurrentMagazine = 0;
    public float MagazineCapacity = 100;
    [Space] // firerate & recoil
    public float FireRate = 300;
    public float SpreadRate = 0f;
    public float ReloadTime = 1f;
    [SerializeField] private float holdTime = 0f;
    private float nextFireTime = 0f;
    private float reloadTimeLeft = 0f;

    [Header("Bullet Stats")]
    public float BulletDamage = 1f;
    public float BulletSpeed = 5f;
    public float BulletExistTime = 5f;

    [Header("States")]
    private bool fire = false;
    private bool reload = false;
    private Vector2 aimPosition;
    [SerializeField] private bool isReloading = false;
    [SerializeField] private bool allowScreenShake = false;

    [Header("Modes/Types")]
    [SerializeField] private bool hasInfinityAmmo = false;
    [SerializeField] private FireMode fireMode = FireMode.Semi;
    [SerializeField] private BulletType bulletType = BulletType.Projectile;

    [Header("Game Objects")]
    public GameObject Bullet;
    public GameObject Muzzle;
    private GameObject laserBeam;
    [Space]
    private GameObject GunRoot;
    private GameObject Hand;
    private GameObject primaryHand;
    private GameObject secondaryHand;
    public Transform PrimaryHand;
    public Transform SecondaryHand;
    private Transform ShootPosition;
    private Transform MuzzlePosition;

    [Header("Components")]
    private bool hasAnimator;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip reloadSound;
    private AudioSource audioSource;

    private int baseSpriteOrder = 0;

    // private
    private enum FireMode { Semi, Auto }
    private enum BulletType { Projectile, Raycast, LaserBeam }

    // [Header("Animation Hash IDs")]
    private int _shootHash;
    private int _reloadHash;

    private void Start()
    {
        Hand = transform.parent.parent.Find("Hand").gameObject;
        // GOs
        PrimaryHand = transform.Find("PrimaryHand");
        SecondaryHand = transform.Find("SecondaryHand");
        ShootPosition = transform.Find("ShootPosition");
        MuzzlePosition = transform.Find("MuzzlePosition");
        GunRoot = transform.parent.gameObject;

        // Components
        hasAnimator = TryGetComponent<Animator>(out _animator);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();

        if (bulletType == BulletType.LaserBeam)
        {
            laserBeam = Instantiate(Bullet, ShootPosition);
            laserBeam.transform.SetParent(ShootPosition);
            laserBeam.GetComponent<LaserBeam>().Damage = BulletDamage;
            laserBeam.SetActive(false);
        }

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

    public void HandleInput(bool f, bool r, Vector2 aimPos)
    {
        fire = f;
        reload = r;
        aimPosition = aimPos;
    }

    private void HandleFire()
    {
        if (nextFireTime > 0)
        {
            nextFireTime -= Time.deltaTime;
        }

        if (isReloading)
            return;

        if (fire)
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

            if (bulletType == BulletType.LaserBeam)
            {
                FireLaserBeam();
            }

            holdTime += Time.deltaTime;
        }
        else
        {
            holdTime = 0f;

            if (bulletType == BulletType.LaserBeam)
            {
                StopLaserBeam();
            }
        }
    }

    private void HandleRotate()
    {
        Vector2 aimDir = aimPosition - (Vector2)GunRoot.transform.position;

        float distanceGun2Mouse = aimDir.magnitude;
        if (distanceGun2Mouse < 0.5)
            return;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        GunRoot.transform.eulerAngles = new Vector3(0, 0, angle);

        if (angle <= 90 && angle >= -90)
            GunRoot.transform.localScale = new Vector3(1f, 1f, 1f);
        else
            GunRoot.transform.localScale = new Vector3(-1f, -1f, 1f);

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
            return;

        switch (bulletType)
        {
            case BulletType.Projectile:
                FireProjectile();
                break;

            case BulletType.Raycast:
                FireRaycast();
                break;
        }

        // screen shake
        if (allowScreenShake)
        {
            GameManager.Instance.MainCamera.GetComponent<CameraController>().Shake(ShootPosition.right.normalized * -1);
        }

        if (hasAnimator)
        {
            _animator.SetTrigger(_shootHash);
        }
    }

    private void FireProjectile()
    {
        nextFireTime = 60 / FireRate;
        if (!hasInfinityAmmo)
            CurrentMagazine--;

        // recoil/spread
        float spread = Random.Range(-SpreadRate, SpreadRate);
        Vector2 shootDir = ShootPosition.right + new Vector3(0f, spread);

        GameObject bullet = Instantiate(Bullet, ShootPosition.position, ShootPosition.rotation);
        GameObject muzzle = Instantiate(Muzzle, MuzzlePosition.position, MuzzlePosition.rotation);

        Destroy(bullet, BulletExistTime);
        Destroy(muzzle, 0.45f);

        bullet.GetComponent<Rigidbody2D>().velocity = shootDir * BulletSpeed;
        bullet.GetComponent<BulletProjectile>().Damage = BulletDamage;

        // sfx
        if (audioSource != null && shotSound != null)
        {
            audioSource.PlayOneShot(shotSound);
        }
    }

    private void FireRaycast()
    {

    }

    private void FireLaserBeam()
    {
        CurrentMagazine -= Time.deltaTime;
        if (laserBeam != null && !laserBeam.activeSelf)
        {
            laserBeam.SetActive(true);
        }
        if (holdTime > 0 && !audioSource.isPlaying)
        {
            if (audioSource != null && shotSound != null)
            {
                audioSource.clip = shotSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    private void StopLaserBeam()
    {
        if (laserBeam != null && laserBeam.activeSelf)
        {
            laserBeam.SetActive(false);
        }
        if (audioSource != null && reloadSound != null)
        {
            audioSource.Stop();
        }
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

                if (audioSource != null && reloadSound != null)
                {
                    audioSource.Stop();
                }
            }
            else
            {
                if (CurrentMagazine == MagazineCapacity)
                    return;

                if (reload)
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
            audioSource.clip = reloadSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (hasAnimator)
        {
            _animator.SetTrigger(_reloadHash);
        }
    }

    private void OnEnable()
    {
        isReloading = false;
        
    }
}
