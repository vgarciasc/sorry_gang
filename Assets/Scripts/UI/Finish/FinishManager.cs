using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishManager : MonoBehaviour
{
    public static FinishManager GetFinishManager() {
        return (FinishManager) HushPuppy.safeFindComponent("FinishManager", "FinishManager");
    }

    public List<FinishToken> finishTokens;
    public GameObject container;
    public Animator animator;
    int currentIndex = -1;

    void Awake() {
        for (int i = 0; i < finishTokens.Count; i++) {
            finishTokens[i].SetTokenIndex(i);
            DeactivateToken(finishTokens[i]);
        }
    }

    public void StartFinalization() {
        GameObject.FindObjectOfType<Player>().ToggleFreeze(true);
        container.SetActive(true);
        ActivateToken(finishTokens[0]);
    }
    
    public void EndFinalization() {
        GameObject.FindObjectOfType<Player>().ToggleFreeze(false);
        container.SetActive(false);
    }

    public void SignalClick(int index) {
        currentIndex = index;

        if (index == finishTokens.Count - 1) {
            DeactivateToken(finishTokens[index]);
            //last token
            Success();
        } else {
            animator.SetTrigger("success");
            DeactivateToken(finishTokens[currentIndex]);
        }
    }

    public void StartNextToken() {
        if (currentIndex < finishTokens.Count - 1) {
            ActivateToken(finishTokens[currentIndex + 1]);
        } else {
            GameObject.FindObjectOfType<Player>().ToggleFreeze(false);
            GameObject.FindObjectOfType<Enemy>().DieAfterFinalization();
        }
    }

    void ActivateToken(FinishToken token) {
        token.gameObject.SetActive(true);
    }

    void DeactivateToken(FinishToken token) {
        token.gameObject.SetActive(false);
    }

    void Success() {
        animator.SetTrigger("final_success");
        print("Success!");
    }

    public void SignalFailure() {
        EndFinalization();
    }
}
