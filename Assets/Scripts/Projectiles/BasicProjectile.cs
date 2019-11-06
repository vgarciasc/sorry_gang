using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
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

                // float rot_z_target = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) % 360;
                // float rot_z_current = (this.transform.rotation.eulerAngles.z + 90) % 360;
                // float rot_z_motion = rot_z_current + (rot_z_target - rot_z_current) / followCorrectionSpeed;

                // transform.rotation = Quaternion.Euler(0f, 0f, rot_z_motion - 90);
                
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
                this.SetVelocity(this.transform.up);
            } else {
                followPlayerCoroutine = StartCoroutine(FollowPlayerCooldown());
            }
        }
    }

    IEnumerator FollowPlayerCooldown() {
        yield return new WaitForSeconds(followPlayerDuration);
        followPlayer = false;
    }
}
