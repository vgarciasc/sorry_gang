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

    protected override void Start()
    {
        base.Start();

        StartCoroutine(BossLoop());
    }

    public override IEnumerator BossLoop() {
        // while (true) {
        //     var card = cards[0];
        //     yield return cardEmitter.playCard(card);
        // }

        yield return new WaitForSeconds(0.25f);

        // yield return Attack_1();
        // yield return Attack_2();
        yield return Attack_3();
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
}
