using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public class StraightProjectileMovementJob : MonoBehaviour {

	[HideInInspector] public List<Projectile> m_projectiles;
	[HideInInspector] public List<StraightProjData> m_dataList;

	private List<Transform> m_transforms;
	private TransformAccessArray m_transformsAccArray;
	private NativeArray<StraightProjData> m_dataArray;
	private JobHandle m_jobHandle;
	private StraightProjMoveJob m_job;
	private static int ID;

	void Start() { 
		m_transforms = new List<Transform>();
		m_dataList = new List<StraightProjData>();
	}

	public void AddProjectile(Projectile p_projectile) {
		m_projectiles.Add(p_projectile);
		m_transforms.Add(p_projectile.transform);

		StraightProjData data = new StraightProjData { 
			ID = ID++,
			Speed = p_projectile.m_speed,
			Direction = new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0)
		};

		p_projectile.m_straightProjData = data;
		m_dataList.Add(data);
	}

	public void RemoveProjectile(Projectile p_projectile) {
		m_projectiles.Remove(p_projectile);
		m_transforms.Remove(p_projectile.transform);
		m_dataList.RemoveAll(d => d.ID == p_projectile.m_straightProjData.ID);
	}

	void Update() {
		if(Time.timeScale == 0f) return;

		m_transformsAccArray = new TransformAccessArray(m_transforms.ToArray());
		m_dataArray = new NativeArray<StraightProjData>(m_dataList.ToArray(), Allocator.Temp);

		m_job = new StraightProjMoveJob {
			DeltaTime = Time.deltaTime,
			DataArray = m_dataArray
		};

		m_jobHandle = m_job.Schedule(m_transformsAccArray);

		JobHandle.ScheduleBatchedJobs();
	}

	void LateUpdate() {
		m_jobHandle.Complete();

		if(m_transformsAccArray.isCreated) m_transformsAccArray.Dispose();
		if(m_dataArray.IsCreated) m_dataArray.Dispose();
	}

	[BurstCompile]
	struct StraightProjMoveJob : IJobParallelForTransform {
		public float DeltaTime;
		public NativeArray<StraightProjData> DataArray;

		public void Execute(int p_index, TransformAccess p_transform) {
			StraightProjData data = DataArray[p_index];
			p_transform.position += (Vector3) (data.Speed * DeltaTime * data.Direction);
		}
	}

	public struct StraightProjData { 
		public int ID;
		public float Speed;
		public float3 Direction;
	}
}