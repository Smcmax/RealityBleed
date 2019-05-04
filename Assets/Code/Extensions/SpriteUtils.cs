using UnityEngine;
using System.IO;

public static class SpriteUtils {

    public static Sprite LoadSpriteFromFile(string p_file) {
        Texture2D texture = LoadTexture(p_file);

        if(texture == null) return null;

        texture.filterMode = FilterMode.Point;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.Tight);
        sprite.name = Path.GetFileNameWithoutExtension(p_file);

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