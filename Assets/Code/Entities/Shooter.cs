using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class Shooter : MonoBehaviour {

	[Tooltip("The minimum delay between pattern starts/loops")]
	[Range(0, 2)] public float m_patternCooldown;
	private float m_lastShot;

	[Tooltip("Event called when the entity shoots.")]
	public UnityEvent m_shotEvent;

	[HideInInspector] public Entity m_entity;
	[HideInInspector] public List<ShotPattern> m_patterns;
    [HideInInspector] public Dictionary<string, float> m_patternLoopTimes;

	// only if it's an entity, shooter supports non-entities
	public void Init(Entity p_entity) {
		m_entity = p_entity;
        m_patternLoopTimes = new Dictionary<string, float>();
	}

	private float GetMana() {
		return m_entity.m_stats.GetStat(Stats.MP);
	}

	public bool ConsumeMana(ShotPattern p_pattern) {
		if(!m_entity) return true;
		if(p_pattern.m_manaPerStep > 0 && GetMana() - p_pattern.m_manaPerStep < 0) return false;

		m_entity.m_stats.AddModifier(Stats.MP, (int) -p_pattern.m_manaPerStep, 0);
		m_shotEvent.Invoke();

		return true;
	}

	public bool CanShoot(ShotPattern p_pattern) {
		if(Time.timeScale == 0f) return false;

		if(p_pattern.m_active) return false;
		if(m_patternCooldown == 0) return true;
		if(m_entity && GetMana() - p_pattern.m_manaPerStep < 0) return false;
		if(!CanLoop(p_pattern)) return false;
		if(p_pattern.m_bypassShooterCooldown) return true;

		return Time.time >= m_lastShot + m_patternCooldown;
	}

	public bool CanLoop(ShotPattern p_pattern) {
		Stats statApplied = (Stats) Enum.Parse(typeof(Stats), p_pattern.m_projectileInfo.m_statApplied);
		float patternCooldown = p_pattern.m_patternCooldown * statApplied.GetAlternateEffect(m_entity.m_stats.GetStat(statApplied));

		return !m_patternLoopTimes.ContainsKey(p_pattern.m_name) ? true : 
                Time.time >= m_patternLoopTimes[p_pattern.m_name] + patternCooldown;
	}

	public void Shoot(ShotPattern p_pattern) {
		if(!CanShoot(p_pattern)) return;

		m_lastShot = Time.time;

		m_patterns.Add(p_pattern);

		p_pattern.m_shotsFired = 0;
		p_pattern.m_loops = 0;
		p_pattern.Init(this);
		p_pattern.m_active = true;

		StartCoroutine(PatternStep(p_pattern));
	}

	private IEnumerator PatternStep(ShotPattern p_pattern) {
		while(p_pattern.m_active) { 
			float delay = p_pattern.m_stepDelay;

			if(p_pattern.m_instant) delay = p_pattern.Instant(this);
			else delay = p_pattern.PreStep(this);

			if(delay == -1) delay = p_pattern.m_stepDelay;

            m_patternLoopTimes[p_pattern.m_name] = p_pattern.m_lastLoopTime;

			yield return new WaitForSeconds(delay);
		}
	}

	public void StopShooting() {
		if(m_patterns.Count > 0)
			foreach(ShotPattern pattern in m_patterns)
				StopShooting(pattern);
	}

	public void StopShooting(ShotPattern p_pattern) {
		if(!m_patterns.Contains(p_pattern)) return;

		p_pattern.m_active = false;
		m_patterns.Remove(p_pattern);

        m_patternLoopTimes[p_pattern.m_name] = p_pattern.m_lastLoopTime;

        if(p_pattern.m_nextPatterns.Count > 0) StartCoroutine(Transition(p_pattern));
	}

	private IEnumerator Transition(ShotPattern p_pattern) { 
		yield return new WaitForSeconds(p_pattern.m_nextPatternSwitchDelay);

		p_pattern.Transition(this);
	}

	// this is the pure event, with no modifications applied prior
	public void Damage(Projectile p_projectile, IDamageable p_damageable) {
		int finalDamage = p_projectile.m_info.m_damage;

		finalDamage += m_entity.m_stats.GetStatEffect((Stats) Enum.Parse(typeof(Stats), p_projectile.m_info.m_statApplied));
		p_damageable.OnDamage(this, DamageType.Get(p_projectile.m_info.m_damageType), finalDamage, p_projectile.m_info.m_armorPiercing, false);
	}
}
