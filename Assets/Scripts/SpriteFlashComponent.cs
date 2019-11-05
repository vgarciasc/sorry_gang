using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlashComponent : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    float flashSpeed = 1;

    Coroutine flashCoroutine;
    bool isFlashing;

    public void Flash() {
        if (flashCoroutine != null) {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(DOFlash());
    }

    IEnumerator DOFlash() {
		isFlashing = false;
		yield return new WaitForEndOfFrame();
		isFlashing = true;
		float flash = 1f;
		while (isFlashing && flash >=0)
		{
			flash -= Time.deltaTime * flashSpeed;
			sr.material.SetFloat("_FlashAmount", flash);
			yield return null;
		}
		isFlashing = false;
    }
}
