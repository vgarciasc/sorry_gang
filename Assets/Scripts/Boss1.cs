using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss1 : Enemy
{
    [SerializeField]
    Transform possiblePositionsContainer;

    public GameObject attack_1_prefab;
    public GameObject attack_2_1_prefab;
    public GameObject attack_2_2_prefab;
    public GameObject attack_3;
    public GameObject attack_4_prefab;
    public GameObject attack_5;

    Coroutine currentPhase;

    protected override void Start()
    {
        base.Start();

        deathEvent += StartNextPhase;

        // StartCoroutine(BossLoop());
        currentPhase = StartCoroutine(BossPhase1Loop());
    }
    
    void Update() {
        if (tookWeakspotHit) {
            tookWeakspotHit = false;
            if (currentPhase != null) {
                StopCoroutine(currentPhase);
            }

            FinishManager.GetFinishManager().StartFinalization();

            // print("Starting next phase...");
            // StartCoroutine(BossPhase1Loop());
        }
    }

    private IEnumerator BossPhase1Loop() {
        while (true) {
            // yield return Attack_1();
            // yield return Attack_2();
            // yield return Attack_3();
            yield return Attack_4();
            // yield return Attack_5();

            var card = cards[0];
            yield return cardEmitter.playCard(card);

            yield return new WaitForSeconds(1f);

            // if (this.health <= 0) {
            //     StartCoroutine(ShowWeakSpot(weakSpots[0], 15f));
            // }

            // yield return GetNextAttack();

            // yield return new WaitForSeconds(2f);

            // yield return new WaitWhile(() => showingWeakSpot);
            // StartCoroutine(ShowWeakSpot(weakSpots[0], 10f));
        }
    }

    protected override void StartNextPhase() {
        base.StartNextPhase();
        StartCoroutine(ShowWeakSpot(weakSpots[0], 10f));
    }

    public override void DieAfterFinalization() { StartCoroutine(DieAfterFinalizationCoroutine()); }
    
    IEnumerator DieAfterFinalizationCoroutine() {
        float duration = 3f;
        this.transform.DOScale(Vector3.zero, duration);
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }

    int lastAttackIndex = -1;
    IEnumerator GetNextAttack() {
        int dice = -1;
        do {
            dice = UnityEngine.Random.Range(0, 5);
        } while (dice == -1 && dice == lastAttackIndex);

        lastAttackIndex = dice;
        
        switch (dice) {
            case 0: yield return Attack_1(); break;
            case 1: yield return Attack_2(); break;
            case 2: yield return Attack_3(); break;
            case 3: yield return Attack_4(); break;
            case 4: yield return Attack_5(); break;
        }

        yield break;
    }

    private IEnumerator MoveToPosition(int index) {
        Vector3 target = possiblePositionsContainer.GetChild(index).position;

        float velocity = 2f;
        float distance = Vector2.Distance(transform.position, target);

        yield return this.transform.DOMove(target, distance / velocity)
            .SetEase(Ease.InBounce);
        yield return new WaitForSeconds(0.5f);
    }

    //following pans
    private IEnumerator Attack_1() {
        CardFlipManager.GetCardFlipManager().SetNewCard(spellCards[0]);
        yield return MoveToPosition(1);

        var posContainer = worldPointsContainer.GetChild(0);

        int N = UnityEngine.Random.Range(3, 6);
        float dur = UnityEngine.Random.Range(0.5f, 0.8f);

        for (int j = 0; j < N; j++) {
            for (int i = 0; i < 3; i++) {
                var obj = Instantiate(
                    attack_1_prefab,
                    posContainer.GetChild(i));
                var proj = obj.GetComponentInChildren<BasicProjectile>();
                proj.SetVelocity(Vector2.down);
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(dur);
        }

        
        yield break;    
    }

    //watermelon
    private IEnumerator Attack_2() {
        CardFlipManager.GetCardFlipManager().SetNewCard(spellCards[1]);
        yield return MoveToPosition(1);

        int initialShotQuantity = 2;
        int divisionQuantity = 6;

        var posContainer = worldPointsContainer.GetChild(1);

        var projs_1 = new List<BasicProjectile>();

        for (int i = 0; i < initialShotQuantity; i++) {
            var obj = Instantiate(
                attack_2_1_prefab,
                posContainer.GetChild(i));
            var proj = obj.GetComponentInChildren<BasicProjectile>();
            projs_1.Add(proj);
            proj.SetVelocity(Vector2.down);
        }

        yield return new WaitForSeconds(2f);

        var projs_2 = new List<BasicProjectile>();
        
        for (int i = 0; i < initialShotQuantity; i++) {
            for (int j = 0; j < divisionQuantity; j++) {
                var angle = j * (360f / divisionQuantity);
                var obj = Instantiate(
                    attack_2_2_prefab,
                    projs_1[i].transform.position,
                    Quaternion.Euler(0, 0, angle));
                var proj = obj.GetComponentInChildren<BasicProjectile>();
                projs_2.Add(proj);
                proj.SetVelocity(obj.transform.up);
            }

            Destroy(projs_1[i].gameObject);
        }

        yield break;    
    }

    bool is_attack_3_in_duration = false;

    //belly of the beast
    private IEnumerator Attack_3() {
        CardFlipManager.GetCardFlipManager().SetNewCard(spellCards[2]);
        yield return MoveToPosition(2);

        attack_3.SetActive(true);
        StartCoroutine(Attack_3_Cooldown());
        yield return new WaitUntil(() => 
            Vector2.Distance(player.transform.position, transform.position) < 0.5f
            || !is_attack_3_in_duration);
        attack_3.SetActive(false);

        if (Vector2.Distance(player.transform.position, transform.position) < 0.5f) {
            // inside belly
            player.ToggleBlockInput(true);
            player.ToggleVisibility(false);
            
            yield return new WaitForSeconds(2f);

            // spit out, unconscious
            player.ToggleVisibility(true);
            player.SetVelocity(Vector2.down * 10f);
            player.ToggleFreezeRotation(false);
            player.SetAngularVelocity(720f);

            yield return new WaitForSeconds(1f);
            
            // conscious regained
            player.ToggleFreezeRotation(true);
            player.SetVelocity(Vector3.zero);
            player.ToggleBlockInput(false);
        }
    }

    private IEnumerator Attack_3_Cooldown() {
        is_attack_3_in_duration = true;
        yield return new WaitForSeconds(3f);
        is_attack_3_in_duration = false;
    }

    //lollipop rain
    private IEnumerator Attack_4() {
        yield return MoveToPosition(1);
        CardFlipManager.GetCardFlipManager().SetNewCard(spellCards[3]);
        var posContainer = worldPointsContainer.GetChild(2);

        Vector3 firstPos = posContainer.GetChild(0).position;
        Vector3 lastPos = posContainer.GetChild(posContainer.childCount - 1).position;

        int M = UnityEngine.Random.Range(4, 7);

        for (int k = 0; k < M; k++) {
            var projs = new List<BasicProjectile>();

            int N = UnityEngine.Random.Range(6, 10);

            for (int i = 0; i < N; i++) {
                var obj = Instantiate(
                    attack_4_prefab,
                    new Vector3(
                        firstPos.x + (lastPos.x - firstPos.x) * i / (float) (N - 1),
                        firstPos.y + (lastPos.y - firstPos.y) * i / (float) (N - 1),
                        0),
                    Quaternion.identity);
                var proj = obj.GetComponentInChildren<BasicProjectile>();
                proj.SetVelocity(Vector2.down);
                projs.Add(proj);

                yield return new WaitForSeconds(0.1f);
            }

            foreach (var proj in projs) {
                yield return new WaitForSeconds(0.05f);
                proj.SetAcceleration(15f);
            }
        }
    }

    //oven fire
    private IEnumerator Attack_5() {
        yield return MoveToPosition(0);
        CardFlipManager.GetCardFlipManager().SetNewCard(spellCards[4]);

        attack_5.SetActive(true);

        attack_5.transform.DOScaleX(1f, 0.05f);

        float animDuration = 0.1f;
        float totalDuration = 2f;
        for (int i = 0; i < (int) totalDuration / animDuration; i++) {
            attack_5.transform.DOScaleX(1.1f, animDuration);
            yield return new WaitForSeconds(animDuration);
            attack_5.transform.DOScaleX(1f, animDuration);
            yield return new WaitForSeconds(animDuration);
        }

        attack_5.transform.DOScaleX(0f, 0.2f);
        yield return new WaitForSeconds(0.2f);

        attack_5.SetActive(false);
    }
}
