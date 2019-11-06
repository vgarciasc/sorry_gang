using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss1 : Enemy
{
    [SerializeField]
    GameObject attack_1_prefab;
    [SerializeField]
    GameObject attack_2_1_prefab;
    [SerializeField]
    GameObject attack_2_2_prefab;
    [SerializeField]
    GameObject attack_3;
    [SerializeField]
    GameObject attack_4_prefab;
    [SerializeField]
    GameObject attack_5;

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
            print("Starting next phase...");
            StartCoroutine(BossPhase1Loop());
        }
    }

    private IEnumerator BossPhase1Loop() {
        while (true) {
            var card = cards[0];
            yield return cardEmitter.playCard(card);

            yield return new WaitForSeconds(1f);

            yield return GetNextAttack();

            // yield return Attack_1();
            // yield return Attack_2();
            // yield return new WaitForSeconds(2f);
            // yield return Attack_3();
            // yield return new WaitForSeconds(2f);
            // yield return Attack_4();
            // yield return Attack_5();

            // yield return new WaitForSeconds(2f);

            // yield return new WaitWhile(() => showingWeakSpot);
            // StartCoroutine(ShowWeakSpot(weakSpots[0], 10f));
        }
    }

    protected override void StartNextPhase() {
        base.StartNextPhase();
        StartCoroutine(StartNextPhaseCoroutine());
    }

    private IEnumerator StartNextPhaseCoroutine() {
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

    private IEnumerator Attack_1() {
        var posContainer = worldPointsContainer.GetChild(0);

        for (int i = 0; i < 3; i++) {
            var obj = Instantiate(
                attack_1_prefab,
                posContainer.GetChild(i));
            var proj = obj.GetComponentInChildren<BasicProjectile>();
            proj.SetVelocity(Vector2.down);
            yield return new WaitForSeconds(0.05f);
        }

        yield break;    
    }

    private IEnumerator Attack_2() {
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

        yield return new WaitForSeconds(0.2f);

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

    private IEnumerator Attack_3() {
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

    private IEnumerator Attack_4() {
        var posContainer = worldPointsContainer.GetChild(2);
        var projs = new List<BasicProjectile>();

        for (int i = 0; i < 6; i++) {
            var obj = Instantiate(
                attack_4_prefab,
                posContainer.GetChild(i));
            var proj = obj.GetComponentInChildren<BasicProjectile>();
            proj.SetVelocity(Vector2.down);
            projs.Add(proj);

            yield return new WaitForSeconds(0.2f);
        }

        foreach (var proj in projs) {
            yield return new WaitForSeconds(0.2f);
            proj.SetAcceleration(8f);
        }
    }

    private IEnumerator Attack_5() {
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
