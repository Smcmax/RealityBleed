using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class StateController : MonoBehaviour {

    [Tooltip("The state currently in execution in this controller")]
	public State m_currentState;

	[Tooltip("Patrol waypoints")]
	public List<Transform> m_waypoints; // TODO: loadable?

	[Tooltip("The list of possible enemy entity lists for this entity, entities it can acquire as targets")]
	public List<string> m_enemyEntitiesSets;

	public bool m_drawGizmos;

	[HideInInspector] public Entity m_entity;
	[HideInInspector] public Look m_look;
	[HideInInspector] public Entity m_target;
	[HideInInspector] public float m_stateTimeElapsed;
	[HideInInspector] public int m_nextWaypoint;
	[HideInInspector] public bool m_patrolFinished;
    [HideInInspector] public List<Point> m_path = new List<Point>();
    [HideInInspector] public float m_lastPathfindingUpdate;
    [HideInInspector] public int m_currentPathfindingCount;
    [HideInInspector] public Vector2 m_tempTarget;
    [HideInInspector] public float m_lastTempTarget;
    [HideInInspector] public float m_combatDropAttemptTime;
	[HideInInspector] public Dictionary<ShootAction, List<ShotPattern>> m_shotPatterns;

    public static TileGrid m_pathfindingGrid = null;
    public static Tilemap m_currentTilemap = null;

    public static void UpdateTilemap() {
        GameObject grid = GameObject.Find("Grid");

        if(grid) {
            m_currentTilemap = grid.transform.Find("Ground").GetComponent<Tilemap>();
            m_pathfindingGrid = Pathfinding.GenerateGridFromTilemaps(m_currentTilemap);
        }
    }

	public void Setup() {
		m_entity = GetComponent<Entity>();
		m_shotPatterns = new Dictionary<ShootAction, List<ShotPattern>>();
		m_look = m_entity.m_look;
		m_entity.m_ai = this;
    }

	void Update() {
		if(m_look && m_currentState) m_currentState.UpdateState(this);
    }

	public bool TransitionToState(string p_nextState) {
		if(p_nextState != m_currentState.m_name) { 
			foreach(Action action in m_currentState.m_actions)
				action.OnTransition(this);

			m_currentState = State.Get(p_nextState);
			OnExitState();

			return true;
		}

		return false;
	}

	public bool CheckCountdown(float p_duration) {
		m_stateTimeElapsed += Time.deltaTime;

		return m_stateTimeElapsed >= p_duration;
	}

	private void OnExitState() {
		m_stateTimeElapsed = 0;
		m_entity.m_controller.Stop();
		m_patrolFinished = false;
		m_entity.transform.rotation = Quaternion.identity;
	}

	void OnDrawGizmos() {
		if(m_currentState && m_drawGizmos) {
			if(m_entity && m_look) {
                if(m_currentState.m_sceneGizmoColor != null)
				    Gizmos.color = m_currentState.m_sceneGizmoColor;

				Gizmos.DrawWireSphere(m_entity.transform.position, m_look.m_lookSphereRadius);

				Debug.DrawRay(transform.position,
								  transform.right.normalized * m_look.m_lookRange,
								  m_currentState.m_sceneGizmoColor);

				Debug.DrawRay(transform.position,
								  (Quaternion.Euler(0, 0, -m_look.m_fieldOfView / 2) * transform.right)
									.normalized * m_look.m_lookRange,
								  Color.black);

				Debug.DrawRay(transform.position,
								  (Quaternion.Euler(0, 0, m_look.m_fieldOfView / 2) * transform.right)
									.normalized * m_look.m_lookRange,
								  Color.black);

                if(m_path.Count > 0) {
                    for(int i = 0; i < m_path.Count - 1; i++) {
                        Vector3 start = m_path[i].ConvertToWorld(m_currentTilemap);
                        Vector3 end = m_path[i + 1].ConvertToWorld(m_currentTilemap);

                        Debug.DrawLine(start, end);
                    }
                }
			}
		}
	}
}
