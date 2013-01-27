using UnityEngine;
using System.Collections;

public class LifeLayer : MonoBehaviour
{
	public WorldGrid grid;
	public SpriteLayer layer;
	public Vector2 grassSize = new Vector2(0.5f, 0.5f);
	public Vector2 treeSize = new Vector2(0.5f, 0.5f);
	public Vector2 animalSize = new Vector2(0.5f, 0.5f);
	
	void Update()
	{
		/*foreach (Globule g in grid.globules)
		{
			Sprite s = new Sprite();
			layer.sprites.Add(s);
			
			s.pos = g.movement.Position;
			s.size = size;
		}*/
	}
}
