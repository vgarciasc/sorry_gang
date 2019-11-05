using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDeluxe : MonoBehaviour {
	public int bullet_ID = -1,
			thread_ID = -1,
			wave_ID = -1,
			loop_ID = -1,
			shot_ID = -1,
			card_ID = -1;
	
	BulletData data;
	Rigidbody2D rb;
	Transform player;

	float speed;
	float angle_grad;
	float deacceleration;

	bool sineMovement;
	bool cosineMovement;
	float sinusoidalAmplitude,
		sinusoidalPeriod;

	bool shouldStopWhenVelocityZero;

	Vector2 acceleration;
	Vector2 acceleration_senoid_y;
	Vector2 acceleration_player_direction;
	Vector2 acceleration_emitter_direction;

	Vector3 start_position;
	Vector3 tangent;

	bool player_direction;
	float player_direction_magnitude;

	bool emitter_direction;
	float emitter_direction_magnitude;

	float timeSlowModifier = 1f;

	float framecount;

	public delegate void BulletDeath(BulletDeluxe bullet);
	public event BulletDeath destroy_event;

	void start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();

		framecount = 0;
		deacceleration = 1;
		acceleration = acceleration_senoid_y = Vector2.zero;
		acceleration_player_direction = acceleration_emitter_direction = Vector2.zero;
		player_direction = emitter_direction = false;
		player_direction_magnitude = emitter_direction_magnitude = 0;

		rb.velocity = Vector2.zero;
		rb.drag = 0;
	}

	void FixedUpdate() {
		if (sineMovement || cosineMovement) {
			framecount++;

			sinusoidalAmplitude += data.amplitudeAcceleration * Time.deltaTime;
			sinusoidalPeriod += data.periodAcceleration * Time.deltaTime;

			acceleration_senoid_y = new Vector2(0, sinusoidalAmplitude * Mathf.Cos(framecount / (sinusoidalPeriod * 2)));
			acceleration_senoid_y = HushPuppy.rotateVector(acceleration_senoid_y, angle_grad + 90);

			if (sineMovement) acceleration_senoid_y *= -1;
		}

		if (player_direction) {
			acceleration_player_direction = (player.position - this.transform.position).normalized * player_direction_magnitude;
		}

		if (emitter_direction) {
			acceleration_emitter_direction = (start_position - this.transform.position).normalized * emitter_direction_magnitude;
		}

		rb.velocity += (acceleration +
						acceleration_senoid_y + 
						acceleration_player_direction + 
						acceleration_emitter_direction)
						* Time.deltaTime
						* timeSlowModifier;

		if (rb.velocity.sqrMagnitude > data.maxVelocity) {
			rb.velocity = rb.velocity.normalized * data.maxVelocity;
		}

		if (rb.velocity.magnitude < 0.5 && shouldStopWhenVelocityZero) {
			rb.velocity = acceleration = Vector2.zero;
		}

		if (data.faceVelocity) {
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			this.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
		}
	}

	#region setters
	public void setData(BulletData bullet_data, Transform player) {
		this.data = Instantiate(bullet_data);
		this.sinusoidalAmplitude = data.amplitude;
		this.sinusoidalPeriod = data.period;
		this.player = player;

		// this.GetComponent<AnimationOverrider>().setAnimation(bullet_data.animation);
	}

	public void setSprite(Sprite sprite) {
		this.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
	}

	public void setColor(Color color) {
		this.GetComponentInChildren<SpriteRenderer>().color = color;
	}

	public void setSize(float size) {
		this.transform.localScale = new Vector3(size, size, size);
	}

	public void setPosition(Vector3 position) {
		this.transform.position = start_position = position;
	}

	public void setRotation(float angle_grad) {
		this.angle_grad = angle_grad;
		this.transform.rotation = Quaternion.identity;
		this.transform.Rotate(new Vector3(0f, 0f, angle_grad));
	}

	public void setVelocity(Vector2 velocity) {
		if (rb == null) {
			start();
		}
		rb.velocity = velocity;
	}

	public void setSpeed(float speed) {
		this.speed = speed;
		rb.velocity = this.transform.up * speed * timeSlowModifier;
	}

	public void setAngularVelocity(float angle) {
		rb.angularVelocity = angle;
	}
	#endregion

	#region creating and destroying
	public void activate() {
		this.gameObject.SetActive(true);
		start();
	}

    public void destroy() {
		if (destroy_event != null) {
			destroy_event(this);
		}

		// this.GetComponent<Animator>().SetTrigger("destroy");
		AnimDestroy();
    }

	void AnimDestroy() {
        this.gameObject.SetActive(false);
	}
	#endregion

	#region colliders
	public void OnTriggerExit2D(Collider2D target) {
		if (target.gameObject.tag == "Arena") {
			destroy();
		}
    }
	#endregion

	#region changing motion
	#region sinusoidal motion

	public void startCosineMovement() {
		cosineMovement = true;
		rb.velocity = new Vector2(this.transform.up.x, this.transform.up.y);
	}

	public void startSineMovement() {
		sineMovement = true;
		rb.velocity = new Vector2(this.transform.up.x, this.transform.up.y);
	}

	public void endSinusoidalMovement() {
		sineMovement = false;
		cosineMovement = false;
	}

	#endregion

	public void addConstantAccelerationEmitterDirection(float magnitude) {
		stopDeacceleration();
		emitter_direction = true;
		emitter_direction_magnitude = magnitude;
	}

	public void addConstantAccelerationPlayerDirection(float magnitude) {
		stopDeacceleration();
		player_direction = true;
		player_direction_magnitude = magnitude;
	}

	public void addAccelerationEmitterDirection(float magnitude) {
		stopDeacceleration();
		acceleration_emitter_direction = (start_position - this.transform.position).normalized * magnitude;
	}

	public void setAccelerationEmitterDirection(float magnitude) {
		stopDeacceleration();
		rb.velocity = (start_position - this.transform.position).normalized * magnitude;
	}

	public void addAccelerationPlayerDirection(float magnitude) {
		stopDeacceleration();
		acceleration_player_direction = (player.position - this.transform.position).normalized * magnitude;
	}

	public void setAccelerationPlayerDirection(float magnitude) {
		stopDeacceleration();
		rb.velocity = (player.position - this.transform.position).normalized * magnitude;
	}

	public void toggleStopWhenVelocityZero(bool value) {
		shouldStopWhenVelocityZero = value;
	}

	public void deaccelerate(float magnitude) {
		rb.drag = magnitude;
	}

	public void stopDeacceleration() {
		rb.drag = 0;
	}
	#endregion

	#region time shenanigans

	bool firstTimeSlow = true;
	public void TimeSlow(float modifier) {
		if (firstTimeSlow) {
			if (rb != null) {
				rb.velocity *= modifier;
			}
			else {
				GetComponentInChildren<Rigidbody2D>().velocity *= modifier;
			}
			timeSlowModifier = modifier;
			firstTimeSlow = false;
		}
	}

	public void ResetTimeSlow() {
		if (rb != null) {
			rb.velocity /= timeSlowModifier;
		}
		else {
			GetComponentInChildren<Rigidbody2D>().velocity /= timeSlowModifier;
		}
		firstTimeSlow = true;
		timeSlowModifier = 1f;
	}

	#endregion
}
