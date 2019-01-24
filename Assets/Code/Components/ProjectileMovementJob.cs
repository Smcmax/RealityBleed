using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public abstract class ProjectileMovementJob : MonoBehaviour {

	[HideInInspector] public Dictionary<Projectile, int> m_projectiles;
	[HideInInspector] public List<IProjData> m_dataList;

	private List<Transform> m_transforms;
	private TransformAccessArray m_transformsAccArray;
	private JobHandle m_jobHandle;
	private static int ID;

	void Start() { 
		m_projectiles = new Dictionary<Projectile, int>();
		m_transforms = new List<Transform>();
		m_dataList = new List<IProjData>();
	}

	public abstract bool CanAdd(ProjectileBehaviour p_behaviour);

	public void AddProjectile(Projectile p_projectile, ProjectileBehaviour p_behaviour) {
		int id = ID++;

		m_projectiles.Add(p_projectile, id);
		m_transforms.Add(p_projectile.transform);
		m_dataList.Add(CreateData(p_projectile, p_behaviour, id));
	}

	public void RemoveProjectile(Projectile p_projectile) {
		int id = -1;
		bool success = m_projectiles.TryGetValue(p_projectile, out id);

		if(success) {
			m_projectiles.Remove(p_projectile);
			m_transforms.Remove(p_projectile.transform);
			m_dataList.RemoveAll(d => d.GetID() == id);
		}
	}

	void Update() {
		if(Time.timeScale == 0f) return;

		m_transformsAccArray = new TransformAccessArray(m_transforms.ToArray());
		CreateJob(m_dataList);
		m_jobHandle = ScheduleJob(m_transformsAccArray);

		JobHandle.ScheduleBatchedJobs();
	}

	void LateUpdate() {
		m_jobHandle.Complete();

		if(m_transformsAccArray.isCreated) m_transformsAccArray.Dispose();

		Dispose();
	}

	protected void UpdateData(IProjData[] p_dataArray) { 
		foreach(IProjData data in p_dataArray)
			m_dataList[m_dataList.FindIndex(d => d.GetID() == data.GetID())] = data;
	}

	protected abstract IProjData CreateData(Projectile p_projectile, ProjectileBehaviour p_behaviour, int p_id);
	protected abstract void CreateJob(List<IProjData> p_dataList);
	protected abstract JobHandle ScheduleJob(TransformAccessArray p_transforms);
	protected abstract void Dispose();
}

public interface IProjData { 
	int GetID();
}