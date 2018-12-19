using UnityEngine;

public static class StatsExtension {

	// this is useful to apply a formula to a specific stat
	// for example, if you wanted STR to deal double damage, you could apply it here
	public static float GetEffect(this Stats p_stat, int p_value) {
		switch(p_stat) {
			case Stats.HP: return p_value;
			case Stats.MP: return p_value;
			case Stats.STR: return p_value;
			case Stats.DEX: return p_value;
			case Stats.INT: return p_value;
			case Stats.SPD: return p_value;
			case Stats.CON: return (float) p_value / 10f; // 1 CON = 0.1 HP/s
			case Stats.DEF: return p_value;
			case Stats.WIS: return (float) p_value / 10f; // 1 WIS = 0.1 MP/s
			default: return p_value;
		}
	}
}