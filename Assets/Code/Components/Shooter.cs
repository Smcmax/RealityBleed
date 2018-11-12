using UnityEngine;
using UnityEngine.Events;
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

	private List<ShotPattern> m_patternsFiring;
	private Entity m_entity;

	public void Init(Entity p_entity) {
		m_entity = p_entity;
		m_patternsFiring = new List<ShotPattern>();
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
		if(m_patternsFiring.Contains(p_pattern)) return false;
		if(m_patternCooldown == 0) return true;
		if(!p_pattern.CanLoop()) return false;

		return Time.time * 1000 >= m_lastShot + m_patternCooldown * 1000;
	}

	public void Shoot(ShotPattern p_pattern) {
		if(!CanShoot(p_pattern)) return;
		if(GetMana() - p_pattern.m_manaPerStep < 0) return;

		m_lastShot = Time.time * 1000;

		m_patternsFiring.Add(p_pattern);
		p_pattern.StartPattern(this);
	}

	public void StopShooting() {
		if(m_patternsFiring.Count > 0)
			foreach(ShotPattern pattern in m_patternsFiring)
				StopShooting(pattern);
	}

	public void StopShooting(ShotPattern p_pattern) {
		m_patternsFiring.Remove(p_pattern);
		p_pattern.StopPattern();
	}

	public void Damage(Projectile p_projectile, Entity p_entity) {
		// TODO: apply stats to these
		if(m_entity.m_health) {
			float damage = p_projectile.m_damage;
			float defense = 0f;
			float finalDamage = damage - defense;

			if(finalDamage > 0) m_entity.m_health.Damage(finalDamage);
		}
	}
}
