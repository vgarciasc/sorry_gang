using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEventManager : MonoBehaviour {
	List<BulletDeluxe> bullets;

	public static BulletEventManager getBulletEventManager() {
		return (BulletEventManager) HushPuppy.safeFindComponent("GameController", "BulletEventManager");
	}

	void Start() {
		bullets = new List<BulletDeluxe>();
	}

	public void addBullet(BulletDeluxe bullet) {
		bullets.Add(bullet);
		bullet.destroy_event += removeBullet;
	}

	public void removeBullet(BulletDeluxe bullet) {
		bullets.Remove(bullet);
	}

	public void fileEvent(ShotEventData data, float delay, int shot_ID, int card_ID) {
		StartCoroutine(start_event(data, delay, shot_ID, card_ID));
	}

	IEnumerator start_event(ShotEventData data, float delay, int shot_ID, int card_ID) {
		List<BulletDeluxe> bullets;
		
		if (data == null) {
			Debug.Log("Event not set. Check scriptable object for event.");
			Debug.Break();
			yield return null;
		}

		yield return new WaitForSeconds(delay);
		
		switch (data.affectsWhichBullets) {
			default:
			case BulletSelection.ALL_BULLETS:
				bullets = getAllBullets();
				break;
			case BulletSelection.SHOT_BULLETS:
				bullets = getShotBullets(shot_ID);
				break;
			case BulletSelection.INTERLACED_THREAD_BULLETS:
				bullets = getInterlacedThreadBullets(shot_ID,
							data.countEachNElements,
							data.startCountingOn);
				break;
			case BulletSelection.INTERLACED_WAVE_BULLETS:
				bullets = getInterlacedWaveBullets(shot_ID,
							data.countEachNElements,
							data.startCountingOn);
				break;
			case BulletSelection.INTERLACED_BULLET_BULLETS:
				bullets = getInterlacedBulletBullets(shot_ID,
							data.countEachNElements,
							data.startCountingOn);
				break;
			case BulletSelection.EARLIEST_WAVE_BULLETS:
				bullets = getEarliestWaveBullets(shot_ID);
				break;
			case BulletSelection.CARD_BULLETS:
				bullets = getCardBullets(card_ID);
				break;
		}

		switch (data.action) {
			default:
			case BulletAction.PLAYER_DIRECTION:
				sendPlayerDirection(bullets, data.acceleration);
				break;
			case BulletAction.PLAYER_CONSTANT_DIRECTION:
				sendPlayerConstantDirection(bullets, data.acceleration);
				break;
			case BulletAction.EMITTER_DIRECTION:
				sendEmitterDirection(bullets, data.acceleration);
				break;
			case BulletAction.EMITTER_CONSTANT_DIRECTION:
				sendEmitterConstantDirection(bullets, data.acceleration);
				break;
			case BulletAction.DEACCELERATION:
				sendDeacceleration(bullets, data.deacceleration);
				break;
		}
	}

	#region executing actions
	void sendPlayerDirection(List<BulletDeluxe> bullets, float magnitude) {
		for (int i = 0; i < bullets.Count; i++) {
			bullets[i].addAccelerationPlayerDirection(magnitude);
		}
	}
	void sendPlayerConstantDirection(List<BulletDeluxe> bullets, float magnitude) {
		for (int i = 0; i < bullets.Count; i++) {
			bullets[i].addConstantAccelerationPlayerDirection(magnitude);
		}
	}

	void sendEmitterDirection(List<BulletDeluxe> bullets, float magnitude) {
		for (int i = 0; i < bullets.Count; i++) {
			bullets[i].addAccelerationEmitterDirection(magnitude);
		}
	}

	void sendEmitterConstantDirection(List<BulletDeluxe> bullets, float magnitude) {
		for (int i = 0; i < bullets.Count; i++) {
			bullets[i].addConstantAccelerationEmitterDirection(magnitude);
		}
	}

	void sendDeacceleration(List<BulletDeluxe> bullets, float magnitude) {
		for (int i = 0; i < bullets.Count; i++) {
			bullets[i].deaccelerate(magnitude);
		}
	}
	#endregion

	#region getting bullets
	public List<BulletDeluxe> getAllBullets() {
		return bullets;
	}

	public List<BulletDeluxe> getShotBullets(int shot_ID) {
		List<BulletDeluxe> aux = new List<BulletDeluxe>();
		
		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].shot_ID == shot_ID) {
				aux.Add(bullets[i]);
			}
		}

		return aux;
	}

	public List<BulletDeluxe> getCardBullets(int card_ID) {
		List<BulletDeluxe> aux = new List<BulletDeluxe>();
		
		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].card_ID == card_ID) {
				aux.Add(bullets[i]);
			}
		}

		return aux;
	}

	public List<BulletDeluxe> getInterlacedThreadBullets(int shot_ID, int each_n, int offset) {
		List<BulletDeluxe> aux = new List<BulletDeluxe>();
		
		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].shot_ID == shot_ID &&
				bullets[i].thread_ID % each_n == offset % each_n) {
				aux.Add(bullets[i]);
			}
		}

		return aux;
	}

	public List<BulletDeluxe> getInterlacedWaveBullets(int shot_ID, int each_n, int offset) {
		List<BulletDeluxe> aux = new List<BulletDeluxe>();
		
		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].shot_ID == shot_ID &&
				bullets[i].wave_ID % each_n == offset % each_n) {
				aux.Add(bullets[i]);
			}
		}

		return aux;
	}

	public List<BulletDeluxe> getInterlacedBulletBullets(int shot_ID, int each_n, int offset) {
		List<BulletDeluxe> aux = new List<BulletDeluxe>();
		
		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].shot_ID == shot_ID &&
				bullets[i].bullet_ID % each_n == offset % each_n) {
				aux.Add(bullets[i]);
			}
		}

		return aux;
	}

	public List<BulletDeluxe> getEarliestWaveBullets(int shot_ID) {
		List<BulletDeluxe> aux = new List<BulletDeluxe>();
		
		int first_wave_ID = 0;
		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].shot_ID == shot_ID ) {
				if (bullets[i].wave_ID < first_wave_ID) {
					first_wave_ID = bullets[i].wave_ID;
				}
			}
		}

		for (int i = 0; i < bullets.Count; i++) {
			if (bullets[i].shot_ID == shot_ID &&
				bullets[i].wave_ID == first_wave_ID) {
				aux.Add(bullets[i]);
			}
		}

		return aux;
	}
	#endregion
}
