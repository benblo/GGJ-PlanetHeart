using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Cell
{
	public int typeIndex;
	internal CellType type;
	public int atlasX;
	
	public bool isHalo;
	public float amount = 1;
	public Color color
	{
		//get { Color c = type.color; c.a = amount; return c; }
		get { return Color.Lerp(Color.white, type.color, amount); }
	}
	
	public float lastSpawnTime;
}

[System.Serializable]
public class CellType
{
	public string name;
	public Color color;
	public Vector2 uv0, uv1, uv2, uv3;
	public int atlasY;
	
	public bool isResource;
	public float spawnRate = 1;
	public float spawnAmount = 0.1f;
	
	public bool canDig;
	public bool canDiffuse;
	public bool canGrow;
	public float growSpeed = 0.1f;
	public bool isEmpty;
}

[ExecuteInEditMode]
public class WorldGrid : MonoBehaviour
{
	public int width = 8;
	public int height = 4;
	
	public List<Cell> grid;

	public bool isInBounds( int x, int y )
	{
		return x >= 0 && x < width &&
			   y >= 0 && y < height;
	}
	
	public bool isInBounds( Vector2 pos )
	{
		return isInBounds((int)pos.x, (int)pos.y);
	}
	
	Cell getCell( int x, int y )
	{
		return grid[x + y * width];
	}
	
	Cell getCell( Vector2 pos )
	{
		return getCell((int)pos.x, (int)pos.y);
	}
	
	Vector2 getMousePos()
	{
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	
	public CellType[] cellTypes;
	
	void Awake()
	{
		int cellCount = height * width;
		if (grid.Count != cellCount)
		{
			grid = new List<Cell>(cellCount);
			for (int i = 0; i < cellCount; i++)
				grid.Add(new Cell());
		}
		
		for (int i = 0; i < cellCount; i++)
		{
			Cell c = grid[i];
			c.type = cellTypes[c.typeIndex];
			c.atlasX = Random.Range(0, atlasVarCount);
		}
		
		initMesh(mainMesh, ref mainUVs);
		initMesh(frontMesh, ref frontUVs);
	}
	

	public Mesh mainMesh, frontMesh;
	Vector2[] mainUVs, frontUVs;

	void initMesh( Mesh mesh, ref Vector2[] uvs )
	{
		int cellCount = height * width;
		
		/*if (mesh.vertices.Length == cellCount * 4)
		{
			return;
		}*/
		
		
		Vector3[] vertices = new Vector3[cellCount * 4];
		int[] triangles = new int[cellCount * 2 * 3];
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int ic = x + y * width;
				int iv = ic * 4;
				
				vertices[iv + 0] = new Vector3(x, y);
				vertices[iv + 1] = new Vector3(x, y + 1);
				vertices[iv + 2] = new Vector3(x + 1, y + 1);
				vertices[iv + 3] = new Vector3(x + 1, y);
			}
		}

		for (int c = 0; c < cellCount; c++)
		{
			int it = c * 6;
			int iv = c * 4;
			
			triangles[it + 0] = iv + 0;
			triangles[it + 1] = iv + 1;
			triangles[it + 2] = iv + 2;

			triangles[it + 3] = iv + 2;
			triangles[it + 4] = iv + 3;
			triangles[it + 5] = iv + 0;
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		//Debug.Log(mesh.normals.Length + " normals");

		uvs = new Vector2[cellCount * 4];
		mesh.uv = uvs;
	}
	
	public int atlasWidth = 2;
	public int atlasHeight = 2;
	public int atlasVarCount = 2;
	
	void updateUVs()
	{
		float atlasStepX = 1f / (float)atlasWidth;
		float atlasStepY = 1f / (float)atlasHeight;
		
		for (int c = 0; c < grid.Count; c++)
		{
			Cell cell = grid[c];
			
			int iv = c * 4;
			float ax = cell.atlasX;
			float ay = cell.type.atlasY;
			mainUVs[iv + 0] = new Vector2(atlasStepX * ax	   , atlasStepY * ay);
			mainUVs[iv + 1] = new Vector2(atlasStepX * ax	   , atlasStepY * (ay + 1));
			mainUVs[iv + 2] = new Vector2(atlasStepX * (ax + 1), atlasStepY * (ay + 1));
			mainUVs[iv + 3] = new Vector2(atlasStepX * (ax + 1), atlasStepY * ay);

			/*mainUVs[iv + 0] = cellType.uv0;
			mainUVs[iv + 1] = cellType.uv1;
			mainUVs[iv + 2] = cellType.uv2;
			mainUVs[iv + 3] = cellType.uv3;*/
		}
		
		mainMesh.uv = mainUVs;
		
		
		// reset halos
		grid.ForEach(c => c.isHalo = false);
		
		for (int x = 1; x < width-1; x++)
		{
			for (int y = 1; y < height-1; y++)
			{
				int ic = x + y * width;
				Cell cell = grid[ic];
				
				if ( cell.type.isResource )
				{
					Cell neighbor;
					
					neighbor = getCell(x-1, y-1);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					neighbor = getCell(x, y-1);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					neighbor = getCell(x+1, y-1);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					
					neighbor = getCell(x-1, y);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					neighbor = getCell(x+1, y);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					
					neighbor = getCell(x-1, y+1);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					neighbor = getCell(x, y+1);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
					
					neighbor = getCell(x+1, y+1);
					if ( !neighbor.type.isResource )
						neighbor.isHalo = true;
				}
			}
		}
		
		int[] triangles = new int[width * height * 6];
		int haloCount = 0;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int ic = x + y * width;
				Cell cell = grid[ic];
				
				if ( cell.isHalo )
				{
					int iv = ic * 4;
					int it = haloCount * 6;
					
					triangles[it + 0] = iv + 0;
					triangles[it + 1] = iv + 1;
					triangles[it + 2] = iv + 2;
					triangles[it + 3] = iv + 2;
					triangles[it + 4] = iv + 3;
					triangles[it + 5] = iv + 0;
					
					haloCount++;
					
					//Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y + 1));
					//Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y));
				}
			}
		}
		
		int[] curTriangles = new int[haloCount * 6];
		System.Array.Copy(triangles, curTriangles, haloCount * 6);
		frontMesh.triangles = curTriangles;
	}
	
	
	void Update()
	{
		if (editMode)
			updateEditor();
		else
			updateDig();
		
		updateUVs();
		
		updateParticles();
	}
	
	void updateDig0()
	{
		Vector2 mousePos = getMousePos();
		bool isClicked = Input.GetMouseButton(0);

		if ( isInBounds(mousePos) && isClicked )
		{
			Cell cell = getCell(mousePos);
			if ( cell.type.canDig )
			{
				cell.typeIndex = 0;
				cell.type = cellTypes[cell.typeIndex];
			}
		}
		
		float dt = Time.deltaTime;
		for (int x = 1; x < width - 1; x++)
		{
			for (int y = 0; y < height - 1; y++)
			{
				int ic = x + y * width;
				Cell cell = grid[ic];
				
				if ( cell.type.canGrow )
				{
					if ( cell.amount < 1 )
					{
						cell.amount += cell.type.growSpeed * dt;
					}
					else
					{
						diffuse(cell, getCell(x, y + 1));
						diffuse(cell, getCell(x - 1, y));
						diffuse(cell, getCell(x + 1, y));
					}
				}
			}
		}
	}
	
	bool isEmpty( int x, int y )
	{
		return isInBounds(x, y) && getCell(x, y).type.isEmpty;
	}
	
	void updateDig()
	{
		Vector2 mousePos = getMousePos();
		bool isClicked = Input.GetMouseButton(0);

		if ( isInBounds(mousePos) && isClicked )
		{
			Cell cell = getCell(mousePos);
			if ( cell.type.canDig )
			{
				int x = (int)mousePos.x;
				int y = (int)mousePos.y;
				
				bool ok = true;
				
				if ( !isEmpty(x - 1, y) &&
					 !isEmpty(x + 1, y) &&
					 !isEmpty(x, y - 1) && 
					 !isEmpty(x, y + 1) )
				{
					ok = false;
				}
				
				if ( isEmpty(x + 1, y  + 1) )
				{
					if ( isEmpty(x + 1, y) &&
						 isEmpty(x, y + 1) )
						ok = false;
				}
				if ( isEmpty(x - 1, y  - 1) )
				{
					if ( isEmpty(x - 1, y) &&
						 isEmpty(x, y - 1) )
						ok = false;
				}
				if ( isEmpty(x + 1, y  - 1) )
				{
					if ( isEmpty(x + 1, y) &&
						 isEmpty(x, y - 1) )
						ok = false;
				}
				if ( isEmpty(x - 1, y  + 1) )
				{
					if ( isEmpty(x - 1, y) &&
						 isEmpty(x, y + 1) )
						ok = false;
				}
				
				if (ok)
				{
					cell.typeIndex = 0;
					cell.type = cellTypes[cell.typeIndex];
				}
			}
		}
		
		float dt = Time.deltaTime;
		for (int x = 1; x < width - 1; x++)
		{
			for (int y = 0; y < height - 1; y++)
			{
				int ic = x + y * width;
				Cell cell = grid[ic];
				
				if ( cell.type.canGrow )
				{
					if ( cell.amount < 1 )
					{
						cell.amount += cell.type.growSpeed * dt;
					}
					else
					{
						diffuse(cell, getCell(x, y + 1));
						diffuse(cell, getCell(x - 1, y));
						diffuse(cell, getCell(x + 1, y));
					}
				}
			}
		}
	}

	void diffuse( Cell from, Cell to )
	{
		if (to.type.canDiffuse)
		{
			to.typeIndex = from.typeIndex;
			to.type = from.type;
			to.amount = 0;
		}
	}
	
	
	class ResourceParticle
	{
		public Vector2 curCell, nextCell;
		public float cursor;
		public float speed;
	}
	
	public float particleSpeedMin = 0.9f;
	public float particleSpeedMax = 1.1f;
	
	List<ResourceParticle> resourceParticles = new List<ResourceParticle>();
	
	void updateParticles()
	{
		float time = Time.time;
		float dt = Time.deltaTime;
		
		for (int x = 1; x < width - 1; x++)
		{
			for (int y = 0; y < height - 1; y++)
			{
				Cell cell = getCell(x, y);
				
				if ( cell.type.isResource )
				{
					if ( cell.lastSpawnTime < time - cell.type.spawnRate )
					{
						Cell other = getCell(x - 1, y);
						if ( other.type.isEmpty )
						{
							ResourceParticle particle = new ResourceParticle();
							resourceParticles.Add(particle);
							
							particle.curCell = new Vector2(x, y);
							particle.nextCell = new Vector2(x - 1, y);
							particle.speed = Random.Range(particleSpeedMin, particleSpeedMax);
							
							cell.lastSpawnTime = time;
						}
					}
				}
			}
		}
		
		foreach (ResourceParticle particle in resourceParticles)
		{
			particle.cursor += dt * particle.speed;
			
			if (particle.cursor >= 1)
			{
				particle.cursor = 1;
				
				int x = (int)particle.nextCell.x;
				int y = (int)particle.nextCell.y;
				
				List<Vector2> availableCells = new List<Vector2>();
				
				checkAvailable(particle.curCell, new Vector2(x, y + 1), availableCells);
				checkAvailable(particle.curCell, new Vector2(x - 1, y), availableCells);
				checkAvailable(particle.curCell, new Vector2(x + 1, y), availableCells);
				checkAvailable(particle.curCell, new Vector2(x, y - 1), availableCells);
				
				if (availableCells.Count > 0)
				{
					particle.cursor = 0;
					particle.curCell = particle.nextCell;
					particle.nextCell = availableCells[ Random.Range(0, availableCells.Count) ];
				}
			}
			
			Vector2 pos = Vector2.Lerp(particle.curCell, particle.nextCell, particle.cursor) + new Vector2(0.5f, 0.5f);
			
			Debug.DrawLine(pos + new Vector2(-0.1f, -0.1f), pos + new Vector2(0.1f, 0.1f), Color.blue);
			Debug.DrawLine(pos + new Vector2(-0.1f, 0.1f), pos + new Vector2(0.1f, -0.1f), Color.blue);
			
		}
	}
	
	void checkAvailable( Vector2 curCell, Vector2 nextCell, List<Vector2> availableCells )
	{
		if ( nextCell != curCell &&
			 isInBounds(nextCell) &&
			 getCell(nextCell).type.isEmpty )
		{
			availableCells.Add(nextCell);
		}
	}
	
	
	// EDITOR CRAP
	
	public bool editMode;
	public int brushType;
	
	void updateEditor()
	{
		if (brushType < 0 || brushType >= cellTypes.Length)
			return;
		
		Vector2 mousePos = getMousePos();
		
		if ( isInBounds(mousePos) )
		{
			bool isClicked = Input.GetMouseButton(0);
			if (isClicked)
			{
				Cell cell = getCell(mousePos);
				cell.typeIndex = brushType;
				cell.type = cellTypes[brushType];
			}
		}
	}
	
/*
	Cell lastCellChanged;
	
	void updateEditor()
	{
		if (Input.GetMouseButton(0))
			Debug.Log("btn");
		if (Input.GetMouseButtonDown(0))
			Debug.Log("down");
		if (Input.GetMouseButtonUp(0))
			Debug.Log("up");

		Vector2 mousePos = getMousePos();
		
		if ( isInBounds(mousePos) )
		{
			bool isClicked = Input.GetMouseButton(0);
			if (isClicked)
			{
				Cell cell = getCell(mousePos);
				
				if (cell != lastCellChanged)
				{
					lastCellChanged = cell;
					
					cell.type++;
					if ( cell.type >= cellTypes.Length )
					{
						cell.type = 0;
					}
				}
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			lastCellChanged = null;
		}
	}
*/
	
	public bool drawGizmos;
	
	void OnDrawGizmos()
	{
		if (!drawGizmos)
			return;
		
		
		for (int x = 0; x < width + 1; x++)
		{
			Debug.DrawLine(new Vector2(x, 0), new Vector2(x, height));
		}
		
		for (int y = 0; y < height + 1; y++)
		{
			Debug.DrawLine(new Vector2(0, y), new Vector2(width, y));
		}

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Cell cell = getCell(x, y);

				Gizmos.color = cell.color;
				Gizmos.DrawCube(new Vector2(x, y) + new Vector2(0.5f, 0.5f), Vector2.one);
			}
		}

		
		Vector2 mousePos = getMousePos();
		Debug.DrawLine(mousePos + new Vector2(-0.1f, -0.1f), mousePos + new Vector2(0.1f, 0.1f));
		Debug.DrawLine(mousePos + new Vector2(-0.1f, 0.1f), mousePos + new Vector2(0.1f, -0.1f));
		
		if ( isInBounds(mousePos) )
		{
			int x = (int)mousePos.x;
			int y = (int)mousePos.y;
			
			Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y + 1));
			Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y));

			//bool isClicked = Input.GetMouseButton(0);
			//Gizmos.color = isClicked ? Color.red : Color.green;
			//Gizmos.DrawCube(new Vector2(x, y) + new Vector2(0.5f, 0.5f), Vector2.one);
		}
	}
}
