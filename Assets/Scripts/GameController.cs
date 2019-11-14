using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour {
    public static GameController GetGameController() {
        return (GameController) HushPuppy.safeFindComponent("GameController", "GameController");
    }

    [SerializeField]
    CanvasGroup transitionScreen;

    public void TransitionToGameOverScreen() {
        transitionScreen.DOFade(1f, 1f).OnComplete(() => {
            SceneManager.LoadScene("GameOverScene");
        });
    }
}
