using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationComponent : MonoBehaviour
{
    [SerializeField]
    float ROTATION_SPEED;

    void Update() {
        var z = this.transform.rotation.eulerAngles.z;
        this.transform.rotation = Quaternion.Euler(
            0, 0, z + ROTATION_SPEED * Time.deltaTime);
    }
}
