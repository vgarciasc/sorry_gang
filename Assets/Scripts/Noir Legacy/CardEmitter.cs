using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEmitter : MonoBehaviour {
	[HeaderAttribute("Testing Cards")]
	public CardPatternData testCard;
	public Transform emitter;
	[SerializeField]
	bool startTestingCard = true;

	protected BulletPoolManager pool;
	protected BulletEventManager event_manager;
	Transform player;

	Coroutine shootingCoroutine;
	Coroutine playingCardCoroutine;

	public bool isShooting;

	public void Start () {
		pool = BulletPoolManager.getBulletPoolManager();
		event_manager = BulletEventManager.getBulletEventManager();
		player = GameObject.FindGameObjectWithTag("Player").transform;

		if (startTestingCard) {
			PlayCard(testCard, emitter, -1);
		}
	}

	void Update() {
		isShooting = shootingCoroutine != null;
	}

	public void PlayCard(CardPatternData card, Transform emitter, int card_ID) {
		if (playingCardCoroutine != null) {
			Debug.Log("WARNING: enemy is playing a card when there is already a card being played.");
		}

		playingCardCoroutine = StartCoroutine(playCard(card, emitter, card_ID));
	}

	public IEnumerator playCard(CardPatternData card) {
		yield return playCard(card, this.transform);
		yield return new WaitWhile(() => this.isShooting);
	}

	public IEnumerator playCard(CardPatternData card, Transform emitter, int card_ID = -1) {
		for (int i = 0; i < card.array.Length; i++) {
			ShotPatternData shot_data = Instantiate(card.array[i].shot);
			
			for (int j = 0; j < card.array[i].shot.loopQuantity; j++) {
				shootingCoroutine = StartCoroutine(shoot(shot_data, card.array[i].bullet, emitter, j, i, card_ID));
				checkShotPatternData(shot_data);

				for (int k = 0; k < (card.array[i].shot.delayBetweenLoops / Time.timeScale) &&
								j % card.array[i].shot.delayAppliedEachNLoops == card.array[i].shot.delayAppliedEachNLoops - 1; k++)
                	yield return new WaitForEndOfFrame();
			}

			for (int h = 0; h < card.array[i].events.Length; h++) {
				event_manager.fileEvent(card.array[i].events[h].shotEvent,
										card.array[i].events[h].delayBeforeEvent,
										i,
										card_ID);
			}

			yield return new WaitForSeconds(card.array[i].delayAfter);
		}

		playingCardCoroutine = null;
	}

	public void stopCurrentCard() {
		if (shootingCoroutine != null) {
			StopCoroutine(shootingCoroutine);
		}
		
		if (playingCardCoroutine != null) {
			StopCoroutine(playingCardCoroutine);
		}
		
		playingCardCoroutine = null;
		shootingCoroutine = null;
		pool.destroyAllBullets();
	}

	IEnumerator shoot(ShotPatternData shot_data, BulletData bullet_data,
	 					Transform emitter, int loop_number, int shot_number, int card_number) {
        // float alternatingAngle = 360 / (shot_data.threadQuantity * 2);
		float bullet_angle = shot_data.angleOffset;
		if (shot_data.randomAngleOffset) {
			bullet_angle += Random.Range(0, 360f);
		}
		bullet_angle += emitter.rotation.z * 180;
		if (shot_data.playerDirection) {
			bullet_angle = Vector2.Angle(emitter.position - player.position, this.transform.up) + 220;
		}
		float original_bullet_angle = bullet_angle;

        int current_bullet_ID = 0;

        for (int i = 0; i < shot_data.waveQuantity; i++) {
			if (!shot_data.waveWrap) {
				bullet_angle = original_bullet_angle;
			}

			if (shot_data.randomAnglePerWave) {
				bullet_angle += Random.Range(0, 360f);
			}

			bullet_angle += shot_data.angleBetweenWaves;

			bullet_angle += shot_data.threadArcAngleIncrementBetweenWaves;

            for (int j = 0; j < shot_data.bulletQuantity; j++) {
				float bullet_angle_increment = shot_data.threadArc / shot_data.bulletQuantity;
				if ((shot_data.waveWrap && i % 2 == 0)) {
					bullet_angle_increment *= -1;
				}
				if (shot_data.clockwise) {
					bullet_angle_increment *= -1;
				}
				if (shot_data.reverseAlternatedLoops && loop_number % 2 == 0) {
					bullet_angle_increment *= -1;
				}

				bullet_angle += bullet_angle_increment;

                for (int h = 0; h < shot_data.threadQuantity; h++) {
                    current_bullet_ID++;

					float angle = bullet_angle + ((shot_data.angleBetweenThreads / shot_data.threadQuantity) * h);
					
					BulletDeluxe bullet = create_bullet(shot_data,
														bullet_data,
														emitter.position,
														angle);
					set_bullet_id(bullet, i, h, j, loop_number, shot_number, card_number);
					set_bullet_visuals(shot_data, bullet, i, h, j, loop_number);
					set_bullet_sinusoidal(shot_data, bullet, i, h, j, loop_number);
                }

                for (int k = 0; j % shot_data.delayAppliedEachNBullets == 0 &&
								k < (shot_data.delayBetweenBullets / Time.timeScale); k++)
                    yield return new WaitForEndOfFrame();
            }

            for (int k = 0; k < (shot_data.delayBetweenWaves / Time.timeScale); k++)
                yield return new WaitForEndOfFrame();
        }

		shootingCoroutine = null;
    }

	BulletDeluxe create_bullet(ShotPatternData shot_data, BulletData bullet_data, Vector3 position, float angle_grad) {
		BulletDeluxe bullet = pool.getNewBullet().GetComponent<BulletDeluxe>();
		bullet.activate();

		bullet.setData(bullet_data, player);

		// if (!bullet_data.faceVelocity) {
		// 	bullet.GetComponentInChildren<ComponentFixRotation>().enabled = true;
		// }

		bullet.setRotation(angle_grad);
		bullet.setPosition(position + bullet.transform.up * shot_data.radialOffset);
		bullet.setSpeed(shot_data.bulletSpeed);
		bullet.setAngularVelocity(shot_data.bulletAngularVelocity);

		event_manager.addBullet(bullet);

		return bullet;
	}

	void set_bullet_id(BulletDeluxe bullet, int wave_ID, int thread_ID, int bullet_ID, int loop_ID, int shot_ID, int card_ID) {
		bullet.wave_ID = wave_ID;
		bullet.loop_ID = loop_ID;
		bullet.bullet_ID = bullet_ID;
		bullet.thread_ID = thread_ID;
		bullet.shot_ID = shot_ID;
		bullet.card_ID = card_ID;
	}

	void set_bullet_visuals(ShotPatternData shot_data, BulletDeluxe bullet,
							int wave_ID, int thread_ID, int bullet_ID, int loop_ID) {
		Color color;
		Sprite sprite;
		float size;

		switch (shot_data.coloringStyle) {
			default:
			case ShotPatternVisualStyle.BULLET_INTERLACED:
				color = shot_data.colors[bullet_ID % shot_data.colors.Length];
				break;
			case ShotPatternVisualStyle.THREAD_INTERLACED:
				color = shot_data.colors[thread_ID % shot_data.colors.Length];
				break;
			case ShotPatternVisualStyle.WAVE_INTERLACED:
				color = shot_data.colors[wave_ID % shot_data.colors.Length];
				break;
			case ShotPatternVisualStyle.LOOP_INTERLACED:
				color = shot_data.colors[loop_ID % shot_data.colors.Length];
				break;
			case ShotPatternVisualStyle.RANDOM:
				color = shot_data.colors[(int) (Time.time) % shot_data.colors.Length];
				break;
		}

		switch (shot_data.spritingStyle) {	
			default:
			case ShotPatternVisualStyle.BULLET_INTERLACED:
				sprite = shot_data.sprites[bullet_ID % shot_data.sprites.Length];
				break;
			case ShotPatternVisualStyle.THREAD_INTERLACED:
				sprite = shot_data.sprites[thread_ID % shot_data.sprites.Length];
				break;
			case ShotPatternVisualStyle.WAVE_INTERLACED:
				sprite = shot_data.sprites[wave_ID % shot_data.sprites.Length];
				break;
			case ShotPatternVisualStyle.LOOP_INTERLACED:
				sprite = shot_data.sprites[loop_ID % shot_data.sprites.Length];
				break;
			case ShotPatternVisualStyle.RANDOM:
				sprite = shot_data.sprites[(int) (Time.time) % shot_data.sprites.Length];
				break;
		}

		switch (shot_data.sizingStyle) {	
			default:
			case ShotPatternVisualStyle.BULLET_INTERLACED:
				size = shot_data.sizes[bullet_ID % shot_data.sizes.Length];
				break;
			case ShotPatternVisualStyle.THREAD_INTERLACED:
				size = shot_data.sizes[thread_ID % shot_data.sizes.Length];
				break;
			case ShotPatternVisualStyle.WAVE_INTERLACED:
				size = shot_data.sizes[wave_ID % shot_data.sizes.Length];
				break;
			case ShotPatternVisualStyle.LOOP_INTERLACED:
				size = shot_data.sizes[loop_ID % shot_data.sizes.Length];
				break;
			case ShotPatternVisualStyle.RANDOM:
				size = shot_data.sizes[(int) (Time.time) % shot_data.sizes.Length];
				break;
		}

		bullet.setColor(color);
		bullet.setSprite(sprite);
		bullet.setSize(size);
	}

	void set_bullet_sinusoidal(ShotPatternData shot_data, BulletDeluxe bullet,
								int wave_ID, int thread_ID, int bullet_ID, int loop_ID) {
		switch (shot_data.sinusoidalMotion) {
			case ShotPatternSinusoidalStyle.ALL_COSINE:
				bullet.startCosineMovement();
				break;
			case ShotPatternSinusoidalStyle.ALL_SINE:
				bullet.startSineMovement();
				break;
			case ShotPatternSinusoidalStyle.THREAD_INTERLACED:
				if (thread_ID % 2 == 0) {
					bullet.startCosineMovement();
				}
				else {
					bullet.startSineMovement();
				}
				break;
			case ShotPatternSinusoidalStyle.WAVE_INTERLACED:
				if (wave_ID % 2 == 0) {
					bullet.startCosineMovement();
				}
				else {
					bullet.startSineMovement();
				}
				break;
			case ShotPatternSinusoidalStyle.LOOP_INTERLACED:
				if (loop_ID % 2 == 0) {
					bullet.startCosineMovement();
				}
				else {
					bullet.startSineMovement();
				}
				break;
			default: case ShotPatternSinusoidalStyle.NONE:
				break;
		}
	}

	void checkShotPatternData(ShotPatternData data) {
		if (data.colors.Length == 0) {
			Debug.Log("No bullet color chosen. ");
			Debug.Break();
		}
		if (data.colors.Length == 0) {
			Debug.Log("No bullet sprite chosen. ");
			Debug.Break();
		}
	}
}
