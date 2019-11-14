using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpotArea : MonoBehaviour
{
    [SerializeField]
    Enemy enemy;

    public void TakeDamage(Player player) {
        // player.transform.position = Vector2.zero;
        enemy.TakeWeakSpotDamage(this.transform);
    }
}
