using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour {
	[SerializeField]
	GameObject bulletPrefab;
	
	public int pooledAmount;
	
	List<GameObject> bullets;

	public static BulletPoolManager getBulletPoolManager() {
		return (BulletPoolManager) HushPuppy.safeFindComponent("GameController", "BulletPoolManager");
	}

	void Start() {
		bullets = new List<GameObject>();
		for (int i = 0; i < pooledAmount; i++) {
			GameObject obj = Instantiate(bulletPrefab);
			obj.SetActive(false);
			bullets.Add(obj);
		}
	}

	public GameObject getNewBullet() {
		for (int i = 0; i < bullets.Count; i++) {
			if (!bullets[i].activeInHierarchy) {
				return bullets[i];
			}
		}
		
		return null;
	}

	public List<BulletDeluxe> getAllBullets() {
		List<BulletDeluxe> bulletScripts = new List<BulletDeluxe>();
		for (int i = 0; i < bullets.Count; i++) {
			bulletScripts.Add(bullets[i].GetComponent<BulletDeluxe>());
		}

		return bulletScripts;
	}

	public void destroyAllBullets() {
		GameObject[] aux = GameObject.FindGameObjectsWithTag("Bullet");
		for (int i = 0; i < aux.Length; i++) {
			if (aux[i].GetComponent<BulletDeluxe>() != null) {
				aux[i].GetComponent<BulletDeluxe>().destroy();
			}
		}
	}
}
