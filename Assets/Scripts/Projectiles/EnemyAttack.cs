using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    bool destroyOnContact = true;
    [SerializeField]
    bool deactivateOnContact = false;
    [SerializeField]
    bool shouldTakeDamage = true;
    [SerializeField]
    int maxHealth = 1;
    [SerializeField]
    SpriteFlashComponent spriteFlashComponent;

    int health = -1;
    
    public int damage = 1;

    void OnEnable() {
        health = maxHealth;
        spriteFlashComponent.ResetState();
    }

    public void OnPlayerContact() {
        Kill();
    }

    public void TakeDamage(int amount) {
        if (!shouldTakeDamage) return;

        StartCoroutine(OnGettingShotCoroutine(amount));
    }

    private IEnumerator OnGettingShotCoroutine(int amount) {
        health -= amount;

        spriteFlashComponent.SetFlash(1);
        yield return HushPuppy.WaitForEndOfFrames(5);
        spriteFlashComponent.SetFlash(1 - (health / (float) maxHealth));

        if (health <= 0) {
            Kill();
        }
    }

    public static EnemyAttack GetAttackComponent(GameObject gameObject) {
        var output = gameObject.GetComponentInChildren<EnemyAttack>();
        if (output != null) return output;
        return gameObject.GetComponentInParent<EnemyAttack>();
    }

    void Kill() {
        if (destroyOnContact) {
            Destroy(this.gameObject);
        } else if (deactivateOnContact) {
            this.gameObject.SetActive(false);
        }
    }
}
