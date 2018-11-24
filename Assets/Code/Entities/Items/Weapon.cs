using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : BaseItem {

	[Tooltip("Stats gained when this item is equipped")]
	public int[] m_statGainValues;

	// Main hands are split into two categories, two-handers and one-handers
	// Two-handers have 2 shots, a left-click and a right-click
	// One-handers on the other hand (haha get it?), only have a left-click while the off-hand has the right-click
	[Tooltip("The shot pattern fired when this weapon's left click is used, if it has one")]
	public ShotPattern m_leftClickPattern;

	[Tooltip("The shot pattern fired when this weapon's right click is used, if it has one")]
	public ShotPattern m_rightClickPattern;

	public override void Use(Entity p_entity, string[] p_args) { 
		if(p_args.Length == 0) return;

		bool useLeft = p_args[0].ToLower() == "true"; // left or right, case insensitive
		ShotPattern pattern = useLeft ? m_leftClickPattern : m_rightClickPattern;

		if(pattern) p_entity.m_shooter.Shoot(pattern);
	}
}
