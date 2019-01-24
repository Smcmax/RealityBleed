using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public class StraightBehaviourJob : ProjectileMovementJob {

	private NativeArray<StraightBehaviourData> m_dataArray;
	private StraightBehaviourMoveJob m_job;

	public override bool CanAdd(ProjectileBehaviour p_behaviour) {
		return p_behaviour is StraightBehaviour;
	}

	protected override IProjData CreateData(Projectile p_projectile, ProjectileBehaviour p_behaviour, int p_id) {
		return new StraightBehaviourData { 
			ID = p_id,
			Direction = new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0),
			Speed = p_projectile.m_speed
		};
	}

	protected override void CreateJob(List<IProjData> p_dataList) {
		StraightBehaviourData[] dataArray = new StraightBehaviourData[p_dataList.Count];

		for(int i = 0; i < p_dataList.Count; i++)
			dataArray[i] = (StraightBehaviourData) p_dataList[i];
		
		m_dataArray = new NativeArray<StraightBehaviourData>(dataArray, Allocator.Temp);
		m_job = new StraightBehaviourMoveJob { 
			DeltaTime = Time.deltaTime,
			DataArray = m_dataArray
		};
	}

	protected override JobHandle ScheduleJob(TransformAccessArray p_transforms) {
		return m_job.Schedule(p_transforms);
	}

	protected override void Dispose() {
		if(m_dataArray.IsCreated) m_dataArray.Dispose();
	}

	[BurstCompile]
	struct StraightBehaviourMoveJob : IJobParallelForTransform {
		public float DeltaTime;
		public NativeArray<StraightBehaviourData> DataArray;

		public void Execute(int p_index, TransformAccess p_transform) {
			if(DataArray.Length <= p_index) return;

			StraightBehaviourData data = DataArray[p_index];
			p_transform.position += (Vector3)(data.Speed * DeltaTime * data.Direction);
		}
	}

	public struct StraightBehaviourData : IProjData {
		public int ID;
		public float Speed;
		public float3 Direction;

		public int GetID() {
			return ID;
		}
	}
}