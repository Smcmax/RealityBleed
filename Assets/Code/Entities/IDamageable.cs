public interface IDamageable {
	void OnDamage(Shooter p_damager, DamageType p_type, int p_damage, bool p_bypassDefense, bool p_bypassImmunityWindow);
	void OnDeath();
}
