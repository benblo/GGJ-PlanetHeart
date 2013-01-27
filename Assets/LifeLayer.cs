using UnityEngine;
using System.Collections;

[System.Serializable]
public class TreeType
{
	public Vector2 size = Vector2.one;
	public int atlasY0;
	public int atlasX0;
	public int animCount = 3;
	public int growthCount = 3;
	public SpriteLayer layer;
}

public class LifeLayer : MonoBehaviour
{
	public WorldGrid grid;
	
	public TreeType[] treeTypes;
	public float playRate = 1;
	
	void Update()
	{
		foreach ( Tree tree in grid.trees )
		{
			TreeType type = tree.type;
			
			Sprite sprite = new Sprite();
			type.layer.sprites.Add(sprite);
			
			sprite.pos = tree.position;
			sprite.pos.x += type.size.x * 0.5f;
			sprite.pos.y += type.size.y * 0.5f - 1;
			
			sprite.size = type.size;
			
			sprite.atlasPos.x = type.atlasX0 + tree.growthLevel;
			
			float cursor = ((Time.time + tree.animOffset) % playRate) / playRate;
			sprite.atlasPos.y = type.atlasY0 + Mathf.FloorToInt(type.animCount * cursor);
		}
	}
}
