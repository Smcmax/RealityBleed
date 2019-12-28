using UnityEngine;

[System.Serializable]
public class TargetInRangeCondition : Condition {

    [Tooltip("Range within which this condition becomes true")]
    public RangedFloat m_range;

    public override bool Test(StateController p_controller) {
        float dist = Vector2.Distance(p_controller.m_entity.transform.position,
                                      p_controller.m_target.transform.position);

        return dist <= m_range.Max && dist >= m_range.Min;
    }
}