using UnityEngine;

public static class Constants {

	public const float EFFECT_TICK_RATE = 0.5f;
	public const float CHARACTER_SPEED_UPDATE_RATE = 0.1f;

	// Colors, they all divide by 255 because unity only takes 0-1 values
	public static Color TRANSPARENT = new Color(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
	public static Color GREEN = new Color(0f / 255f, 255f / 255f, 0f / 255f);
	public static Color YELLOW = new Color(226f / 255f, 226f / 255f, 126f / 255f);
	public static Color RED = new Color(255f / 255f, 0f / 255f, 0f / 255f);
	public static Color MANA_BLUE = new Color(97f / 255f, 131f / 255f, 222f / 255f);
	public static Color PURPLE = new Color(173f / 255f, 85f / 255f, 255f / 255f);
	public static Color WHITE = new Color(255f / 255f, 255f / 255f, 255f / 255f);
	public static Color BLACK = new Color(0, 0, 0, 1);

}
