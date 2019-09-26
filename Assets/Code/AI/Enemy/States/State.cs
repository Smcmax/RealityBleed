using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

[Serializable]
public class State {

    [Tooltip("All included possible states in the game")]
    public static List<State> m_states = new List<State>();

    [Tooltip("All externally loaded states")]
    public static List<State> m_externalStates = new List<State>();

    [Tooltip("Internal and external states in a single list. Note that if an external and an internal state have the same name, the external will load over it")]
    public static List<State> m_combinedStates = new List<State>();

    [Tooltip("Are we using external states on top of the internal ones?")]
    public static bool m_useExternalStates = true; // TODO: change to reflect modded usage instead of a hard true

    [Tooltip("The name of the state")]
    public string m_name;

    [Tooltip("Contains a state's actions, runs every update tick")]
	public List<Action> m_actions;

	[Tooltip("Transitions are checked every update tick, if the conditions are met, the state will change")]
	public List<Transition> m_transitions;

    [Tooltip("The serializable gizmo color used in the editor")]
	public SerializableColor m_sceneGizmoColor;

    public static void LoadAll() {
        m_states.Clear();
        m_externalStates.Clear();
        m_combinedStates.Clear();

        foreach(TextAsset loadedState in Resources.LoadAll<TextAsset>("States")) {
            State state = Load(loadedState.text);

            if(state) m_states.Add(state);
        }

        List<string> files = new List<string>();
        FileSearch.RecursiveRetrieval(Application.dataPath + "/Data/States/", ref files);

        if(files.Count > 0)
            foreach(string file in files) {
                if(file.ToLower().EndsWith(".json")) {
                    StreamReader reader = new StreamReader(file);
                    State state = Load(reader.ReadToEnd());

                    if(state) m_externalStates.Add(state);
                    reader.Close();
                }
            }

        foreach(State state in m_states) {
            State external = m_externalStates.Find(s => s.m_name == state.m_name);

            if(external) m_combinedStates.Add(external);
            else m_combinedStates.Add(state);
        }

        if(m_externalStates.Count > 0)
            foreach(State external in m_externalStates)
                if(!m_states.Exists(s => s.m_name == external.m_name))
                    m_combinedStates.Add(external);
    }

    public static State Load(string p_json) { // this is really jank
        State state = JsonUtility.FromJson<State>(p_json);

        state.m_actions = new List<Action>();
        state.m_transitions = new List<Transition>();

        string jsonActions = p_json.Split(new string[]{ "m_actions" }, StringSplitOptions.RemoveEmptyEntries)[1]
                                   .Split('[')[1].Split(']')[0];

        foreach(string split in jsonActions.Split('{'))
                foreach(string jsonAction in split.Split('}'))
                    if(jsonAction.Contains("m_type"))
                        state.m_actions.Add(Action.Load("{" + jsonAction + "}"));

        if(p_json.Contains("m_transitions")) {
            string jsonTransitions = p_json.Split(new string[]{ "m_transitions" }, StringSplitOptions.RemoveEmptyEntries)[1]
                                           .Split('[')[1].Split(']')[0];

            foreach(string split in jsonTransitions.Split('{'))
                foreach(string jsonTransition in split.Split('}'))
                    if(jsonTransition.Contains("m_condition")) {
                        string jsonCondition = jsonTransition.Split(new string[] { "m_condition" }, StringSplitOptions.RemoveEmptyEntries)[1]
                                                             .Split('{')[1].Split('}')[0];
                        Transition transition = JsonUtility.FromJson<Transition>("{" + jsonTransition + "}");

                        transition.m_condition = Condition.Load("{" + jsonCondition + "}");
                        state.m_transitions.Add(transition);
                }
        }

        return state;
    }

    public static State Get(string p_name) {
        List<State> availableStates = m_useExternalStates ? m_combinedStates : m_states;

        return availableStates.Find(s => s.m_name == p_name);
    }

    public void UpdateState(StateController p_controller) {
		ExecuteActions(p_controller);
		CheckTransitions(p_controller);
	}

	private void ExecuteActions(StateController p_controller) {
		foreach(Action action in m_actions)
			action.Execute(p_controller);
	}

	private void CheckTransitions(StateController p_controller) {
        if(m_transitions.Count == 0) return;

		foreach(Transition transition in m_transitions)
			if(transition.m_condition.Test(p_controller)) {
				if(p_controller.TransitionToState(Get(transition.m_trueState))) return;
			} else {
				if(p_controller.TransitionToState(Get(transition.m_falseState))) return;
			}
	}

    public static implicit operator bool(State p_instance) {
        return p_instance != null;
    }
}
