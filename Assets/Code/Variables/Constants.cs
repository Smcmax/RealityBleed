using UnityEngine;

public static class Constants {

	public const float EFFECT_TICK_RATE = 0.5f;
	public const float CHARACTER_SPEED_UPDATE_RATE = 0.1f;
	public const float FOG_DISCOVERY_UPDATE_RATE = 0.1f;

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
}
