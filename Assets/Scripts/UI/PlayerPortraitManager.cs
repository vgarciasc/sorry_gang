using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerPortraitManager : MonoBehaviour
{
    public static PlayerPortraitManager GetPlayerPortraitManager() {
        return (PlayerPortraitManager) HushPuppy.safeFindComponent("PortraitManager", "PlayerPortraitManager");
    }
    
    [SerializeField]
    Image playerPortraitContainer;
    [SerializeField]
    Image playerPortrait;
    [SerializeField]
    Sprite playerNormalPortrait;
    [SerializeField]
    Sprite playerHurtPortrait;
    [SerializeField]
    Color hurtColor;

    Color originalContainerColor;

    void Start() {
        originalContainerColor = playerPortraitContainer.color;
    }

    public void TakeDamage() { StartCoroutine(TakeDamageCoroutine()); }
    IEnumerator TakeDamageCoroutine() {
        playerPortrait.sprite = playerHurtPortrait;
        playerPortraitContainer.color = hurtColor;
        yield return new WaitForSeconds(0.2f);
        playerPortrait.sprite = playerNormalPortrait;
        playerPortraitContainer.color = originalContainerColor;
    }
}
