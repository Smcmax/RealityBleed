using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour {

    [Tooltip("The template to use for every enemy in the game")]
    public GameObject m_enemyTemplate;

    [Tooltip("Are we using external data to generate enemies?")]
    public bool m_useExternalTypes;

    [Tooltip("All included possible types of enemies in the game")]
    [HideInInspector] public List<EnemyType> m_types = new List<EnemyType>();

    [Tooltip("All externally loaded types of enemies (includes existing types from m_types if they contain modifications)")]
    [HideInInspector] public List<EnemyType> m_externalTypes = new List<EnemyType>();

    [Tooltip("Internal and external types merged together")]
    [HideInInspector] public List<EnemyType> m_combinedTypes = new List<EnemyType>();

    void Awake() {
        LoadTypes();
    }

    public void LoadTypes() {
        m_types.Clear();
        m_externalTypes.Clear();
        m_combinedTypes.Clear();

        TextAsset[] types = Resources.LoadAll<TextAsset>("EnemyTypes");

        foreach(TextAsset typeText in types) {
            EnemyType type = JsonUtility.FromJson<EnemyType>(typeText.text);

            if(type != null) m_types.Add(type);
        }

        List<string> files = new List<string>();
        FileSearch.RecursiveRetrieval(Application.dataPath + "/Data/EnemyTypes/", ref files);

        if(files.Count > 0)
            foreach(string file in files) {
                if(file.ToLower().EndsWith(".json")) {
                    StreamReader reader = new StreamReader(file);
                    EnemyType type = JsonUtility.FromJson<EnemyType>(reader.ReadToEnd());

                    if(type != null) m_externalTypes.Add(type);
                    reader.Close();
                }
            }

        foreach(EnemyType type in m_types) {
            EnemyType external = m_externalTypes.Find(nt => nt.m_type == type.m_type);

            if(external != null) {
                EnemyType combined = type.Clone();

                combined.Combine(external, true);
                m_combinedTypes.Add(combined);
            } else m_combinedTypes.Add(type);
        }

        if(m_externalTypes.Count > 0)
            foreach(EnemyType external in m_externalTypes)
                if(!m_types.Exists(nt => nt.m_type == external.m_type)) {
                    external.AppendDataMarker();
                    m_combinedTypes.Add(external);
                }
    }

    public void CleanUp() {
        Resources.UnloadUnusedAssets();
    }

    public void Generate(string p_type) {
        if(m_useExternalTypes) Generate(m_combinedTypes.Find(et => et.m_type == p_type));
        else Generate(m_types.Find(et => et.m_type == p_type));
    }

    public void Generate(EnemyType p_type) {
        GameObject enemyObject = Instantiate(m_enemyTemplate);
        SpriteRenderer renderer = enemyObject.GetComponent<SpriteRenderer>();
        StateController controller = enemyObject.GetComponent<StateController>();
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        EnemyType type = p_type;

        List<SerializableSprite> sprites = new List<SerializableSprite>();
        List<string> deathSounds = new List<string>();
        List<int> minStats = new List<int>(type.m_minimumStats);
        List<int> maxStats = new List<int>(type.m_maximumStats);
        bool male = Random.Range(0, 100) >= 50;
        DropTable equipmentTable = new DropTable();
        DropTable dropTable = new DropTable();

        if(m_useExternalTypes) type = m_combinedTypes.Find(et => et.m_type == p_type.m_type);

        if(male) {
            if(type.m_maleSprites.Count > 0)
                sprites.AddRange(type.m_maleSprites);

            if(type.m_maleDeathSounds.Count > 0)
                deathSounds.AddRange(type.m_maleDeathSounds);
        } else {
            if(type.m_femaleSprites.Count > 0)
                sprites.AddRange(type.m_femaleSprites);

            if(type.m_femaleDeathSounds.Count > 0)
                deathSounds.AddRange(type.m_femaleDeathSounds);
        }

        if(type.m_equipmentTable) equipmentTable = type.m_equipmentTable;
        if(type.m_dropTable) dropTable = type.m_dropTable;

        enemy.Init();

        enemyObject.name = type.m_type;

        SerializableSprite sprite = sprites[Random.Range(0, sprites.Count)];
        sprite.m_name = (male ? "Male/" : "Female/") + sprite.m_name;
        renderer.sprite = sprite.Sprite;

        if(deathSounds.Count > 0)
            enemy.m_entity.m_deathSound = deathSounds[Random.Range(0, deathSounds.Count)];

        enemy.m_dialog.m_dialogTemplate = GameObject.Find("UI").transform.Find("Dialogue Canvas").Find("Speech Bubble").gameObject;
        enemy.m_dialog.m_currentDialog = Dialog.Get(type.m_greetings[Random.Range(0, type.m_greetings.Count)]);
        enemy.m_dialog.Init();

        enemy.transform.position = Player.GetPlayerFromId(0).transform.position;
        enemy.m_entity.m_feedbackTemplate = GameObject.Find("UI").transform.Find("Feedback Canvas").Find("Feedback").gameObject;

        if(type.m_defaultStates.Count > 0 && type.m_looks.Count > 0) {
            enemy.m_entity.m_look = type.m_looks[Random.Range(0, type.m_looks.Count)];

            controller.m_enemyEntitiesSets.Add("players");
            controller.m_enemyEntitiesSets.Add("npcs");
            controller.m_currentState = State.Get(type.m_defaultStates[Random.Range(0, type.m_defaultStates.Count)]);
            controller.Setup();
        }

        if(equipmentTable && equipmentTable.m_items.Count > 0)
            equipmentTable.DropAndEquip(enemy.m_entity.m_equipment);

        if(dropTable && dropTable.m_items.Count > 0) {
            enemy.m_entity.m_dropInventoryOnDeath = false;
            enemy.m_entity.m_lootTable = dropTable;
        } else enemy.m_entity.m_dropInventoryOnDeath = true;

        StartCoroutine(SetStats(enemy, minStats, maxStats));
    }

    private IEnumerator SetStats(Enemy p_enemy, List<int> p_minStats, List<int> p_maxStats) {
        yield return new WaitForSeconds(0.05f);

        for(int i = 0; i < UnitStats.STAT_AMOUNT; i++) {
            bool defaultStat = p_minStats.Count <= i || p_maxStats.Count <= i;
            p_enemy.m_entity.m_stats.SetBaseStat((Stats) i, defaultStat ? 5 : Random.Range(p_minStats[i], p_maxStats[i]));
        }
    }
}