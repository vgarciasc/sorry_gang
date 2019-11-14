using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float baseSpeed;
    [SerializeField]
    float shotCooldown;
    [SerializeField]
    float invincibilityDuration;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject teleportBulletPrefab;
    [SerializeField]
    LineRenderer shotLineRenderer;
    [SerializeField]
    int maxHealth = 50;

    Rigidbody2D rb;
    SpriteRenderer sr;
    bool insideShootingCooldown = false;

    GameObject teleportBullet;

    bool blockInput = false;
    GameObject weakSpot;

    int health = 0;
    bool isInvincible = false;

    void Start() {
        rb = this.GetComponentInChildren<Rigidbody2D>();
        sr = this.GetComponentInChildren<SpriteRenderer>();

        health = maxHealth;
    }

    void Update() {
        HandleRotate();
        HandleShooting();
    }

    void FixedUpdate() {
        HandleMotion();
    }

    void HandleMotion() {
        if (blockInput) return;

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        
        Vector3 motion = new Vector3(hor, ver) * baseSpeed / 10f;
        motion = new Vector3(motion.x, motion.y, 0);

        rb.MovePosition(this.transform.position + motion);
    }

    void HandleRotate() {
        if (blockInput) return;

        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    void HandleShooting() {
        if (blockInput) return;

        if (Input.GetButton("Fire1") && weakSpot != null) {
            weakSpot.GetComponent<WeakSpotArea>().TakeDamage(this);
            return;
        }

        HandleShooting2();
        return;

        if (Input.GetButton("Fire1") && !insideShootingCooldown) {
            Shoot(bulletPrefab);
            StartCoroutine(ShootingCooldown());
        }
        if (Input.GetButtonDown("Fire2")) {
            if (teleportBullet == null) {
                teleportBullet = Shoot(teleportBulletPrefab);
            } else {
                Teleport();
            }
        }
    }

    void HandleShooting2() {
        if (Input.GetButtonDown("Fire1")) {
            StartCoroutine(Shoot2());
        }
    }

    IEnumerator Shoot2() {
        SpecialCamera.GetSpecialCamera().screenShake_(0.00001f, 5);

        shotLineRenderer.gameObject.SetActive(true);
        blockInput = true;
        rb.velocity = Vector2.zero;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(
            this.transform.position,
            new Vector3(mousePos.x - this.transform.position.x, mousePos.y - this.transform.position.y, 0),
            100,
            (1 << LayerMask.NameToLayer("Enemy")));

        shotLineRenderer.SetPosition(0, this.transform.position);

        if (hit) {
            shotLineRenderer.SetPosition(1, hit.point);
            var enemyAttack = EnemyAttack.GetAttackComponent(hit.collider.gameObject);
            if (enemyAttack != null) {
                enemyAttack.TakeDamage(1);
            } else {
                var enemy = hit.collider.gameObject.GetComponentInChildren<Enemy>();
                if (enemy != null) {
                    enemy.TakeDamage(1);
                }
            }
        }
        else {
            shotLineRenderer.SetPosition(1, new Vector3(
                this.transform.position.x + (mousePos.x - this.transform.position.x) * 100,
                this.transform.position.y + (mousePos.y - this.transform.position.y) * 100,
                0
            ));
        }
        yield return HushPuppy.WaitForEndOfFrames(5);
        // yield return new WaitForSeconds(0.05f);
        blockInput = false;
        shotLineRenderer.gameObject.SetActive(false);
    }

    public void ToggleBlockInput(bool value) {
        blockInput = value;
    }

    public void ToggleVisibility(bool value) {
        foreach (var sr in this.GetComponentsInChildren<SpriteRenderer>()) {
            sr.enabled = value;
        }
    }

    public void SetVelocity(Vector3 vector) {
        rb.velocity = vector;
    }

    public void SetAngularVelocity(float value) {
        rb.angularVelocity = value;
    }

    public void ToggleFreezeRotation(bool value) {
        rb.freezeRotation = value;
    }

    GameObject Shoot(GameObject prefab)
    {
        GameObject bullet = Instantiate(
            prefab,
            this.transform.position,
            this.transform.rotation);
        bullet.GetComponent<PlayerBullet>().Initialize(this.transform.up);
        return bullet;
    }

    IEnumerator ShootingCooldown() {
        insideShootingCooldown = true;
        yield return new WaitForSeconds(shotCooldown);
        insideShootingCooldown = false;
    }

    void Teleport() {
        if (teleportBullet == null) {
            Debug.LogError("This should not be happening.");
            return;
        }

        this.transform.position = teleportBullet.transform.position;
        teleportBullet.GetComponentInChildren<TeleportBullet>().Die();

        teleportBullet = null;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        var obj = collider.gameObject;

        if (obj.CompareTag("WeakSpot")) {
            weakSpot = obj;
        }

        // if (obj.CompareTag("EnemyBullet")) {
        //     var attack = obj.GetComponentInChildren<EnemyAttack>();
        //     if (attack == null) {
        //         attack = obj.GetComponentInParent<EnemyAttack>();
        //     }
        //     attack.OnPlayerContact();
        //     StartCoroutine(TakeDamage(attack.damage));
        // }
    }

    void OnTriggerStay2D(Collider2D collider) {
        var obj = collider.gameObject;

        if (obj.CompareTag("EnemyBullet")) {
            var attack = obj.GetComponentInChildren<EnemyAttack>();
            if (attack == null) {
                attack = obj.GetComponentInParent<EnemyAttack>();
            }
            attack.OnPlayerContact();
            StartCoroutine(TakeDamage(attack.damage));
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        var obj = collider.gameObject;
        if (obj.CompareTag("WeakSpot")) {
            weakSpot = null;
        }
    }

    IEnumerator TakeDamage(int amount) {
        if (isInvincible) yield break;

        PlayerPortraitManager.GetPlayerPortraitManager().TakeDamage();

        SpecialCamera.GetSpecialCamera().screenShake_(0.0001f, 15);

        health -= amount;
        if (health <= 0) {
            Death();
        }

        isInvincible = true;
        sr.color = HushPuppy.getColorWithOpacity(sr.color, 0.5f);
        yield return new WaitForSeconds(invincibilityDuration);
        sr.color = HushPuppy.getColorWithOpacity(sr.color, 1f);
        isInvincible = false;

        yield break;
    }

    void Death() {
        GameController.GetGameController().TransitionToGameOverScreen();
    }

    public void ToggleFreeze(bool value) {
        this.isInvincible = value;
        this.blockInput = value;
        if (!value) {
            rb.velocity = Vector3.zero;
        }
    }

    public float GetHealthPercentage() {
        return health / (float) maxHealth;
    }
}
