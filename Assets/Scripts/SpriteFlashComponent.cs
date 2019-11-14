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

    public void Flash(bool inverted = false) {
        if (flashCoroutine != null) {
            StopCoroutine(flashCoroutine);
        }

        if (inverted) {
            flashCoroutine = StartCoroutine(DOFlashInverted());
        } else {
            flashCoroutine = StartCoroutine(DOFlash());
        }
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

    IEnumerator DOFlashInverted() {
        if (isFlashing) yield break;

		isFlashing = false;
		yield return new WaitForEndOfFrame();
		isFlashing = true;
		float flash = 0f;
		while (isFlashing && flash <= 1f)
		{
			flash += Time.deltaTime * flashSpeed;
			sr.material.SetFloat("_FlashAmount", flash);
			yield return null;
		}
		isFlashing = false;
    }

    public void ResetState() {
        SetFlash(0f);
    }

    public void SetFlash(float value) {
        sr.material.SetFloat("_FlashAmount", value);
    }
}
