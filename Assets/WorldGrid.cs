using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Cell
{
	public int type;
}

[System.Serializable]
public class CellType
{
	public string name;
	public Color color;
	public Vector2 uv0, uv1, uv2, uv3;
}

[ExecuteInEditMode]
public class WorldGrid : MonoBehaviour
{
	public int width = 8;
	public int height = 4;
	
	public List<Cell> grid;

	public bool isInBounds( Vector2 pos )
	{
		return pos.x > 0 && pos.x < width &&
			   pos.y > 0 && pos.y < height;
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
		
		initMesh();
	}
	

	public Mesh mesh;
	Vector2[] uvs;

	void initMesh()
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
	
	void updateUVs()
	{
		for (int c = 0; c < grid.Count; c++)
		{
			Cell cell = grid[c];
			CellType cellType = cellTypes[cell.type];
			
			int iv = c * 4;
			uvs[iv + 0] = cellType.uv0;
			uvs[iv + 1] = cellType.uv1;
			uvs[iv + 2] = cellType.uv2;
			uvs[iv + 3] = cellType.uv3;
		}
		
		mesh.uv = uvs;
	}
	
	
	void Update()
	{
		updateEditor();
		
		updateUVs();
	}
	
	
	
	// EDITOR CRAP
	
	public int brushType;
	
	void updateEditor()
	{
		Vector2 mousePos = getMousePos();
		
		if ( isInBounds(mousePos) )
		{
			bool isClicked = Input.GetMouseButton(0);
			if (isClicked)
			{
				Cell cell = getCell(mousePos);
				cell.type = brushType;
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
				CellType type = cellTypes[cell.type];
				Gizmos.color = type.color;
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
