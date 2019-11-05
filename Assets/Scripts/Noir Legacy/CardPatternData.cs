using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventInfo {
	public ShotEventData shotEvent;	

	[RangeAttribute(0f, 5f)]
	public float delayBeforeEvent;
}

[System.Serializable]
public class CardInfo {
	public ShotPatternData shot;
	public BulletData bullet;
	[RangeAttribute(0f, 20f)]
	public float delayAfter = 0f;
	public EventInfo[] events;
}

[CreateAssetMenu(fileName = "Data", menuName = "Card Pattern", order = 1)]
public class CardPatternData : ScriptableObject {
	public string card_text_1, card_text_2, card_text_3;
	public bool show_card_text = false;
	
	public CardInfo[] array;
}
