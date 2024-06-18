using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Stats")]
    public float Damage = 1f;
    [Space] // ammo
    public int CurrentMagazine = 0;
    public int MagazineCapacity = 30;
    [Space] // firerate & recoil
    public float FireRate = 300;
    public float SpreadRate = 0.15f;
    public float ReloadTime = 3f;
    private float holdTime = 0f;

    [Header("States")]
    [SerializeField] private bool isReloading = false;
    [SerializeField] private bool isFiring = false;
    [SerializeField] private bool canFire = true;
    [SerializeField] private bool allowScreenShake = false;

    [Header("Modes/Types")]
    [SerializeField] private bool hasInfinityAmmo = true;
    [SerializeField] private Hand handMode = Hand.One;
    [SerializeField] private FireMode fireMode = FireMode.Semi;
    [SerializeField] private BulletType bulletType = BulletType.Projectile;

    [Header("Game Objects")]
    public GameObject Visual;
    public GameObject Bullet;
    public GameObject Muzzle;

    [Header("Components")]
    [SerializeField] private bool _hasAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip reloadSound;
    private AudioSource audioSource;

    // private
    private enum Hand { One, Two }
    private enum FireMode { Semi, Auto }
    private enum BulletType { Projectile, Raycast, Laser }

    // [Header("Animation Hash IDs")]
    private int _shootHash;

    private void Start()
    {
        _hasAnimator = Visual.TryGetComponent<Animator>(out _animator);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = shotSound;
        AssignAnimationHashes();
    }

    private void AssignAnimationHashes()
    {
        _shootHash = Animator.StringToHash("Shoot");
    }

    private void Update()
    {
        if (isFiring)
        {
            PullTrigger();
            holdTime += Time.deltaTime;
        }
        else
        {
            holdTime = 0f;
        }
    }

    public void SetFiring(bool fire)
    {
        isFiring = fire;
    }

    private void PullTrigger()
    {
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
    }

    private void Fire()
    {
        if (!canFire || isReloading || CurrentMagazine <= 0)
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

        CurrentMagazine--;
        StartCoroutine(Waiting(60 / FireRate));

        if (_hasAnimator)
        {
            _animator.SetTrigger(_shootHash);
        }
    }

    private void FireProjectile()
    {
        // recoil/spread
        float spread = Random.Range(-SpreadRate, SpreadRate);
        Vector2 shootDir = ShootPosition.up + new Vector3(0f, spread);

        GameObject bullet = Instantiate(Bullet, ShootPosition.position, ShootPosition.rotation);
        GameObject muzzle = Instantiate(Muzzle, ShootPosition.position, ShootPosition.rotation);

        Destroy(muzzle, 0.5f);

        // bullet.GetComponent<Rigidbody2D>().velocity = shootDir * 5f;

        // sfx
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // screen shake
        if (allowScreenShake)
        {
            if (Camera.main.gameObject.TryGetComponent<CameraController>(out var mainCameraController))
            {
                mainCameraController.Shake(shootDir.normalized * -1);
            }
        }
    }

    private void FireRaycast()
    {

    }

    private IEnumerator Waiting(float time)
    {
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
    }

    public void Reload()
    {
        if (CurrentMagazine == MagazineCapacity || isReloading)
            return;

        // start reloading
        StartCoroutine(Reloading(ReloadTime));
    }

    private IEnumerator Reloading(float time)
    {
        // start reload
        isReloading = true;

        yield return new WaitForSeconds(time);

        // finish reloading
        isReloading = false;

        CurrentMagazine = MagazineCapacity;
    }

    public bool IsOneHand()
    {
        return handMode == Hand.One;
    }

    public bool IsTwoHands()
    {
        return handMode == Hand.Two;
    }
}
