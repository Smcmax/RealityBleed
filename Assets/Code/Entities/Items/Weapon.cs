using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon : BaseItem {

	[Tooltip("The type of weapon this item is")]
	public string m_weaponType;

	[Tooltip("Stats gained when this item is equipped")]
	public int[] m_statGainValues;

	// Main hands are split into two categories, two-handers and one-handers
	// Two-handers have 2 shots, a left-click and a right-click
	// One-handers, on the other hand (haha get it?), only have a left-click while the off-hand has the right-click
	[Tooltip("The shot pattern fired when this weapon's left click is used, if it has one. A weapon's FIRST shot is always left-click")]
	public string m_leftClickPattern;

	[Tooltip("The shot pattern fired when this weapon's right click is used, if it has one")]
	public string m_rightClickPattern;

	public override void Use(Entity p_entity, string[] p_args) { 
		if(p_args.Length == 0) return;

		bool useLeft = p_args[0].ToLower() == "true"; // left or right, case insensitive
		ShotPattern pattern = ShotPattern.Get(useLeft ? m_leftClickPattern : m_rightClickPattern, false);

		if(pattern != null) {
			if(p_args.Length >= 2)
				if(p_args[1].ToLower() == "true" && p_entity is Player)
					pattern.m_forcedTarget = Camera.main.ScreenToWorldPoint(((Player)p_entity).m_mouse.GetPosition());

			p_entity.m_shooter.Shoot(pattern);
		}
	}

	public bool IsTwoHanded() { return !string.IsNullOrEmpty(m_leftClickPattern) && !string.IsNullOrEmpty(m_rightClickPattern); }

	public WeaponType GetWeaponType() { 
		WeaponType type;

		if(Enum.TryParse(m_weaponType, true, out type))
			return type;

		return WeaponType.Sword;
	}
}

public enum WeaponType { 
	Sword, Dagger, Axe, Polearm, Mace, Staff, Wand, Crossbow, Bow
}