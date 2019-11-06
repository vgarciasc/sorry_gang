using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    bool destroyOnContact = true;
    [SerializeField]
    bool deactivateOnContact = false;
    
    public int damage = 1;

    public void OnPlayerContact() {
        if (destroyOnContact) {
            Destroy(this.gameObject);
        } else if (deactivateOnContact) {
            this.gameObject.SetActive(false);
        }
    }
}
