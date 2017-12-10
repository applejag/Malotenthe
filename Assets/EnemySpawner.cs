using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject meteoritePrefab;
	public GameObject[] enemyPrefabs;

	[Header("Count")]
	public int enemiesPerWaveLower = 5;
	public int enemiesPerWaveUpper = 10;
	[Header("Timing")]
	public float betweenEachMeteorite = 0.5f;
	public float betweenEachWave = 30f;
	public float remaingWaitTime = 10;
	[Header("Position")]
	public float spawnHeight = 100;

	private int enemiesLeft = 0;
	private float spawnDegrees = 0;
	[SerializeField, HideInInspector]
	private GameObject enemyPrefab;

	[SerializeField, HideInInspector]
	private PlayerController player;

	private void Awake()
	{
		player = FindObjectOfType<PlayerController>();
	}

	private void Start()
	{
		if (enemyPrefabs.Length == 0)
			this.enabled = false;
	}

	private void Update()
	{
		if (!player || player.IsDead)
		{
			this.enabled = false;
			return;
		}

		remaingWaitTime -= Time.deltaTime;
		if (!(remaingWaitTime <= 0)) return;

		// New wave
		if (enemiesLeft <= 0)
		{
			enemiesLeft = Random.Range(enemiesPerWaveLower, enemiesPerWaveUpper + 1);
			enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
			spawnDegrees = Random.Range(0f, 360f);
		}

		// Spawn from wave
		enemiesLeft--;
		Vector3 position = RingObject.RingPositionY(spawnDegrees, spawnHeight);
		Quaternion rotation = Quaternion.identity;
		GameObject clone = Instantiate(meteoritePrefab, position, rotation);
		var meteorite = clone.GetComponent<MeteoriteSpawner>();
		meteorite.prefab = enemyPrefab;

		if (enemiesLeft == 0)
			// End of wave
			remaingWaitTime += betweenEachWave;
		else
			remaingWaitTime += betweenEachMeteorite;
	}

}
