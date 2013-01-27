using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sprite
{
	public Vector2 pos;
	public Vector2 size;
	public Vector2 atlasPos;
	public Color color = Color.white;
}

public class SpriteLayer : MonoBehaviour
{
	public Mesh mesh;
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;
	Color[] colors;
	
	public int atlasWidth = 2;
	public int atlasHeight = 2;
	float atlasStepX;
	float atlasStepY;
	
	public bool autoUpdate = true;
	internal List<Sprite> sprites = new List<Sprite>();

	void Awake()
	{
		atlasStepX = 1f / atlasWidth;
		atlasStepY = 1f / atlasHeight;
	}
	
	void Update()
	{
		if (autoUpdate)
		{
			buildMesh();
			sprites.Clear();
		}
	}
	
	public void buildMesh()
	{
		vertices = new Vector3[sprites.Count * 4];
		triangles = new int[sprites.Count * 6];
		uvs = new Vector2[sprites.Count * 4];
		
		for (int i = 0; i < sprites.Count; i++)
		{
			Sprite sprite = sprites[i];
			
			int iv = i * 4;
			vertices[iv + 0] = sprite.pos + new Vector2(-sprite.size.x * 0.5f, -sprite.size.y * 0.5f);
			vertices[iv + 1] = sprite.pos + new Vector2(-sprite.size.x * 0.5f, +sprite.size.y * 0.5f);
			vertices[iv + 2] = sprite.pos + new Vector2(+sprite.size.x * 0.5f, +sprite.size.y * 0.5f);
			vertices[iv + 3] = sprite.pos + new Vector2(+sprite.size.x * 0.5f, -sprite.size.y * 0.5f);
		
			int it = i * 6;
			triangles[it + 0] = iv + 0;
			triangles[it + 1] = iv + 1;
			triangles[it + 2] = iv + 2;
			triangles[it + 3] = iv + 2;
			triangles[it + 4] = iv + 3;
			triangles[it + 5] = iv + 0;

			float ax = sprite.atlasPos.x;
			float ay = sprite.atlasPos.y;
			uvs[iv + 0] = new Vector2(atlasStepX * ax	   , atlasStepY * ay);
			uvs[iv + 1] = new Vector2(atlasStepX * ax	   , atlasStepY * (ay + 1));
			uvs[iv + 2] = new Vector2(atlasStepX * (ax + 1), atlasStepY * (ay + 1));
			uvs[iv + 3] = new Vector2(atlasStepX * (ax + 1), atlasStepY * ay);
		}
		
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
	
	public void applyColors()
	{
		colors = new Color[sprites.Count * 4];
		
		for (int i = 0; i < sprites.Count; i++)
		{
			Sprite sprite = sprites[i];
			
			int iv = i * 4;
			colors[iv + 0] = sprite.color;
			colors[iv + 1] = sprite.color;
			colors[iv + 2] = sprite.color;
			colors[iv + 3] = sprite.color;
		}
		
		mesh.colors = colors;
	}
	
	/*void OnDisable()
	{
		mesh.Clear();
	}*/
}
