using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected List<CardPatternData> cards = new List<CardPatternData>();
    [SerializeField]
    protected Transform worldPointsContainer;
    
    protected CardEmitter cardEmitter;
    protected Player player;

    protected Vector3 originalScale;

    protected virtual void Start()
    {
        cardEmitter = this.GetComponentInChildren<CardEmitter>();
        cardEmitter.Start();

        originalScale = transform.localScale;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        
    }

    public virtual IEnumerator BossLoop() {
        while (true) {
            var card = cards[0];
            yield return cardEmitter.playCard(card);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collider) {
        var obj = collider.gameObject;

        if (obj.CompareTag("PlayerBullet")) {
            Destroy(obj);
            TakeDamage();
        }
    }

    protected void TakeDamage() {
        var sr = GetComponent<SpriteRenderer>();
        foreach (var sfc in GetComponentsInChildren<SpriteFlashComponent>()) {
            sfc.Flash();
        }

        if (sploshCoroutine != null) {
            StopCoroutine(sploshCoroutine);
        }

        sploshCoroutine = StartCoroutine(DOSplosh());
        
        // transform.localScale = originalScale * 1.2f;
        // transform.DOScale(originalScale, 0.2f);
    }

    Coroutine sploshCoroutine;

    IEnumerator DOSplosh() {
        this.transform.localScale = originalScale * 1.2f;
        while (Vector3.Distance(originalScale, this.transform.localScale) > 0.05f) {
            this.transform.localScale -= Vector3.one * Time.deltaTime * 1f;
            yield return new WaitForEndOfFrame();
        }
    }
}
