using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public class WavyProjectileMovementJob : MonoBehaviour {

	[HideInInspector] public List<Projectile> m_projectiles;
	[HideInInspector] public List<WavyProjData> m_dataList;
	
	private List<Transform> m_transforms;
	private TransformAccessArray m_transformsAccArray;
	private NativeArray<WavyProjData> m_dataArray;
	private JobHandle m_jobHandle;
	private WavyProjMoveJob m_job;
	private static int ID;

	void Start() { 
		m_transforms = new List<Transform>();
		m_dataList = new List<WavyProjData>();
	}

	public void AddProjectile(Projectile p_projectile, WavyBehaviour p_behaviour) { 
		m_projectiles.Add(p_projectile);
		m_transforms.Add(p_projectile.transform);

		WavyProjData data = new WavyProjData { 
			ID = ID++,
			Speed = p_projectile.m_speed,
			Direction = new float3(p_projectile.m_direction.x, p_projectile.m_direction.y, 0),
			Range = p_behaviour.m_range,
			Distance = p_behaviour.m_range / 2,
			Steps = p_behaviour.m_steps,
			Reverse = 0
		};

		p_projectile.m_wavyProjData = data;
		m_dataList.Add(data);
	}

	public void RemoveProjectile(Projectile p_projectile) {
		m_projectiles.Remove(p_projectile);
		m_transforms.Remove(p_projectile.transform);
		m_dataList.RemoveAll(d => d.ID == p_projectile.m_wavyProjData.ID);
	}

	void Update() {
		if(Time.timeScale == 0f) return;

		m_transformsAccArray = new TransformAccessArray(m_transforms.ToArray());
		m_dataArray = new NativeArray<WavyProjData>(m_dataList.ToArray(), Allocator.Temp);

		m_job = new WavyProjMoveJob {
			DeltaTime = Time.deltaTime,
			DataArray = m_dataArray
		};

		m_jobHandle = m_job.Schedule(m_transformsAccArray);

		JobHandle.ScheduleBatchedJobs();
	}

	void LateUpdate() {
		m_jobHandle.Complete();

		if(m_transformsAccArray.isCreated) m_transformsAccArray.Dispose();
		if(m_dataArray.IsCreated) {
			for(int i = 0; i < m_dataArray.Length; i++)
				m_dataList[i] = m_dataArray[i];

			m_dataArray.Dispose();
		}
	}

	[BurstCompile]
	struct WavyProjMoveJob : IJobParallelForTransform {
		[ReadOnly] public float DeltaTime;
		public NativeArray<WavyProjData> DataArray;

		public void Execute(int p_index, TransformAccess p_transform) {
			if(DataArray.Length <= p_index) return;

			WavyProjData data = DataArray[p_index];
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

			if (data.Distance >= data.Range) data.Reverse = 1;
			else if(data.Distance <= 0) data.Reverse = 0;

			DataArray[p_index] = data;
		}
	}

	public struct WavyProjData {
		public int ID;
		public float Speed;
		public float3 Direction;
		public float Distance;
		public float Range;
		public int Steps;
		public int Reverse;
	}
}