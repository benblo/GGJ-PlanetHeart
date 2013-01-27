using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceLayer : MonoBehaviour
{
	public WorldGrid grid;
	public SpriteLayer layer;
	
	List<Sprite> sparks = new List<Sprite>();
	
	void Start()
	{
		for (int x = 1; x < grid.width - 1; x++)
		{
			for (int y = 1; y < grid.height - 1; y++)
			{
				Cell cell = getCell(x, y);
				
				if ( cell.type.isResource )
				{
					// resource spark
					Sprite sprite = new Sprite();
					layer.sprites.Add(sprite);
					sparks.Add(sprite);
					
					sprite.pos = new Vector2(x + 0.5f, y + 0.5f);
					sprite.size = Vector2.one;
					sprite.atlasPos.x = Random.Range(1, 4);
					sprite.atlasPos.y = cell.type.atlasY;
					
					continue;
				}
				
				
				int sum = 0;
				CellType resource = null;
				
				if ( checkResource(getCell(x - 1, y + 1), ref resource) )
					sum += 1;
				if ( checkResource(getCell(x, y + 1), ref resource) )
					sum += 2;
				if ( checkResource(getCell(x + 1, y + 1), ref resource) )
					sum += 4;
				if ( checkResource(getCell(x - 1, y), ref resource) )
					sum += 8;
				if ( checkResource(getCell(x + 1, y), ref resource) )
					sum += 16;
				if ( checkResource(getCell(x - 1, y - 1), ref resource) )
					sum += 32;
				if ( checkResource(getCell(x, y - 1), ref resource) )
					sum += 64;
				if ( checkResource(getCell(x + 1, y - 1), ref resource) )
					sum += 128;
				
				if (sum > 0)
				{
					// border sprite
					Sprite sprite = new Sprite();
					layer.sprites.Add(sprite);
					
					sprite.pos = new Vector2(x + 0.5f, y + 0.5f);
					sprite.size = Vector2.one;
					sprite.atlasPos.x = getBorderIndex(sum);
					sprite.atlasPos.y = resource.atlasY - 1;
					
					
					// border spark
					sprite = new Sprite();
					layer.sprites.Add(sprite);
					sparks.Add(sprite);
					
					sprite.pos = new Vector2(x + 0.5f, y + 0.5f);
					sprite.size = Vector2.one;
					sprite.atlasPos.x = getBorderIndex(sum);
					sprite.atlasPos.y = resource.atlasY - 2;
				}
			}
		}
		
		layer.buildMesh();
		
		layer.applyColors();
	}
	
	public float perlinFrequency = 1;
	public float sparkRate = 1;
	public float sparkMinAlpha = 0.1f;
	public float sparkMaxAlpha = 1f;
	
	void Update()
	{
		foreach (Sprite spark in sparks)
		{
			float offset = Mathf.PerlinNoise(spark.pos.x * perlinFrequency, spark.pos.y * perlinFrequency);
			float sin = Mathf.Sin(Time.time * sparkRate + offset * 2 * Mathf.PI);
			spark.color.a = Mathf.Lerp(sparkMinAlpha, sparkMaxAlpha, (sin + 1) * 0.5f);
		}

		layer.applyColors();
	}
	
	
	
	
	
	Cell getCell( int x, int y )
	{
		return grid.getCell(x, y);
	}
	
	Cell getCell( Vector2 pos )
	{
		return grid.getCell((int)pos.x, (int)pos.y);
	}
	
	bool checkResource( Cell cell, ref CellType resource )
	{
		if (cell.type.isResource)
		{
			resource = cell.type;
			return true;
		}
		return false;
	}
	
	int getBorderIndex( int sum )
	{
		switch (sum)
		{
		case 7:
		case 3:
		case 6:
			return 0;
		case 224:
		case 96:
		case 192:
			return 1;
		case 41:
		case 9:
		case 40:
			return 2;
		case 148:
		case 20:
		case 144:
			return 3;
		case 1:
			return 4;
		case 244:
		case 208:
		case 240:
		case 212:
			return 5;
		case 4:
			return 6;
		case 233:
		case 104:
		case 105:
		case 232:
			return 7;
		case 32:
			return 8;
		case 151:
		case 22:
		case 23:
		case 150:
			return 9;
		case 128:
			return 10;
		case 47:
		case 11:
		case 15:
		case 43:
			return 11;
		default:
			//Debug.Log(sum);
			return 13; // error
		}
	}
	

}
