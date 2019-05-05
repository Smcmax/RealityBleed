public class UnitHealth : HealthInfo {

	public override int GetHealth() {
		return GetEntity().m_stats.GetStat(Stats.HP);
	}

	public override int GetMaxHealth() { 
		return GetEntity().m_stats.GetBaseStatWithGear(Stats.HP);
	}

	public override void SetHealth(int p_value) {
		int value = p_value;

		if(value > GetMaxHealth()) value = GetMaxHealth();
		else if(value < 0) value = 0;

		GetEntity().m_stats.AddModifier(Stats.HP, value - GetHealth(), 0);
	}

	private Entity GetEntity() {
		return (Entity) m_damageable;
	}
}
