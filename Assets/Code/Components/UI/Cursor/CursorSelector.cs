using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorSelector : MonoBehaviour {

	[Tooltip("The image displaying the current cursor")]
	public Image m_spriteDisplay;

	[Tooltip("The file browser's reference")]
	public CFileBrowser m_fileBrowser;

	private List<FileSprite> m_sprites;
	private int m_currentIndex;

	void OnEnable() {
		Load();
	}

	public void Load() {
		m_sprites = new List<FileSprite>();
		m_currentIndex = 0;

		if(!MenuHandler.Instance || MenuHandler.Instance.m_handlingPlayer == null) return;

        string loadedCursor = Game.m_options.Get("CursorSprite", MenuHandler.Instance.m_handlingPlayer.id).GetString();
		int index = 0;

        foreach(string file in System.IO.Directory.GetFiles(Application.dataPath + "/Data/Cursors/")) {
			if(file.ToLower().EndsWith(".png")) {
				Sprite sprite = SpriteUtils.LoadSpriteFromFile(file);
				string name = Path.GetFileNameWithoutExtension(file);

				if(sprite == null) continue;

				m_sprites.Add(new FileSprite(name, sprite));

				if(name.Equals(loadedCursor)) m_currentIndex = index;

				index++;
			}
		}

		Display(m_currentIndex);
	}

	public void Browse() {
		m_fileBrowser.Open(GetComponentInParent<Menu>(), "", "", "png");
        m_fileBrowser.OnFileSelect += HandleBrowsingResult;
	}

	public void HandleBrowsingResult(string p_file) {
		string copiedFile = Application.dataPath + "/Data/Cursors/" + Path.GetFileName(p_file);

		File.Copy(p_file, copiedFile);
		Load();
	}

	public void Apply() {
        Game.m_options.Get("CursorSprite", MenuHandler.Instance.m_handlingPlayer.id).Save(m_sprites[m_currentIndex].Name);
		GetPlayerCursor().SetCursorImage(m_sprites[m_currentIndex].Sprite);
	}

	public void Previous() {
		Display(m_currentIndex - 1 < 0 ? m_sprites.Count - 1 : m_currentIndex - 1);
	}

	public void Next() {
		Display(m_currentIndex + 1 == m_sprites.Count ? 0 : m_currentIndex + 1);
	}

	private void Display(int p_index) {
		m_spriteDisplay.sprite = m_sprites[p_index].Sprite;
		m_currentIndex = p_index;
	}

	private PlayerCursor GetPlayerCursor() {
        if(Player.m_players.Count > 0)
            return Player.GetPlayerFromId(MenuHandler.Instance.m_handlingPlayer.id).m_mouse;
        else
            return GameObject.Find("PlayerCursor" + MenuHandler.Instance.m_handlingPlayer.id).GetComponent<PlayerCursor>();
	}
}

[Serializable]
public class FileSprite {
	public string Name;
	public Sprite Sprite;

	public FileSprite(string p_name, Sprite p_sprite) {
		Name = p_name;
		Sprite = p_sprite;
	}
}