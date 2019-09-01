using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class ProjectileBehaviour {

	public static List<ProjectileBehaviour> m_loadedBehaviours = new List<ProjectileBehaviour>();

	[Tooltip("This behaviour's name, its reference")]
	public string m_name;

	[Tooltip("This behaviour's type (Straight, Wave, FireShotPattern, ExplodeOnDeath")]
	public string m_type;

	public static void LoadAll() {
		m_loadedBehaviours.Clear();

		foreach(TextAsset loadedBehaviour in Resources.LoadAll<TextAsset>("ProjectileBehaviours")) {
			ProjectileBehaviour behaviour = Load(loadedBehaviour.text);

			if(behaviour) m_loadedBehaviours.Add(behaviour);
		}

		string[] files = Directory.GetFiles(Application.dataPath + "/Data/ProjectileBehaviours/");

		if(files.Length > 0)
			foreach(string file in files) {
				if(file.ToLower().EndsWith(".json")) {
					StreamReader reader = new StreamReader(file);
					ProjectileBehaviour behaviour = Load(reader.ReadToEnd());

					if(behaviour) m_loadedBehaviours.Add(behaviour);
					reader.Close();
				}
			}
	}

	private static ProjectileBehaviour Load(string p_json) {
		ProjectileBehaviour behaviour = JsonUtility.FromJson<ProjectileBehaviour>(p_json);
		Type type = null;

        if(!behaviour) return null;

		switch(behaviour.m_type.ToLower()) {
			case "straight": case "line":
				type = typeof(StraightBehaviour); break;
			case "wavy": case "wave":
				type = typeof(WavyBehaviour); break;
			case "fireshotpattern": case "firepattern": case "pattern": case "shotpattern":
				type = typeof(FireShotPatternBehaviour); break;
			case "explodeondeath": case "explode": case "explodedeath": case "explosion": case "deathexplosion":
				type = typeof(ExplodeOnDeathBehaviour); break;
		}

		if(type == null) return null;

		return (ProjectileBehaviour) JsonUtility.FromJson(p_json, type);
	}

	public static ProjectileBehaviour Get(string p_name, bool p_reference) {
		ProjectileBehaviour found = m_loadedBehaviours.Find(pb => pb.m_name == p_name);

		if(found) return p_reference ? found : found.Clone();

		return null;
	}

	public virtual void Init(Projectile p_projectile) { }
	public virtual void Move(Projectile p_projectile) { }
	public virtual void Die(Projectile p_projectile) { }

	public virtual ProjectileBehaviour Clone() {
		ProjectileBehaviour behaviour = (ProjectileBehaviour) Activator.CreateInstance(GetType());

		behaviour.m_name = m_name;
		behaviour.m_type = m_type;

		return behaviour;
	}

    public static implicit operator bool(ProjectileBehaviour p_instance){
        return p_instance != null;
    }
}
