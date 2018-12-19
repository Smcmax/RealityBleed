using UnityEngine;

[CreateAssetMenu(menuName = "Stat Gains")]
public class StatGains : ScriptableObject {

	public int[] m_minimumStatGainsPerLevel;
	public int[] m_maximumStatGainsPerLevel;

	public int Random(Stats p_stat) {
		int min = m_minimumStatGainsPerLevel[(int) p_stat];
		int max = m_maximumStatGainsPerLevel[(int) p_stat];

		return UnityEngine.Random.Range(min, max);
	}

}
