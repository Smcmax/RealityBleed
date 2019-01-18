using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public class StraightProjectileMovementJob : MonoBehaviour {

	[HideInInspector] public List<Projectile> m_projectiles;
	[HideInInspector] public List<float> m_speedList;
	[HideInInspector] public List<float3> m_directionList;

	private List<Transform> m_transforms;
	private TransformAccessArray m_transformsAccArray;
	private NativeArray<float> m_speedArray;
	private NativeArray<float3> m_directionArray;
	private JobHandle m_jobHandle;
	private StraightProjMoveJob m_job;

	void Start() { 
		m_transforms = new List<Transform>();
	}

	public void AddProjectile(Projectile p_projectile) { 
		m_projectiles.Add(p_projectile);
		m_transforms.Add(p_projectile.transform);
		m_speedList.Add(p_projectile.m_speed);
		m_directionList.Add(new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0));
	}

	public void RemoveProjectile(Projectile p_projectile) { 
		m_projectiles.Remove(p_projectile);
		m_transforms.Remove(p_projectile.transform);
		m_speedList.Remove(p_projectile.m_speed);
		m_directionList.Remove(new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0));
	}

	void Update() {
		if(Time.timeScale == 0f) return;

		m_transformsAccArray = new TransformAccessArray(m_transforms.ToArray());
		m_speedArray = new NativeArray<float>(m_speedList.ToArray(), Allocator.Temp);
		m_directionArray = new NativeArray<float3>(m_directionList.ToArray(), Allocator.Temp);

		m_job = new StraightProjMoveJob {
			DeltaTime = Time.deltaTime,
			SpeedArray = m_speedArray,
			DirectionArray = m_directionArray
		};

		m_jobHandle = m_job.Schedule(m_transformsAccArray);

		JobHandle.ScheduleBatchedJobs();
	}

	void LateUpdate() {
		m_jobHandle.Complete();

		if(m_transformsAccArray.isCreated) m_transformsAccArray.Dispose();
		if(m_speedArray.IsCreated) m_speedArray.Dispose();
		if(m_directionArray.IsCreated) m_directionArray.Dispose();
	}

	[BurstCompile]
	struct StraightProjMoveJob : IJobParallelForTransform {
		public float DeltaTime;
		public NativeArray<float> SpeedArray;
		public NativeArray<float3> DirectionArray;

		public void Execute(int p_index, TransformAccess p_transform) {
			p_transform.position += (Vector3) ((SpeedArray[p_index] * DeltaTime * DirectionArray[p_index]));
		}
	}
}