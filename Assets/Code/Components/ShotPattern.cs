using UnityEngine;
using System.Collections.Generic;

public abstract class ShotPattern : MonoBehaviour {

	[Tooltip("Projectile to shoot in this pattern")]
	public Projectile m_projectile;

	[Tooltip("Amount of shots to include in this pattern")]
	[Range(1, 100)] public int m_shots;

	[Tooltip("Mana taken per pattern step, instant makes this a single step per loop")]
	[Range(0, 100)] public float m_manaPerStep;

	[Tooltip("If the shot happens instantly")]
	public bool m_instant;
	
	[Tooltip("If the pattern continues after the first loop")]
	public bool m_loop;

	[Tooltip("How many loops of the pattern will be shot before the pattern stops/transitions, 0 = infinite")]
	[ConditionalField("m_loop")] public int m_loopsBeforeSwitch;

	[Tooltip("Time in seconds to wait between pattern loops/restarts")]
	[Range(0, 10)] public float m_patternCooldown;

	[Tooltip("Time in seconds before the next pattern update (next shot in the pattern)")]
	[Range(0, 10)] public float m_stepDelay;

	[Tooltip("Patterns to switch to after this one ends (they overlap), can be null")]
	public List<ShotPattern> m_nextPatterns;

	[Tooltip("The delay in seconds before this pattern switches to the next one")]
	[Range(0, 10)] public float m_nextPatternSwitchDelay;

	[HideInInspector] public Vector2 m_forcedTarget;
	private bool m_active;
	private int m_shotsFired;
	private int m_loops;
	private float m_lastLoopTime;
	protected Shooter m_shooter;
	protected Vector3 m_spawnLocation;

	public void StartPattern(Shooter p_shooter) {
		StartPattern(p_shooter, transform.position);
	}

	public void StartPattern(Shooter p_shooter, Vector3 p_spawnLocation) {
		m_spawnLocation = p_spawnLocation;
		m_shooter = p_shooter;
		m_active = true;
		m_shotsFired = 0;
		m_loops = 0;

		Init();

		if(m_instant) {
			if (m_patternCooldown == 0) InvokeRepeating("Instant", 0f, m_stepDelay);
			else Invoke("Instant", 0f);
		} else InvokeRepeating("PreStep", 0f, m_stepDelay);
	}

	public void StopPattern() {
		m_active = false;

		CancelInvoke();

		if(m_nextPatterns.Count > 0) TransitionToPattern();
	}

	public bool CanLoop() {
		return Time.time * 1000 >= m_lastLoopTime + m_patternCooldown * 1000;
	}

	protected Vector2 FetchTarget(Projectile p_projectile) {
		return m_forcedTarget == Vector2.zero ? (Vector2) transform.up : m_forcedTarget;
	}

	protected Projectile SpawnProjectile() {
		GameObject proj = m_projectile.m_projectilePooler.Get();
		Projectile projectile = proj.GetComponent<Projectile>();

		proj.transform.position = m_spawnLocation;
		proj.transform.rotation = m_projectile.transform.rotation;

		projectile.Clone(m_projectile);

		return projectile;
	}

	private void TransitionToPattern() {
		Invoke("Transition", m_nextPatternSwitchDelay);
	}

	private void Transition() {
		m_shooter.StopShooting(this);

		for(int i = 0; i < m_nextPatterns.Count; ++i)
			m_shooter.Shoot(m_nextPatterns[i]);
	}

	private void Instant() {
		if (!m_shooter.ConsumeMana(this)){
			m_shooter.StopShooting(this);
			return;
		}

		for(int i = 0; i < m_shots; ++i) PreStep();

		if(m_shotsFired == m_shots) AddLoop();

		if(!CanLoop()){
			Invoke("Instant", m_lastLoopTime + m_patternCooldown * 1000);
			return;
		}
	}

	public void PreStep() {
		if(!m_active) return;

		if(m_shotsFired == m_shots)
			if(AddLoop()) return;

		if(!CanLoop()) {
			CancelInvoke();
			InvokeRepeating("PreStep", (Time.time * 1000 - (m_lastLoopTime + m_patternCooldown * 1000)) / 1000, m_stepDelay);
			return;
		}

		if(!m_instant && !m_shooter.ConsumeMana(this)) {
			m_shooter.StopShooting(this);
			return;
		}

		Step();

		m_shotsFired++;
	}

	private bool AddLoop() {
		m_shotsFired = 0;
		m_loops++;
		m_lastLoopTime = Time.time * 1000;

		if(IsDoneLooping()) {
			m_shooter.StopShooting(this);
			return true;
		}

		return false;
	}

	private bool IsDoneLooping() {
		return (m_loop && m_loops >= m_loopsBeforeSwitch && m_loopsBeforeSwitch != 0) || (!m_loop && m_loops >= 1);
	}

	public abstract void Init();

	public abstract void Step();
}
