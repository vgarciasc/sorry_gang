using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCamera : MonoBehaviour
{
    Vector3 originalPos;

    public static SpecialCamera GetSpecialCamera() {
        return Camera.main.GetComponentInChildren<SpecialCamera>();
    }

    void Start() {
        originalPos = this.transform.localPosition;
    }
    
    #region Screen Shake
    public void screenShake_(float power, int durationFrames = 10) { StartCoroutine(screenShake(power, durationFrames)); }
    IEnumerator screenShake(float power, int durationFrames) {
        // if (power < 0.01f) {
            // power = 0.01f;
        // }

        for (int i = 0; i < durationFrames; i++) {
            yield return new WaitForEndOfFrame();
            float x = getScreenShakeDistance(power);
            float y = getScreenShakeDistance(power);

            this.transform.localPosition = new Vector3(originalPos.x + x,
                                                       originalPos.y + y,
                                                       originalPos.z);
        }

        this.transform.localPosition = originalPos;
    }

    float getScreenShakeDistance(float power) {
        float power_aux = power;
        int count = 0;
        while (true) {
            count++;
            float aux = Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(power_aux / 4, power_aux / 2);
            if (Mathf.Abs(aux) > 0.1f) {
                return aux;
            }
            if (count > 5) {
                count = 0;
                power_aux += 0.25f;
            }
        }
    }
    #endregion
}
