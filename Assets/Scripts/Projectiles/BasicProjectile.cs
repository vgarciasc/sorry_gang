using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : EnemyAttack
{
    [Header("Attributes")]
    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    bool followPlayer;
    [SerializeField]
    float followPlayerDuration;
    [SerializeField]
    float followCorrectionSpeed;
    [SerializeField]
    bool faceVelocity;

    [Header("Components")]

    [SerializeField]
    Rigidbody2D rb;
    Transform player;

    float acceleration = 0f;
    Coroutine followPlayerCoroutine;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        HandleAcceleration();
        HandleFollowPlayer();
        HandleFaceVelocity();
    }

    void HandleFaceVelocity() {
        if (faceVelocity) {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    void HandleAcceleration() {
        if (acceleration != 0) {
            rb.velocity = rb.velocity + rb.velocity.normalized * acceleration * Time.deltaTime;
        }
    }

    public void SetVelocity(Vector3 vector) {
        rb.velocity = vector.normalized * bulletSpeed;
    }

    public void SetAcceleration(float acceleration) {
        this.acceleration = acceleration;
    }
    
    public void OnTriggerExit2D(Collider2D collision) {
        var obj = collision.gameObject;

        if (obj.CompareTag("Arena")) {
            Destroy(this.gameObject);
        }
    }

    void HandleFollowPlayer() {
        if (followPlayer) {
            if (followPlayerCoroutine != null) {
                Vector3 diff = player.position - transform.position;
                diff.Normalize();

                float rot_z_target = TrigonometrySquash(Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90);
                // if (rot_z_target > 180) {
                //     rot_z_target = rot_z_target - 360;
                // }
                float rot_z_current = TrigonometrySquash(transform.rotation.eulerAngles.z);
                float rot_z_diff = TrigonometrySquash(rot_z_target - rot_z_current);

                // print("rot_z_current: " + rot_z_current + ", rot_z_target: " + rot_z_target);
                // print("rot_z_diff: " + rot_z_diff);
                float new_rot_z = rot_z_current + rot_z_diff * followCorrectionSpeed;

                transform.rotation = Quaternion.Euler(0f, 0f, new_rot_z);

                this.SetVelocity(this.transform.up);
            } else {
                followPlayerCoroutine = StartCoroutine(FollowPlayerCooldown());
            }
        }
    }

    float TrigonometrySquash(float value) {
        // if (value < -180) value += 360;
        // if (value > +360) value %= 360;
        value = (value + 360) % 360;
        if (value > 180 && value < 360) return value - 360;
        return value;
    }

    IEnumerator FollowPlayerCooldown() {
        yield return new WaitForSeconds(followPlayerDuration);
        followPlayer = false;
    }
}
