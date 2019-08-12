using UnityEngine;

public enum Stats {
	HP, MP, STR, DEX, INT, SPD, CON, DEF, WIS
}

public static class StatsExtension {

	// this is useful to apply a formula to a specific stat
	// for example, if you wanted STR to deal double damage globally, you could apply it here
	public static float GetEffect(this Stats p_stat, int p_value) {
		switch(p_stat) {
			case Stats.STR: return p_value;                   // 1 STR = +1 damage per hit
			case Stats.DEX: return p_value;                   // 1 DEX = +1 damage per hit
			case Stats.INT: return p_value;                   // 1 INT = +1 damage per hit
			case Stats.SPD: return 2 + (float) p_value / 50f; // 0 SPD = 2 speed, 50 SPD = 3 speed, 100 SPD = 4 speed
			case Stats.CON: return (float) p_value / 10f;     // 1 CON = 0.1 HP/s
			case Stats.DEF: return p_value;                   // 1 DEF = -1 damage taken per hit
			case Stats.WIS: return (float) p_value / 10f;     // 1 WIS = 0.1 MP/s
			default: return p_value;
		}
	}

	public static float GetAlternateEffect(this Stats p_stat, int p_value) {
		switch(p_stat) {
			case Stats.STR:                                    // 0 STR = base firing speed, 50 STR = 25% faster, 100 STR = 50% faster
			case Stats.DEX:                                    // 0 DEX = base firing speed, 50 DEX = 25% faster, 100 DEX = 50% faster
			case Stats.INT: return 1 - (float) p_value / 200f; // 0 INT = base firing speed, 50 INT = 25% faster, 100 INT = 50% faster
			default: return p_value;
		}
	}

	public static Color GetColor(this Stats p_stat) {
		switch(p_stat) {
			case Stats.HP: return Constants.HP_COLOR;
			case Stats.MP: return Constants.MP_COLOR;
			case Stats.STR: return Constants.STR_COLOR;
			case Stats.DEX: return Constants.DEX_COLOR;
			case Stats.INT: return Constants.INT_COLOR;
			case Stats.SPD: return Constants.SPD_COLOR;
			case Stats.CON: return Constants.CON_COLOR;
			case Stats.DEF: return Constants.DEF_COLOR;
			case Stats.WIS: return Constants.WIS_COLOR;
			default: return Constants.YELLOW;
		}
	}
}