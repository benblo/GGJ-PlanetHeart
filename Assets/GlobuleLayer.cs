using UnityEngine;
using System.Collections;

public class GlobuleLayer : MonoBehaviour
{
	public WorldGrid grid;
	public SpriteLayer layer;
	public Vector2 size = new Vector2(0.5f, 0.5f);
	public Vector2 offset = new Vector2(0.5f, 0.5f);
	public int animCount = 4;
	public float sparkRate = 1;
	
	void Update()
	{
		float cursor = (Time.time * sparkRate) % 1f;
		float animStep = Mathf.Floor(animCount * cursor);
		
        foreach (Globule globule in grid.globules)
        {
            Sprite sprite = new Sprite();
            layer.sprites.Add(sprite);
			
            sprite.pos = globule.movement.Position + offset;
            sprite.size = size;
			
			sprite.atlasPos.y = getAtlasY(globule.type);
			
			if (globule.type == RessourceType.None)
				sprite.atlasPos.x = 0;
			else
				sprite.atlasPos.x = animStep;
        }
	}
	
	int getAtlasY( RessourceType type )
	{
		switch (type)
		{
		case RessourceType.None:
			return 0;
		case RessourceType.Blue:
			return 1;
		case RessourceType.Green:
			return 2;
		//case RessourceType.Yellow:
		//	return 3;
		case RessourceType.Purple:
			return 4;
		case RessourceType.Heart:
			return 5;
		}
		
		return 7;
	}
}
