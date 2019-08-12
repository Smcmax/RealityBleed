using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ModifierSkill : Skill {

	[Header("Specific Attributes")]
	[Tooltip("The modifier to change with this skill")]
	public string m_modifier;

	[Tooltip("Values to modify the modifier by per training level")]
	public List<TrainingLevelFloatWrapper> m_values;

	[Tooltip("The time after which the modifier will revert back to its normal state, 0 = permanent")]
	public List<TrainingLevelFloatWrapper> m_ttls;

	[TextArea]
	[Tooltip("Simple inspector comment zone, do not fill in json files")]
	public string DescriptionVariables = "Description's auto-filled variables: {newLine} {modifier} {value} {ttl}";

	public override string GetDescription(int p_trainingLevel, bool p_translate) {
		string description = m_descriptions.Find(d => d.TrainingLevel == p_trainingLevel).Description;

        if(p_translate) description = Game.m_languages.GetLine(description);

		return description.Replace("{modifier}", m_modifier)
						  .Replace("{value}", m_values.Find(v => v.TrainingLevel == p_trainingLevel).Value.ToString())
						  .Replace("{ttl}", m_ttls.Find(t => t.TrainingLevel == p_trainingLevel).Value.ToString())
						  .Replace("{newLine}", "\n");
	}

	public override void Use(Entity p_entity, int p_trainingLevel) {
		float value = m_values.Find(v => v.TrainingLevel == p_trainingLevel).Value;
		float ttl = m_ttls.Find(t => t.TrainingLevel == p_trainingLevel).Value;

		p_entity.m_modifiers.AddToModifier(m_modifier, value, ttl);
	}

	public override void Remove(Entity p_entity, int p_trainingLevel) {
		float value = m_values.Find(v => v.TrainingLevel == p_trainingLevel).Value;

		p_entity.m_modifiers.RemoveFromModifier(m_modifier, value);
	}
}