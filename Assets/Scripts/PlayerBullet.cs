using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    float BULLET_SPEED;

    public void Initialize(Vector3 vector) {
        this.GetComponent<Rigidbody2D>().velocity = vector * BULLET_SPEED;
    }
    
    public void OnTriggerExit2D(Collider2D collision) {
        var obj = collision.gameObject;

        if (obj.CompareTag("Arena")) {
            Destroy(this.gameObject);
        }
    }
}
