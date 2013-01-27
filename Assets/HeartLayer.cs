using UnityEngine;
using System.Collections;

public class HeartLayer : MonoBehaviour
{
	public SpriteLayer layer;
	public Vector2 pos = new Vector2(21, 31);
	public Vector2 size = new Vector2(5f, 5f);
	
	void Start()
	{
		Sprite sprite = new Sprite();
		layer.sprites.Add(sprite);
		
		sprite.pos = pos + new Vector2(0.5f, 0.5f);
		sprite.size = size;
		sprite.atlasPos.x = 0;
		sprite.atlasPos.y = 0;
		
		layer.buildMesh();
	}
}
