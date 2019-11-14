using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField]
    Image healthFillImage;

    void Update()
    {
        healthFillImage.fillAmount = FindObjectOfType<Enemy>().GetHealthPercentage();
    }
}
