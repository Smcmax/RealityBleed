using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShotPatternAbility : Ability {

	[Header("Specific Attributes")]
	[Tooltip("The list containing the shot pattern to be fired at each training level")]
	public List<ShotPatternLevelWrapper> m_shotPatterns;

	[Space]
	[Tooltip("Whether or not the shot pattern should be aimed towards the cursor")]
	public bool m_aimAtCursor;

	[TextArea]
	[Tooltip("Simple inspector comment zone, do not fill in json files")]
	public string DescriptionVariables = "Description's auto-filled variables: {newLine} {damage} {manaCost} {manaPerStep} {cooldown} {shots} {statApplied} {range} {speed}";

	public override string GetDescription(int p_trainingLevel, bool p_translate) {
		ShotPattern pattern = ShotPattern.Get(m_shotPatterns.Find(s => s.TrainingLevel == p_trainingLevel).Pattern, true);
		string description = m_descriptions.Find(d => d.TrainingLevel == p_trainingLevel).Description;

		if(p_translate) description = Game.m_languages.GetLine(description);

		return description.Replace("{damage}", pattern.m_projectileInfo.m_damage.ToString())
						  .Replace("{manaPerStep}", pattern.m_manaPerStep.ToString())
						  .Replace("{manaCost}", m_manaCosts.Find(m => m.TrainingLevel == p_trainingLevel).Value.ToString())
						  .Replace("{cooldown}", m_cooldowns.Find(m => m.TrainingLevel == p_trainingLevel).Value.ToString())
						  .Replace("{shots}", pattern.m_shots.ToString())
						  .Replace("{statApplied}", pattern.m_projectileInfo.m_statApplied.ToString())
						  .Replace("{range}", pattern.m_projectileInfo.m_range.ToString())
						  .Replace("{speed}", pattern.m_projectileInfo.m_speed.ToString())
						  .Replace("{newLine}", "\n");
	}

	public override void Use(Entity p_entity, int p_trainingLevel) {
		ShotPattern pattern = ShotPattern.Get(m_shotPatterns.Find(s => s.TrainingLevel == p_trainingLevel).Pattern, false);

		if(m_aimAtCursor)
			pattern.m_forcedTarget = Camera.main.ScreenToWorldPoint((p_entity as Player).m_mouse.GetPosition());

		pattern.m_manaPerStep = 0;

		p_entity.m_shooter.Shoot(pattern);
	}
}

[System.Serializable]
public struct ShotPatternLevelWrapper {
	public int TrainingLevel;
	public string Pattern;
}