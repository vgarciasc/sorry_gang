using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Bullet Data", order = 1)]
public class BulletData : ScriptableObject {
	[HeaderAttribute("General Behaviour")]
	public AnimationClip animation;

	[HeaderAttribute("Visual Attributes")]
	[Tooltip("Will the sprite rotate in the direction of the bullet?")]
	public bool faceVelocity = true;

	[HeaderAttribute("Sinusoidal Motion Attributes")]
	[RangeAttribute(0f, 10f)]
	[Tooltip("The amplitude of the sine movement of the bullet.")]
	public float amplitude = 5;
	
	[RangeAttribute(-5f, 5f)]
	public float amplitudeAcceleration = 0;
	
	[RangeAttribute(1f, 10f)]
	[Tooltip("The frequency of the sine movement of the bullet.")]
	public float period = 5;
	
	[RangeAttribute(-5f, 5f)]
	public float periodAcceleration = 0;

	[HeaderAttribute("Velocity Attributes")]
	[RangeAttribute(0f, 20f)]
	public float maxVelocity = 20f;
	//stop when reach position
}
