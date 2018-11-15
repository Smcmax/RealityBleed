using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Shooter : MonoBehaviour {

	[Tooltip("The mana mode that should be used")]
	public bool m_useSharedMana;

	[Tooltip("Unit's mana")]
	[ConditionalField("m_useSharedMana", false)][Range(0, 1000)] public float m_mana;

	[Tooltip("Unit's mana")]
	[ConditionalField("m_useSharedMana", true)] public FloatVariable m_refMana;

	[Tooltip("Unit's max mana")]
	public FloatReference m_maxMana;

	[Tooltip("The minimum delay between pattern starts/loops")]
	[Range(0, 2)] public float m_patternCooldown;
	private float m_lastShot;

	[Tooltip("Event called when the entity shoots.")]
	public UnityEvent m_shotEvent;

	private Dictionary<ShotPattern, DataHolder> m_patterns;
	private Entity m_entity;

	public void Init(Entity p_entity) {
		m_entity = p_entity;
		m_patterns = new Dictionary<ShotPattern, DataHolder>();
	}

	private float GetMana() {
		return (m_refMana) ? m_refMana.Value : m_mana;
	}

	private void SetMana(float p_value) {
		if(m_refMana) m_refMana.Value = p_value;
		else m_mana = p_value;
	}

	public bool ConsumeMana(ShotPattern p_pattern) {
		if(GetMana() - p_pattern.m_manaPerStep < 0) return false;

		SetMana(GetMana() - p_pattern.m_manaPerStep);

		m_shotEvent.Invoke();

		return true;
	}

	public bool CanShoot(ShotPattern p_pattern) {
		object active = GetPatternInfo(p_pattern, "active");
		if(m_patterns.ContainsKey(p_pattern) && active != null && (bool) active) return false;
		if(m_patternCooldown == 0) return true;
		if(!CanLoop(p_pattern)) return false;

		return Time.time * 1000 >= m_lastShot + m_patternCooldown * 1000;
	}

	public bool CanLoop(ShotPattern p_pattern) {
		object lastLoopTimeObj = GetPatternInfo(p_pattern, "lastLoopTime");

		return lastLoopTimeObj == null ? true : Time.time * 1000 >= (float) lastLoopTimeObj + p_pattern.m_patternCooldown * 1000;
	}

	public void Shoot(ShotPattern p_pattern) {
		if(!CanShoot(p_pattern)) return;
		if(GetMana() - p_pattern.m_manaPerStep < 0) return;

		m_lastShot = Time.time * 1000;

		if(!m_patterns.ContainsKey(p_pattern)) m_patterns.Add(p_pattern, new DataHolder());

		p_pattern.Init(this);
		SetPatternInfo(p_pattern, "shotsFired", 0);
		SetPatternInfo(p_pattern, "loops", 0);
		SetPatternInfo(p_pattern, "active", true);
		StartCoroutine(PatternStep(p_pattern));
	}

	private IEnumerator PatternStep(ShotPattern p_pattern) {
		while((bool) GetPatternInfo(p_pattern, "active")) { 
			float delay = p_pattern.m_stepDelay;

			if(p_pattern.m_instant) delay = p_pattern.Instant(this);
			else delay = p_pattern.PreStep(this);

			if(delay == -1) delay = p_pattern.m_stepDelay;

			yield return new WaitForSeconds(delay);
		}
	}

	public object GetPatternInfo(ShotPattern p_pattern, string p_key) {
		DataHolder data = null;

		bool success = m_patterns.TryGetValue(p_pattern, out data);

		return success ? data.Get(p_key) : null;
	}

	public void SetPatternInfo(ShotPattern p_pattern, string p_key, object p_value) { 
		if(!m_patterns.ContainsKey(p_pattern)) m_patterns.Add(p_pattern, new DataHolder());

		m_patterns[p_pattern].Set(p_key, p_value);
	}

	public void StopShooting() {
		if(m_patterns.Count > 0)
			foreach(ShotPattern pattern in m_patterns.Keys)
				StopShooting(pattern);
	}

	public void StopShooting(ShotPattern p_pattern) {
		SetPatternInfo(p_pattern, "active", false);

		if(p_pattern.m_nextPatterns.Count > 0) StartCoroutine(Transition(p_pattern));
	}

	private IEnumerator Transition(ShotPattern p_pattern) { 
		yield return new WaitForSeconds(p_pattern.m_nextPatternSwitchDelay);

		p_pattern.Transition(this);
	}

	// TODO: add damage modifiers/stats to this
	public void Damage(Projectile p_projectile, Entity p_entity) {
		p_entity.Damage(p_projectile.m_damage, false, false);
	}
}
