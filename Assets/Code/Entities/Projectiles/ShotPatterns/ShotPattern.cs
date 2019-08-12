using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class ShotPattern { // currently not shooting references

	[Tooltip("All included possible shot patterns in the game")]
	public static List<ShotPattern> m_patterns = new List<ShotPattern>();

	[Tooltip("All externally loaded shot patterns")]
	public static List<ShotPattern> m_externalPatterns = new List<ShotPattern>();

	[Tooltip("Internal and external patterns in a single list. Note that if an external and an internal pattern have the same name, the external will load over it")]
	public static List<ShotPattern> m_combinedPatterns = new List<ShotPattern>();

	[Tooltip("Are we using external shot patterns on top of the internal ones?")]
	public static bool m_useExternalPatterns = true; // TODO: change to reflect modded usage instead of a hard true

	[Tooltip("The name of this pattern, equivalent to the file name")]
	public string m_name;

	[Tooltip("This pattern's overall behaviour type (Circle, CircleSection, Shape, Wave)")]
	public string m_type;
	
	[Tooltip("Projectile to shoot in this pattern")]
	public string m_projectile;

	[Tooltip("The information to attribute to every projectile in the pattern")]
	public ProjectileInfo m_projectileInfo;

	[Tooltip("Behaviours to add to every shot projectile in the pattern")]
	public List<string> m_behaviours;

	[Tooltip("Amount of shots to include in this pattern")]
	public int m_shots;

	[Tooltip("Mana taken per pattern step, instant makes this a single step per loop")]
	public int m_manaPerStep;

	[Tooltip("If the shot happens instantly")]
	public bool m_instant;
	
	[Tooltip("If the pattern continues after the first loop")]
	public bool m_loop;

	[Tooltip("How many loops of the pattern will be shot before the pattern stops/transitions, 0 = infinite")]
	public int m_loopsBeforeSwitch;

	[Tooltip("Time in seconds to wait between pattern loops/restarts")]
	public float m_patternCooldown;

	[Tooltip("Should this shot pattern be allowed to shoot regardless of the shooter's shot cooldown?")]
	public bool m_bypassShooterCooldown;

	[Tooltip("Time in seconds before the next pattern update (next shot in the pattern)")]
	public float m_stepDelay;

	[Tooltip("Patterns to switch to after this one ends (they overlap), can be null")]
	public List<string> m_nextPatterns;

	[Tooltip("The delay in seconds before this pattern switches to the next one")]
	public float m_nextPatternSwitchDelay;

	[Tooltip("Add in special effects to display in tooltip like 'Fires multiple projectiles'")]
	public string m_extraTooltipInfo;

	[HideInInspector] public bool m_active;
	[HideInInspector] public int m_shotsFired;
	[HideInInspector] public int m_loops;
	[HideInInspector] public float m_lastLoopTime;
	[HideInInspector] public Vector2 m_forcedTarget = Vector2.zero;
	[HideInInspector] public Vector3 m_spawnLocation = Vector3.zero;

	public static void LoadAll() {
		m_patterns.Clear();
		m_externalPatterns.Clear();
		m_combinedPatterns.Clear();

		TextAsset[] patterns = Resources.LoadAll<TextAsset>("ShotPatterns");

		foreach(TextAsset loadedPattern in patterns) {
			ShotPattern pattern = Load(loadedPattern.text);

			if(pattern != null) m_patterns.Add(pattern);
		}

		string[] files = Directory.GetFiles(Application.dataPath + "/Data/ShotPatterns/");

		if(files.Length > 0)
			foreach(string file in files) {
				if(file.ToLower().EndsWith(".json")) {
					StreamReader reader = new StreamReader(file);
					ShotPattern pattern = Load(reader.ReadToEnd());

					if(pattern != null) m_externalPatterns.Add(pattern);
					reader.Close();
				}
			}

		foreach(ShotPattern pattern in m_patterns) { 
			ShotPattern external = m_externalPatterns.Find(sp => sp.m_type == pattern.m_type);

			if(external != null) m_combinedPatterns.Add(external);
			else m_combinedPatterns.Add(pattern);
		}

		if(m_externalPatterns.Count > 0)
			foreach(ShotPattern external in m_externalPatterns)
				if(!m_patterns.Exists(sp => sp.m_type == external.m_type))
					m_combinedPatterns.Add(external);

		// foreach(ShotPattern pattern in m_combinedPatterns)
		// then generate the mana cost for each of them
	}

	private static ShotPattern Load(string p_json) { 
		ShotPattern pattern = JsonUtility.FromJson<ShotPattern>(p_json);
		Type type = null;

		switch(pattern.m_type.ToLower()) {
			case "circle": type = typeof(CirclePattern); break;
			case "circlesection": type = typeof(CircleSectionPattern); break;
			case "shape": type = typeof(ShapePattern); break;
			case "wave": type = typeof(WavePattern); break;
		}

		if(type == null) return null;

		return (ShotPattern) JsonUtility.FromJson(p_json, type);
	}

	public static ShotPattern Get(string p_name, bool p_reference) { 
		List<ShotPattern> availablePatterns = m_useExternalPatterns ? m_combinedPatterns : m_patterns;
		ShotPattern found = availablePatterns.Find(sp => sp.m_name == p_name);

		if(found != null) return p_reference ? found : found.Clone();
		
		return null;
	}

	protected Vector2 FetchTarget(Shooter p_shooter, Projectile p_projectile) {
		return m_forcedTarget == Vector2.zero ? (Vector2) p_projectile.transform.up : m_forcedTarget;
	}

	protected Projectile SpawnProjectile(Shooter p_shooter) {
		GameObject proj = Game.m_projPool.Get();
		Projectile projectile = proj.GetComponent<Projectile>();
		Projectile referenceProj = Projectile.Get(m_projectile);

		proj.transform.position = m_spawnLocation == Vector3.zero ? p_shooter.transform.position : (Vector3)m_spawnLocation;
		proj.transform.rotation = referenceProj.transform.rotation;

		projectile.Clone(referenceProj, m_projectileInfo, m_behaviours);
		projectile.GetComponent<PolygonColliderExtruder>().Extrude();

		return projectile;
	}

	public void Transition(Shooter p_shooter) {
		for(int i = 0; i < m_nextPatterns.Count; ++i)
			p_shooter.Shoot(Get(m_nextPatterns[i], false));
	}

	public float Instant(Shooter p_shooter) {
		if(!p_shooter.ConsumeMana(this)){
			p_shooter.StopShooting(this);
			return -1;
		}

		for(int i = 0; i < m_shots; ++i) PreStep(p_shooter);

		if(m_shotsFired == m_shots) AddLoop(p_shooter);
		if(!p_shooter.CanLoop(this)) return Time.time - (m_lastLoopTime + m_patternCooldown);

		return m_patternCooldown;
	}

	public float PreStep(Shooter p_shooter) {
		if(!m_active) return -1;

		if(m_shotsFired == m_shots)
			if(AddLoop(p_shooter)) return -1;

		if(!p_shooter.CanLoop(this)) return Time.time - (m_lastLoopTime + m_patternCooldown);

		if(!m_instant && !p_shooter.ConsumeMana(this)) {
			p_shooter.StopShooting(this);
			return -1;
		}

		Step(p_shooter);

		m_shotsFired++;

		return m_stepDelay;
	}

	private bool AddLoop(Shooter p_shooter) {
		m_shotsFired = 0;
		m_loops++;
		m_lastLoopTime = Time.time;

		if(IsDoneLooping(p_shooter)) {
			p_shooter.StopShooting(this);
			return true;
		}

		Init(p_shooter);

		return false;
	}

	private bool IsDoneLooping(Shooter p_shooter) {
		return (m_loop && m_loops >= m_loopsBeforeSwitch && m_loopsBeforeSwitch != 0) || (!m_loop && m_loops >= 1);
	}

	public virtual void Init(Shooter p_shooter) { }

	public virtual void Step(Shooter p_shooter) { }

	public virtual ShotPattern Clone() { 
		ShotPattern pattern = (ShotPattern) Activator.CreateInstance(GetType());

		pattern.m_name = m_name;
		pattern.m_projectile = m_projectile;
		pattern.m_projectileInfo = m_projectileInfo;
		pattern.m_behaviours = m_behaviours;
		pattern.m_shots = m_shots;
		pattern.m_manaPerStep = m_manaPerStep;
		pattern.m_instant = m_instant;
		pattern.m_loop = m_loop;
		pattern.m_loopsBeforeSwitch = m_loopsBeforeSwitch;
		pattern.m_patternCooldown = m_patternCooldown;
		pattern.m_bypassShooterCooldown = m_bypassShooterCooldown;
		pattern.m_stepDelay = m_stepDelay;
		pattern.m_nextPatterns = m_nextPatterns;
		pattern.m_nextPatternSwitchDelay = m_nextPatternSwitchDelay;
		pattern.m_extraTooltipInfo = m_extraTooltipInfo;

		m_active = false;
		m_shotsFired = 0;
		m_loops = 0;
		m_lastLoopTime = 0;
		m_forcedTarget = Vector2.zero;
		m_spawnLocation = Vector3.zero;

		return pattern;
	}
}
