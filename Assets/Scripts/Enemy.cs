using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected List<CardPatternData> cards = new List<CardPatternData>();
    [SerializeField]
    protected Transform worldPointsContainer;
    
    protected CardEmitter cardEmitter;
    protected Player player;

    protected virtual void Start()
    {
        cardEmitter = this.GetComponentInChildren<CardEmitter>();
        cardEmitter.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        
    }

    public virtual IEnumerator BossLoop() {
        while (true) {
            var card = cards[0];
            yield return cardEmitter.playCard(card);
        }
    }
}
