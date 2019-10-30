using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class NPC : Interactable {

	private static int m_npcIds = 0;
	private static List<NPC> m_npcs = new List<NPC>();

    [Tooltip("Whether or not this npc can sell things using its inventory")]
    public bool m_hasShop = true;

	[HideInInspector] public string m_npcId;
	[HideInInspector] public Entity m_entity;
	[HideInInspector] public DialogController m_dialog;
    [HideInInspector] public List<NPCType> m_types;
	[HideInInspector] public List<string> m_questsAvailable;

    void OnEnable() {
		if(!(this is Enemy)) m_npcs.Add(this);
    }

	void OnDisable() {
        if(!(this is Enemy)) m_npcs.Remove(this);
	}

	public void Init(List<NPCType> p_types, string p_set) {
        m_npcId = m_npcIds.ToString();
        m_npcIds++;

		m_types = p_types;

		m_dialog = GetComponent<DialogController>();
		m_entity = GetComponent<Entity>();
        m_dialog.m_npc = this;
		m_entity.m_npc = this;

        m_entity.m_sets.Add(p_set);
        m_entity.HandleSets(true);
	}

    public override void Interact(Entity p_entity) {
		if(m_dialog && p_entity is Player) m_dialog.Interact((Player) p_entity);
	}

    public override void OutOfRange(Entity p_entity) {
		if(m_dialog && p_entity is Player) m_dialog.ChangeToStartingDialog(true);
	}

	public void Die() {
		if(m_dialog) {
			m_dialog.ChangeToStartingDialog(true);
			Destroy(m_dialog);
		}
		
		if(m_tooltipRenderer) Destroy(m_tooltipRenderer);
		if(m_interactBounds) Destroy(m_interactBounds);

		Destroy(this);
	}

	public static NPC FindRandom() { 
		return m_npcs[Random.Range(0, m_npcs.Count)];
	}

	public static NPC FindRandomFromType(string p_type) { 
		List<NPC> eligible = m_npcs.FindAll(n => n.m_types.Find(t => t.m_type == p_type) != null);

		if(eligible.Count == 0) return null;
		else return eligible[Random.Range(0, eligible.Count)];
	}
}
