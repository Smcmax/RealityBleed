using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyType {

    [Tooltip("The name of this type")]
    public string m_type;

    [Tooltip("The lower starting stat bounds for this enemy type, leave blank if using default stats")]
    public List<int> m_minimumStats;

    [Tooltip("The upper starting stat bounds for this enemy type, leave blank if using default stats")]
    public List<int> m_maximumStats;

    [Tooltip("The drop table used to equip this enemy")]
    public DropTable m_equipmentTable;

    [Tooltip("The drop table dropped on death, if not set, will drop the enemy's inventory")]
    public DropTable m_dropTable;

    [Tooltip("All available male sprites for enemies")]
    public List<SerializableSprite> m_maleSprites;

    [Tooltip("All available female sprites for enemies")]
    public List<SerializableSprite> m_femaleSprites;

    [Tooltip("All available default states from which one will be selected at random")]
    public List<string> m_defaultStates; // TODO: add % chance to have?

    [Tooltip("All available default look variables from which one will be selected at random")]
    public List<Look> m_looks;

    [Tooltip("All dialogues associated to this enemy type, determines which dialogues are allowed to be used for this type")]
    public List<string> m_greetings;

    public EnemyType Clone() {
        EnemyType newType = new EnemyType();

        newType.m_type = m_type;
        newType.m_minimumStats = new List<int>(m_minimumStats);
        newType.m_maximumStats = new List<int>(m_maximumStats);
        newType.m_equipmentTable = m_equipmentTable;
        newType.m_dropTable = m_dropTable;
        newType.m_maleSprites = new List<SerializableSprite>(m_maleSprites);
        newType.m_femaleSprites = new List<SerializableSprite>(m_femaleSprites);
        newType.m_defaultStates = new List<string>(m_defaultStates);
        newType.m_looks = new List<Look>(m_looks);
        newType.m_greetings = new List<string>(m_greetings);

        return newType;
    }

    public void Combine(EnemyType type, bool p_appendDataMarker) {
        if(type.m_minimumStats.Count > 0) {
            m_minimumStats.Clear();
            m_minimumStats.AddRange(type.m_minimumStats);
        }

        if(type.m_maximumStats.Count > 0) {
            m_maximumStats.Clear();
            m_maximumStats.AddRange(type.m_maximumStats);
        }

        m_equipmentTable = type.m_equipmentTable;
        m_dropTable = type.m_dropTable;

        if(type.m_maleSprites.Count > 0)
            foreach(SerializableSprite sprite in type.m_maleSprites) {
                if(p_appendDataMarker) sprite.m_internal = false;

                m_maleSprites.Add(sprite);
            }

        if(type.m_femaleSprites.Count > 0)
            foreach(SerializableSprite sprite in type.m_femaleSprites) {
                if(p_appendDataMarker) sprite.m_internal = false;

                m_maleSprites.Add(sprite);
            }

        if(type.m_defaultStates.Count > 0) {
            m_defaultStates.Clear();
            m_defaultStates.AddRange(type.m_defaultStates);
        }

        if(type.m_looks.Count > 0) {
            m_looks.Clear();
            m_looks.AddRange(type.m_looks);
        }

        if(type.m_greetings.Count > 0)
            foreach(string greeting in type.m_greetings)
                m_greetings.Add(greeting + (p_appendDataMarker ? "-External" : ""));
    }

    public void AppendDataMarker() {
        if(m_maleSprites.Count > 0)
            foreach(SerializableSprite sprite in m_maleSprites)
                sprite.m_internal = false;

        if(m_femaleSprites.Count > 0)
            foreach(SerializableSprite sprite in m_femaleSprites)
                sprite.m_internal = false;

        if(m_greetings.Count > 0) {
            List<string> clonedGreetings = new List<string>(m_greetings);
            m_greetings.Clear();

            foreach(string greeting in clonedGreetings)
                m_greetings.Add(greeting + "-External");
        }
    }
}
