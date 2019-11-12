using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardFlipManager : MonoBehaviour
{
    public static CardFlipManager GetCardFlipManager() {
        return (CardFlipManager) HushPuppy.safeFindComponent("CardFlipManager", "CardFlipManager");
    }

    public Animator animator;

    public GameObject currentCardObject;

    public TextMeshProUGUI currentCardText;
    public TextMeshProUGUI nextCardText;

    public Image currentCardImage;
    public Image nextCardImage;

    CardFlipScriptableObject currentCard;

    bool firstTime = true;

    void Start() {
        currentCardObject.SetActive(false);
    }
    
    public void SetNewCard(CardFlipScriptableObject card) {
        currentCard = card;

        nextCardText.text = card.title;
        nextCardImage.sprite = card.sprite;

        animator.SetTrigger("flip");
    }

    public void FinishAnimation() {
        if (firstTime) {
            firstTime = false;
            currentCardObject.SetActive(true);
        }
        
        currentCardText.text = currentCard.title;
        currentCardImage.sprite = currentCard.sprite;
    }
}
