using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public class WavyBehaviourJob : ProjectileMovementJob {

	private NativeArray<WavyBehaviourData> m_dataArray;
	private WavyBehaviourMoveJob m_job;

	public override bool CanAdd(ProjectileBehaviour p_behaviour) {
		return p_behaviour is WavyBehaviour;
	}

	protected override IProjData CreateData(Projectile p_projectile, ProjectileBehaviour p_behaviour, int p_id) {
		WavyBehaviour behaviour = (WavyBehaviour) p_behaviour;

		return new WavyBehaviourData {
			ID = p_id,
			Speed = p_projectile.m_info.m_speed,
			Direction = new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0),
			Frequency = behaviour.m_frequency,
			Magnitude = behaviour.m_magnitude,
			Axis = p_projectile.transform.right,
			SpriteRotation = p_projectile.m_info.m_spriteRotation,
			StartTime = Time.time
		};
	}

	protected override void CreateJob(List<IProjData> p_dataList) {
		WavyBehaviourData[] dataArray = new WavyBehaviourData[p_dataList.Count];

		for(int i = 0; i < p_dataList.Count; i++)
			dataArray[i] = (WavyBehaviourData) p_dataList[i];

		m_dataArray = new NativeArray<WavyBehaviourData>(dataArray, Allocator.Temp);
		m_job = new WavyBehaviourMoveJob {
			DeltaTime = Time.deltaTime,
			Time = Time.time,
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
	struct WavyBehaviourMoveJob : IJobParallelForTransform {
		[ReadOnly] public float DeltaTime;
		[ReadOnly] public float Time;
		public NativeArray<WavyBehaviourData> DataArray;

		public void Execute(int p_index, TransformAccess p_transform) {
			if(DataArray.Length <= p_index) return;

			WavyBehaviourData data = DataArray[p_index];
			float3 originalPos = p_transform.position;
			float3 pos = originalPos + data.Direction * data.Speed * DeltaTime;
			float angle = Mathf.Sin((Time - data.StartTime) * data.Frequency + Mathf.PI / 2f);

			p_transform.position = pos + (float3) data.Axis * angle * data.Magnitude * DeltaTime;
			p_transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(data.Direction.y, data.Direction.x) * Mathf.Rad2Deg - 
														angle * Mathf.Rad2Deg + data.SpriteRotation, Vector3.forward);
		}
	}

	public struct WavyBehaviourData : IProjData {
		public int ID;
		public float Speed;
		public float Frequency;
		public float Magnitude;
		public float3 Direction;
		public Vector3 Axis;
		public float SpriteRotation;
		public float StartTime;

		public int GetID() { 
			return ID;
		}
	}
}