using UnityEngine;

public static class Constants {

	public const float EFFECT_TICK_RATE = 0.5f;
	public const float CHARACTER_SPEED_UPDATE_RATE = 0.1f;
	public const float FOG_DISCOVERY_UPDATE_RATE = 0.1f;
	public const float CORPSE_LIFETIME = 60f;
	public const int CORPSE_HEALTH_BASE = 5;
	public const float CORPSE_HEALTH_PERCENTAGE = 0.2f;
	public const float AUTO_LOOT_DELAY = 0.75f;
	public const float ABILITY_VIEW_REFRESH = 1f;
	public const float ABILITY_COOLDOWN_REFRESH = 0.1f;
	public const int MAX_PLAYER_COUNT = 4;
    public const float PATHFINDING_REFRESH_RATE = 0.1f;

	// Colors, they all divide by 255 because unity only takes 0-1 values
	public static Color TRANSPARENT = new Color(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
	public static Color GREEN = new Color(0f / 255f, 255f / 255f, 0f / 255f);
	public static Color YELLOW = new Color(226f / 255f, 226f / 255f, 126f / 255f);
	public static Color RED = new Color(255f / 255f, 0f / 255f, 0f / 255f);
	public static Color MANA_BLUE = new Color(97f / 255f, 131f / 255f, 222f / 255f);
	public static Color PURPLE = new Color(173f / 255f, 85f / 255f, 255f / 255f);
	public static Color WHITE = new Color(255f / 255f, 255f / 255f, 255f / 255f);
	public static Color BLACK = new Color(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);

	public static Color HP_COLOR = new Color(224f / 255f, 60f / 255f, 40f / 255f, 255f / 255f);
	public static Color MP_COLOR = new Color(91f / 255f, 168f / 255f, 255f / 255f, 255f / 255f);
	public static Color STR_COLOR = new Color(27f / 255f, 27f / 255f, 27f / 255f, 255f / 255f);
	public static Color DEX_COLOR = new Color(219f / 255f, 121f / 255f, 44f / 255f, 255f / 255f);
	public static Color INT_COLOR = new Color(222f / 255f, 190f / 255f, 87f / 255f, 255f / 255f);
	public static Color SPD_COLOR = new Color(88f / 255f, 219f / 255f, 152f / 255f, 255f / 255f);
	public static Color CON_COLOR = new Color(243f / 255f, 10f / 255f, 28f / 255f, 255f / 255f);
	public static Color DEF_COLOR = new Color(93f / 255f, 93f / 255f, 93f / 255f, 255f / 255f);
	public static Color WIS_COLOR = new Color(56f / 255f, 132f / 255f, 233f / 255f, 255f / 255f);

	public static Color AIR_COLOR = new Color(124f / 255f, 177f / 255f, 205f / 255f, 255f / 255f);
	public static Color EARTH_COLOR = new Color(225f / 255f, 169f / 255f, 95f / 255f, 255f / 255f);
	public static Color ELECTRIC_COLOR = new Color(255f / 255f, 254f / 255f, 0f / 255f, 255f / 255f);
	public static Color FIRE_COLOR = new Color(255f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
	public static Color HOLY_COLOR = new Color(244f / 255f, 220f / 255f, 98f / 255f, 255f / 255f);
	public static Color ICE_COLOR = new Color(185f / 255f, 232f / 255f, 234f / 255f, 255f / 255f);
	public static Color NORMAL_COLOR = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
	public static Color POISON_COLOR = new Color(91f / 255f, 47f / 255f, 82f / 255f, 255f / 255f);
	public static Color VOID_COLOR = new Color(11f / 255f, 11f / 255f, 55f / 255f, 255f / 255f);
	public static Color WATER_COLOR = new Color(64f / 255f, 164f / 255f, 223f / 255f, 255f / 255f);

	public static Color CORPSE_COLOR = new Color(50f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);
	public static Color DESTROYED_CORPSE_COLOR = new Color(150f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);
}

// Allows use of constant colors in the inspector from a dropdown
public enum ConstantColors { 
	TRANSPARENT, GREEN, YELLOW, RED, MANA_BLUE, PURPLE, WHITE, BLACK, 
	HP, MP, STR, DEX, INT, SPD, CON, DEF, WIS, 
	AIR, EARTH, ELECTRIC, FIRE, HOLY, ICE, NORMAL, POISON, VOID, WATER,
	CORPSE
}

// I hate this solution too, don't worry
public static class ConstantColorsExtension { 
	
	public static Color GetColor(this ConstantColors p_color) { 
		switch(p_color) {
			case ConstantColors.TRANSPARENT: return Constants.TRANSPARENT;
			case ConstantColors.GREEN: return Constants.GREEN;
			case ConstantColors.YELLOW: return Constants.YELLOW;
			case ConstantColors.RED: return Constants.RED;
			case ConstantColors.MANA_BLUE: return Constants.MANA_BLUE;
			case ConstantColors.PURPLE: return Constants.PURPLE;
			case ConstantColors.WHITE: return Constants.WHITE;
			case ConstantColors.BLACK: return Constants.BLACK;
			case ConstantColors.HP: return Constants.HP_COLOR;
			case ConstantColors.MP: return Constants.MP_COLOR;
			case ConstantColors.STR: return Constants.STR_COLOR;
			case ConstantColors.DEX: return Constants.DEX_COLOR;
			case ConstantColors.INT: return Constants.INT_COLOR;
			case ConstantColors.SPD: return Constants.SPD_COLOR;
			case ConstantColors.CON: return Constants.CON_COLOR;
			case ConstantColors.DEF: return Constants.DEF_COLOR;
			case ConstantColors.WIS: return Constants.WIS_COLOR;
			case ConstantColors.AIR: return Constants.AIR_COLOR;
			case ConstantColors.EARTH: return Constants.EARTH_COLOR;
			case ConstantColors.ELECTRIC: return Constants.ELECTRIC_COLOR;
			case ConstantColors.FIRE: return Constants.FIRE_COLOR;
			case ConstantColors.HOLY: return Constants.HOLY_COLOR;
			case ConstantColors.ICE: return Constants.ICE_COLOR;
			case ConstantColors.NORMAL: return Constants.NORMAL_COLOR;
			case ConstantColors.POISON: return Constants.POISON_COLOR;
			case ConstantColors.VOID: return Constants.VOID_COLOR;
			case ConstantColors.WATER: return Constants.WATER_COLOR;
			case ConstantColors.CORPSE: return Constants.CORPSE_COLOR;
			default: return Constants.TRANSPARENT;
		}
	}
}