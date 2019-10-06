using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorSelector : Selectable {

	[Tooltip("The image displaying the current cursor")]
	public Image m_spriteDisplay;

	[Tooltip("The file browser's reference")]
	public CFileBrowser m_fileBrowser;

	private List<FileSprite> m_sprites;
	private int m_currentIndex;
	private bool m_xPressed = false;

	protected override void OnEnable() {
		base.OnEnable();
		Load();
	}

	void Update() {
		if(EventSystem.current.currentSelectedGameObject == gameObject) {
			float xAxis = MenuHandler.Instance.m_handlingPlayer.GetAxisRaw("UIMoveX");

			if(xAxis != 0 && !m_xPressed) {
				m_xPressed = true;

				if(xAxis > 0) Next();
				else if(xAxis < 0) Previous();
			} else if(Math.Abs(xAxis) < 0.01f) m_xPressed = false;
		}
	}

	public void Load() {
		List<FileSprite> sprites = new List<FileSprite>();
		m_currentIndex = 0;

		if(!MenuHandler.Instance || MenuHandler.Instance.m_handlingPlayer == null) return;

        string loadedCursor = Game.m_options.Get("CursorSprite", MenuHandler.Instance.m_handlingPlayer.id).GetString();

        foreach(string file in Directory.GetFiles(Application.dataPath + "/Data/Cursors/")) {
			if(file.ToLower().EndsWith(".png")) {
                string name = Path.GetFileNameWithoutExtension(file);

                try {
                    FileSprite existingSprite = m_sprites.Find(fs => fs.Name.Equals(name));
                    sprites.Add(existingSprite);
                } catch(NullReferenceException) {
                    Sprite sprite = SpriteUtils.LoadSpriteFromFile(file);

                    if(sprite == null) continue;

                    sprites.Add(new FileSprite(name, sprite));
                }
			}
		}

        m_sprites = sprites;
        m_currentIndex = m_sprites.FindIndex(fs => fs.Name.Equals(loadedCursor));

		Display(m_currentIndex);
	}

	public void Browse() {
		m_fileBrowser.Open(GetComponentInParent<Menu>(), "", "", "png");
        m_fileBrowser.OnFileSelect += HandleBrowsingResult;
	}

	public void HandleBrowsingResult(string p_file) {
		string copiedFile = Application.dataPath + "/Data/Cursors/" + Path.GetFileName(p_file);

		File.Copy(p_file, copiedFile);
		Game.m_options.Get("CursorSprite", MenuHandler.Instance.m_handlingPlayer.id).Save(Path.GetFileNameWithoutExtension(p_file));
		Load();
		GetPlayerCursor().SetCursorImage(m_sprites[m_currentIndex].Sprite);
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