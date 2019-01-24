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
			Speed = p_projectile.m_speed,
			Direction = new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0),
			Range = behaviour.m_range,
			Distance = behaviour.m_range / 2,
			Steps = behaviour.m_steps,
			Reverse = 0
		};
	}

	protected override void CreateJob(List<IProjData> p_dataList) {
		WavyBehaviourData[] dataArray = new WavyBehaviourData[p_dataList.Count];

		for(int i = 0; i < p_dataList.Count; i++)
			dataArray[i] = (WavyBehaviourData) p_dataList[i];

		m_dataArray = new NativeArray<WavyBehaviourData>(dataArray, Allocator.Temp);
		m_job = new WavyBehaviourMoveJob {
			DeltaTime = Time.deltaTime,
			DataArray = m_dataArray
		};
	}

	protected override JobHandle ScheduleJob(TransformAccessArray p_transforms) {
		return m_job.Schedule(p_transforms);
	}

	protected override void Dispose() {
		if(m_dataArray.IsCreated) {
			IProjData[] dataArray = new IProjData[m_dataArray.Length];

			for(int i = 0; i < m_dataArray.Length; i++)
				dataArray[i] = m_dataArray[i];

			UpdateData(dataArray);

			m_dataArray.Dispose();
		}
	}

	[BurstCompile]
	struct WavyBehaviourMoveJob : IJobParallelForTransform {
		[ReadOnly] public float DeltaTime;
		public NativeArray<WavyBehaviourData> DataArray;

		public void Execute(int p_index, TransformAccess p_transform) {
			if(DataArray.Length <= p_index) return;

			WavyBehaviourData data = DataArray[p_index];
			float3 moveVector = data.Direction * data.Speed * DeltaTime;
			float2 perpendicular = Vector2.Perpendicular((Vector3) moveVector);
			float perpX = Mathf.Abs(perpendicular.x);
			float perpY = Mathf.Abs(perpendicular.y);
			float stepDistance = data.Range / (float) data.Steps;
			float totalPerpendicularMovement = perpX + perpY;
			float2 sideMovement = new float2(stepDistance * (perpX / totalPerpendicularMovement),
													stepDistance * (perpY / totalPerpendicularMovement));

			if(perpX < 0) sideMovement.x *= -1;
			if(perpY < 0) sideMovement.y *= -1;

			if(data.Reverse == 1) {
				sideMovement.x *= -1;
				sideMovement.y *= -1;
			}
			
			float sideDist = Vector2.Distance((Vector3) moveVector, sideMovement);

			data.Distance += data.Reverse == 1 ? -sideDist : sideDist;
			moveVector += new float3(sideMovement.x, sideMovement.y, 0);
			p_transform.position += (Vector3) moveVector;

			if(data.Distance >= data.Range) data.Reverse = 1;
			else if(data.Distance <= 0) data.Reverse = 0;

			DataArray[p_index] = data;
		}
	}

	public struct WavyBehaviourData : IProjData {
		public int ID;
		public float Speed;
		public float3 Direction;
		public float Distance;
		public float Range;
		public int Steps;
		public int Reverse;

		public int GetID() { 
			return ID;
		}
	}
}