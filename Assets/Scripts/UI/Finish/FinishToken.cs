using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishToken : MonoBehaviour
{
    int index = -1;

    public Transform envelopingCircle;
    public float clickInterval;

    Tweener intervalTweener;
    Coroutine intervalCoroutine;

    bool isInInterval = false;

    void Awake() {
        intervalCoroutine = StartCoroutine(DoClickInterval());
    }

    public void SetTokenIndex(int index) {
        this.index = index;
    }
    
    IEnumerator DoClickInterval() {
        isInInterval = true;
        intervalTweener = envelopingCircle.DOScale(Vector3.one, clickInterval);
        yield return new WaitForSeconds(clickInterval);
        isInInterval = false;
        FinishManager.GetFinishManager().SignalFailure();
    }

    public void OnClick() {
        if (isInInterval) {
            if (intervalCoroutine != null) {
                StopCoroutine(intervalCoroutine);
            }

            if (intervalTweener != null) {
                intervalTweener.Kill();
            }

            FinishManager.GetFinishManager().SignalClick(this.index);
        }
    }
}
