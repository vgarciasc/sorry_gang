using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using DG.Tweening;
using TMPro;

public class TitleSceneManager : MonoBehaviour
{
    public Animator animator;
    public TextTyper twoFactsTextTyper;
    public TextTyper fact1;
    public TextTyper fact2;
    public TextTyper titledrop;

    public string currentSignal;

    void Start()
    {
        twoFactsTextTyper.gameObject.SetActive(false);
        fact1.GetComponent<TextMeshProUGUI>().text = "";
        fact2.GetComponent<TextMeshProUGUI>().text = "";
        titledrop.GetComponent<TextMeshProUGUI>().text = "";
        StartCoroutine(Manage());
    }

    IEnumerator Manage() {
        //two facts
        yield return new WaitForSeconds(1f);
        twoFactsTextTyper.gameObject.SetActive(true);
        var twoFactsOriginalScale = twoFactsTextTyper.transform.localScale;
        twoFactsTextTyper.transform.localScale = twoFactsOriginalScale * 1.5f;
        yield return new WaitForSeconds(0.02f);
        twoFactsTextTyper.transform.DOScale(twoFactsOriginalScale, 0.1f);
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitForSeconds(0.5f);

        //fact 1
        fact1.TypeText("<color=#666>1.</color> You are the legendary assassin <color=#FDA440>Princesa Lanoche</color>.",
            0.05f);
        yield return new WaitUntil(() => currentSignal == "fact1");

        yield return new WaitForSeconds(0.5f);

        //fact 2
        fact2.TypeText(
            "<color=#666>2.</color> You have <color=#FDA440>one job</color>.",
            0.05f);
        yield return new WaitUntil(() => currentSignal == "fact2");
        
        yield return new WaitForSeconds(0.5f);

        //screen transition
        animator.SetTrigger("falldown");
        yield return new WaitUntil(() => currentSignal == "falldown_finish");

        yield return new WaitForSeconds(0.5f);

        //sorry gang
        titledrop.TypeText("sorry, gang.", 0.05f);
        yield return new WaitUntil(() => currentSignal == "sorrygang_finish");
        
        yield return new WaitForSeconds(0.5f);

        //show grid
        animator.SetTrigger("showgrid");
    }

    public void Signal(string sig) {
        this.currentSignal = sig;
    }
}
