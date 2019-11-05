using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletSelection { ALL_BULLETS, SHOT_BULLETS, INTERLACED_THREAD_BULLETS,
							  INTERLACED_WAVE_BULLETS, INTERLACED_BULLET_BULLETS,
							  EARLIEST_WAVE_BULLETS, CARD_BULLETS }
public enum BulletAction { PLAYER_DIRECTION, PLAYER_CONSTANT_DIRECTION,
							EMITTER_DIRECTION, EMITTER_CONSTANT_DIRECTION,
							DEACCELERATION }

[CreateAssetMenu(fileName = "Data", menuName = "Bullet Event", order = 1)]
public class ShotEventData : ScriptableObject {
	
	[HeaderAttribute("Core Attributes")]
	[TooltipAttribute("Which bullets will it affect?")]
	public BulletSelection affectsWhichBullets;
	
	[TooltipAttribute("The event action.")]
	public BulletAction action;

	[HeaderAttribute("Misc")]
	[RangeAttribute(0, 5)]
	[TooltipAttribute("Initial offset to start counting.")]
	public int startCountingOn = 0;

	[RangeAttribute(1, 5)]
	[TooltipAttribute("If it's interlaced threads, select one thread each N threads. Etc.")]
	public int countEachNElements = 1;

	[RangeAttribute(-5f, 5f)]
	[TooltipAttribute("Acceleration to be applied when appropriate.")]
	public float acceleration = 0;

	[RangeAttribute(0, 5f)]
	[TooltipAttribute("Deacceleration to be applied when appropriate.")]
	public float deacceleration = 0;
}
