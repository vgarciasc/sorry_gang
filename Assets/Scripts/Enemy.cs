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
    [SerializeField]
    protected List<Transform> weakSpots = new List<Transform>();
    [SerializeField]
    protected List<CardFlipScriptableObject> spellCards = new List<CardFlipScriptableObject>();

    protected CardEmitter cardEmitter;
    protected Player player;

    protected Vector3 originalScale;

    protected Flag tookWeakspotHit;
    protected Flag isInvincible;

    [SerializeField]
    private int maxHealth = 50;
    protected int health;

    // public delegate void changeHealthDelegate(int amount);
    // public event changeHealthDelegate changeHealthEvent;

    public delegate void deathDelegate();
    public event deathDelegate deathEvent;

    protected virtual void Start()
    {
        cardEmitter = this.GetComponentInChildren<CardEmitter>();
        cardEmitter.Start();

        originalScale = transform.localScale;
        health = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
            var playerBullet = obj.GetComponent<PlayerBullet>();
            TakeDamage(playerBullet.damage);
            Destroy(obj);
        }
    }

    public void TakeDamage(int amount) {
        if (isInvincible) return;

        health -= amount;
        if (health < 0 && deathEvent != null) {
            deathEvent();
        }

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
            this.transform.localScale -= Vector3.one * Time.deltaTime * 2f;
            yield return new WaitForEndOfFrame();
        }
    }

    Tween weakspotScaleTween;
    protected bool showingWeakSpot;
    protected IEnumerator ShowWeakSpot(Transform weakSpot, float duration) {
        showingWeakSpot = true;
        weakSpot.gameObject.SetActive(true);
        weakSpot.localScale = Vector3.one;
        
        var weakspotSr = weakSpot.GetComponent<SpriteRenderer>();
        float originalOpacity = weakspotSr.color.a;
        for (int i = 0; i < 5; i++) {
            weakspotSr.color = HushPuppy.getColorWithOpacity(weakspotSr.color, 0f);
            yield return new WaitForSeconds(0.05f);
            weakspotSr.color = HushPuppy.getColorWithOpacity(weakspotSr.color, originalOpacity);
            yield return new WaitForSeconds(0.05f);
        }

        weakspotScaleTween = weakSpot.DOScale(Vector3.zero, duration);
        yield return new WaitForSeconds(duration);
    }

    public void TakeWeakSpotDamage(Transform weakSpot) {
        showingWeakSpot = false;
        if (weakspotScaleTween != null) {
            weakspotScaleTween.Kill();
        }
        weakSpot.gameObject.SetActive(false);
        tookWeakspotHit = true;
    }

    protected virtual void StartNextPhase() {
        isInvincible = true;
    }

    public virtual void DieAfterFinalization() { }

    public float GetHealthPercentage() {
        return health / (float) maxHealth;
    }
}
