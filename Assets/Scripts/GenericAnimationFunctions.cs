using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnimationFunctions : MonoBehaviour
{
    public GameObject target;
    
    public bool shouldDestroyAfterTime = false;
    public float destroyAfterXSeconds = 0f;

    void Start() {
        if (shouldDestroyAfterTime) {
            StartCoroutine(DestroyAfterTime());
        }
    }

    IEnumerator DestroyAfterTime() {
        yield return new WaitForSeconds(this.destroyAfterXSeconds);
        Destroy(this.gameObject);
    }

    public void Finish_EndAnimation() {
        target.GetComponent<FinishManager>().StartNextToken();
    }
}
