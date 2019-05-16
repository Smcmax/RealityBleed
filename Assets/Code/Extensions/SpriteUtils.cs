using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SpriteUtils {

	private static List<FileSprite> m_loadedSprites = new List<FileSprite>();

	public static Sprite LoadNPCSprite(string p_name) {
		FileSprite loaded = m_loadedSprites.Find(fs => fs.Name == p_name);
		if(loaded != null && loaded.Sprite) return loaded.Sprite;

		Sprite sprite = Resources.Load<Sprite>("Sprites/" + p_name);
		sprite.texture.filterMode = FilterMode.Point;

		m_loadedSprites.Add(new FileSprite(p_name, sprite));

		return sprite;
	}

    public static Sprite LoadSpriteFromFile(string p_file) {
		FileSprite loaded = m_loadedSprites.Find(fs => fs.Name == Path.GetFileNameWithoutExtension(p_file));
		if(loaded != null && loaded.Sprite) return loaded.Sprite;

        Texture2D texture = LoadTexture(p_file);
        if(texture == null) return null;

        texture.filterMode = FilterMode.Point;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.Tight);
        sprite.name = Path.GetFileNameWithoutExtension(p_file);

		m_loadedSprites.Add(new FileSprite(p_file, sprite));

        return sprite;
    }

    public static Texture2D LoadTexture(string p_file) {
        Texture2D texture;
        byte[] data;

        if(File.Exists(p_file)) {
            data = File.ReadAllBytes(p_file);
            texture = new Texture2D(2, 2);
            
            if(texture.LoadImage(data)) return texture;
        }

        return null;
    }
}