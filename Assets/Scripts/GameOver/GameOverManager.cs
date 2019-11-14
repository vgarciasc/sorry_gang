using System.Collections;
using System.Collections.Generic;
using RedBlueGames.Tools.TextTyper;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameOverManager : MonoBehaviour
{
    public TextTyper typer;
    public CanvasGroup button;

    void Start() {
        typer.GetComponent<TextMeshProUGUI>().text = "";
        button.alpha = 0f;
        StartCoroutine(Manage());
    }

    IEnumerator Manage() {
        yield return new WaitForSeconds(1f);
        typer.TypeText("You have <color=#FDA440>one job</color>.");
        yield return new WaitWhile(() => typer.IsTyping);
        button.DOFade(1f, 0.5f);
    }
}
