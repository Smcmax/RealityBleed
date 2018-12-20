using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class UnitStats : MonoBehaviour {

	public const int STAT_AMOUNT = 9;

	// base stats, no modifiers applied
	public IntReference m_maxHP;
	public IntReference m_maxMP;

	// all three of these affect their respective weapon's firing speed
	public IntReference m_strength;      // STR - Damage with melee-type items/weapons
	public IntReference m_dexterity;     // DEX - Damage with agility-type items/weapons
	public IntReference m_intellect;      // INT - Damage with caster-type items/weapons

	public IntReference m_speed;         // SPD - Character speed
	public IntReference m_constitution;  // CON - Affects health regen and HP gained/level
	public IntReference m_defense;      // DEF - Reduces incoming damage
	public IntReference m_wisdom;        // WIS - Affects mana regen and MP gained/level

	[Tooltip("Whether or not the entity is allowed to regen health")]
	public bool m_healthRegen;

	[Tooltip("Whether or not the entity is allowed to regen mana")]
	public bool m_manaRegen;

	[Tooltip("Stat update events called whenever a stat changes. Same order as stats, null = no call")]
	public GameEvent[] m_statEvents;

	private int[] m_gearModifiers;
	private int[] m_modifiers;

	void Awake() { 
		m_modifiers = new int[STAT_AMOUNT];
		m_gearModifiers = new int[STAT_AMOUNT];

		// constant HP/MP regen
		StartCoroutine(RegenHealth());
		StartCoroutine(RegenMana());
	}

	public int GetBaseStat(Stats p_stat) { 
		switch(p_stat) {
			case Stats.HP: return m_maxHP;
			case Stats.MP: return m_maxMP;
			case Stats.STR: return m_strength;
			case Stats.DEX: return m_dexterity;
			case Stats.INT: return m_intellect;
			case Stats.SPD: return m_speed;
			case Stats.CON: return m_constitution;
			case Stats.DEF: return m_defense;
			case Stats.WIS: return m_wisdom;
			default: return 0;
		}
	}

	public int GetBaseStatWithGear(Stats p_stat) { 
		return GetBaseStat(p_stat) + m_gearModifiers[(int) p_stat];
	}

	public int GetStat(Stats p_stat) { 
		return GetBaseStat(p_stat) + m_modifiers[(int) p_stat] + m_gearModifiers[(int) p_stat];
	}

	public int GetStatEffect(Stats p_stat) { 
		return (int) GetStatEffectFloat(p_stat);
	}

	public float GetStatEffectFloat(Stats p_stat) { 
		return p_stat.GetEffect(GetStat(p_stat));
	}

	public int GetModifier(Stats p_stat) {
		return m_modifiers[(int) p_stat];
	}

	public int GetGearModifier(Stats p_stat) {
		return m_gearModifiers[(int) p_stat];
	}

	public void SetModifier(Stats p_stat, int p_value) { 
		m_modifiers[(int) p_stat] = p_value;
	}

	public void SetModifiers(int[] p_modifiers) { 
		m_modifiers = p_modifiers;
	}

	public void UpdateGearModifiers(int[] p_newModifiers) {
		int[] oldModifiers = m_gearModifiers;
		m_gearModifiers = p_newModifiers;

		CallStatUpdateEvents(oldModifiers, m_gearModifiers);
	}

	private void CallStatUpdateEvents(int[] oldModifiers, int[] newModifiers) {
		for(int i = 0; i < STAT_AMOUNT; i++)
			if(oldModifiers[i] != newModifiers[i])
				CallStatUpdateEvent((Stats) i);
	}

	private void CallStatUpdateEvent(Stats p_stat) {
		if(m_statEvents.Length > (int) p_stat && m_statEvents[(int) p_stat])
			m_statEvents[(int) p_stat].Raise();
	}

	// a ttl of 0 = permanent (can still be manually removed)
	// NOTE: HP and MP work in reduction of the stat!!!
	public void AddModifier(Stats p_stat, int p_value, float p_ttl) { 
		m_modifiers[(int) p_stat] += p_value;
		CallStatUpdateEvent(p_stat);

		if(p_ttl > 0) StartCoroutine(RemoveModifierCoroutine(p_stat, p_value, p_ttl));
	}

	public void RemoveModifier(Stats p_stat, int p_value) {
		m_modifiers[(int) p_stat] -= p_value;
		CallStatUpdateEvent(p_stat);
	}

	// for use with the ttl-enabled AddModifier only
	private IEnumerator RemoveModifierCoroutine(Stats p_stat, int p_value, float p_ttl) {
		yield return new WaitForSeconds(p_ttl);

		RemoveModifier(p_stat, p_value);
	}

	public IEnumerator RegenHealth() {
		while(m_healthRegen) {
			if (GetBaseStatWithGear(Stats.HP) - (GetStat(Stats.HP) + 1) >= 0) {
				AddModifier(Stats.HP, 1, 0);
				CallStatUpdateEvent(Stats.HP);
			}

			yield return new WaitForSeconds(1f / GetStatEffectFloat(Stats.CON));
		}
	}

	public IEnumerator RegenMana() {
		while(m_manaRegen) {
			if(GetBaseStatWithGear(Stats.MP) - (GetStat(Stats.MP) + 1) >= 0) {
				AddModifier(Stats.MP, 1, 0);
				CallStatUpdateEvent(Stats.MP);
			}

			yield return new WaitForSeconds(1f / GetStatEffectFloat(Stats.WIS));
		}
	}
}

public enum Stats { 
	HP, MP, STR, DEX, INT, SPD, CON, DEF, WIS
}
