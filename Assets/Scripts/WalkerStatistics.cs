using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RingWalker))]
public class WalkerStatistics : MonoBehaviour
{
	[SerializeField] private int damageTaken;
	[SerializeField] private int damageDealt;
	[SerializeField] private int numOfKills;
	[SerializeField] private float timeAliveStart;
	[SerializeField] private float timeAliveEnd;

	public int DamageTaken
	{
		get { return damageTaken; }
	}

	public int DamageDealt
	{
		get { return damageDealt; }
	}

	public int NumOfKills
	{
		get { return numOfKills; }
	}

	public float TimeAliveStart
	{
		get { return timeAliveStart; }
	}

	public float? TimeAliveEnd
	{
		get
		{
			if (!walker || walker.IsDead) return null;
			return timeAliveEnd;
		}
	}

	public float TimeAliveTotal
	{
		get
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) return 0;
#endif
			if (!walker || walker.IsDead) return timeAliveEnd - timeAliveStart;
			return Time.time - TimeAliveStart;
		}
	}

	public RingWalker Walker
	{
		get { return walker ?? (walker = GetComponent<RingWalker>()); }
	}

	private RingWalker walker;

	private void OnEnable()
	{
		walker = GetComponent<RingWalker>();
		walker.EventKilled += WalkerOnEventKilled;
		walker.EventDamageDealt += WalkerOnEventDamageDealt;
		walker.EventDamageTaken += WalkerOnEventDamageTaken;
		walker.EventDeath += WalkerOnEventDeath;
	}

	private void OnDisable()
	{
		walker.EventKilled -= WalkerOnEventKilled;
		walker.EventDamageDealt -= WalkerOnEventDamageDealt;
		walker.EventDamageTaken -= WalkerOnEventDamageTaken;
		walker.EventDeath -= WalkerOnEventDeath;
	}

	private void Start()
	{
		timeAliveStart = 
		timeAliveEnd = Time.time;
	}

	private void WalkerOnEventDamageTaken(int damage, object source)
	{
		damageTaken += damage;
	}

	private void WalkerOnEventDamageDealt(int damage, object source)
	{
		damageDealt += damage;
	}

	private void WalkerOnEventKilled(int damage, object source)
	{
		numOfKills++;
	}

	private void WalkerOnEventDeath(int damage, object source)
	{
		timeAliveEnd = Time.time;
	}
}
