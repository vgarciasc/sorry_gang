using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float BASE_SPEED;
    [SerializeField]
    float SHOT_COOLDOWN;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject teleportBulletPrefab;

    Rigidbody2D rb;
    bool insideShootingCooldown = false;

    GameObject teleportBullet;

    bool blockInput = false;
    GameObject weakSpot;

    void Start() {
        rb = this.GetComponentInChildren<Rigidbody2D>();
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
        
        Vector3 motion = new Vector3(hor, ver) * BASE_SPEED / 10f;
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
        }

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
        yield return new WaitForSeconds(SHOT_COOLDOWN);
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
    }

    void OnTriggerExit2D(Collider2D collider) {
        var obj = collider.gameObject;
        if (obj.CompareTag("WeakSpot")) {
            weakSpot = null;
        }
    }
}
