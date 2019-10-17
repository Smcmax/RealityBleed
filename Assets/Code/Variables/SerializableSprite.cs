using UnityEngine;

[System.Serializable]
public class SerializableSprite {
	
	[Tooltip("The name of the sprite to load")]
	public string m_name = "";

	[HideInInspector] public bool m_internal = true;

	private Sprite m_sprite = null;

	public Sprite Sprite { 
		get { 
			if(m_sprite == null) { 
				if(m_internal) m_sprite = SpriteUtils.LoadSpriteFromResources(m_name);
				else m_sprite = SpriteUtils.LoadSpriteFromFile(Application.dataPath + "/Data/Sprites/" + m_name + ".png");
			}

			return m_sprite;
		} set {
			m_sprite = value;
		}
	}

	public static implicit operator Sprite(SerializableSprite p_instance) { 
		return p_instance.Sprite;
	}

	public static implicit operator bool(SerializableSprite p_instance) { 
		return p_instance != null && p_instance.m_sprite != null;
	}

	public static implicit operator SerializableSprite(Sprite p_sprite) { 
		return new SerializableSprite { Sprite = p_sprite };
	}
}