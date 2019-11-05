using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBullet : MonoBehaviour
{
    public void Die() {
        Destroy(this.gameObject);
    }
}
