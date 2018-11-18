using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : BaseItem {

	// Main hands are split into two categories, two-handers and one-handers
	// Two-handers have 2 shots, a left-click and a right-click
	// One-handers on the other hand (haha get it?), only have a left-click while the off-hand has the right-click
	[Tooltip("The shot pattern fired when this weapon's left click is used, if it has one")]
	public ShotPattern m_leftClickPattern;

	[Tooltip("The shot pattern fired when this weapon's right click is used, if it has one")]
	public ShotPattern m_rightClickPattern;

	[Tooltip("If the weapon is a two-handed weapon (using left AND right click")]
	public bool m_isTwoHanded; // temp, will probably be switched eventually

	public override void Use(string[] p_args) { 
		if(p_args.Length == 0) return;

		bool useLeft = p_args[0].ToLower() == "true"; // left or right, case insensitive

		m_holder.m_shooter.Shoot(useLeft ? m_leftClickPattern : m_rightClickPattern);
	}
}
