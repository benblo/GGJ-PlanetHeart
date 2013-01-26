using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Flags]
public enum CellFlowDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
}

[System.Serializable]
public class Cell
{
	public int typeIndex;
	
	internal CellType type;
	internal int atlasX;
	
	internal float amount = 1;
	internal float lastSpawnTime;

    internal bool isArtere = false;
    internal CellFlowDirection flow;
	
	internal Color color
	{
		//get { Color c = type.color; c.a = amount; return c; }
		get { return Color.Lerp(Color.white, type.color, amount); }
	}
}

[System.Serializable]
public class CellType
{
	public string name;
	public Color color = Color.white;
	public int atlasY;

	public bool isResource;
    public RessourceType ressourceType;

	public bool canDig;
	public int digLevel;
}

[ExecuteInEditMode]
public class WorldGrid : MonoBehaviour
{
	public int width = 8;
    public int height = 4;

    public float particleSpeedMin = 0.9f;
    public float particleSpeedMax = 1.1f;

    public List<Cell> grid;
    public List<Globule> globules = new List<Globule>();

	public bool isInBounds( int x, int y )
	{
		return x >= 0 && x < width &&
			   y >= 0 && y < height;
	}
	
	public bool isInBounds( Vector2 pos )
	{
		return isInBounds((int)pos.x, (int)pos.y);
	}

    public Cell getCellNeighbour(int x, int y, CellFlowDirection direction)
    {
        Vector3 dirVec = FlowDirectionToVector(direction);
        if (isInBounds(new Vector2(x + (int)dirVec.x, y + (int)dirVec.y)))
            return getCell(x + (int)dirVec.x, y + (int)dirVec.y);
        return null;
    }

	public Cell getCell( int x, int y )
	{
		return grid[x + y * width];
	}
	
	public Cell getCell( Vector2 pos )
	{
		return getCell((int)pos.x, (int)pos.y);
	}

    public IEnumerable<Cell> getNeighbourCell(int x, int y)
    {
        if (isInBounds(x, y + 1))
            yield return getCell(x, y + 1);

        if (isInBounds(x, y - 1))
            yield return getCell(x, y - 1);

        if (isInBounds(x + 1, y))
            yield return getCell(x + 1, y);

        if (isInBounds(x - 1, y))
            yield return getCell(x - 1, y);
    }
	
	Vector2 getMousePos()
	{
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

    public int getCellTypeIndexByName(String name)
    {
        for (int i = 0; i< cellTypes.Length; i++)
        {
            if (cellTypes[i].name == name)
                return i;
        }
        return -1;
    }	
	
	public CellType[] cellTypes;
	
	void Awake()
	{
		initGrid();
		
		initMesh(mainMesh, ref mainUVs);
		initMesh(frontMesh, ref frontUVs);
	}
	
	void initGrid()
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
			c.atlasX = UnityEngine.Random.Range(0, atlasVarCount);
		}
	}
	
	[ContextMenu("Fill grid with current brush")]
	void fillGridCurBrush()
	{
		int cellCount = height * width;
		for (int i = 0; i < cellCount; i++)
		{
			Cell c = grid[i];
			c.typeIndex = brushType;
		}
		
		initGrid();
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
	public int borderAtlasY;
	
	void updateMainUVs()
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
		}
		
		mainMesh.uv = mainUVs;
	}
	
	void updateFrontUVs()
	{
		float atlasStepX = 1f / (float)atlasWidth;
		float atlasStepY = 1f / (float)atlasHeight;
		
		int[] triangles = new int[width * height * 6];
		int quadCount = 0;

		for (int x = 1; x < width - 1; x++)
		{
			for (int y = 1; y < height - 1; y++)
			{
				int ic = x + y * width;
				Cell cell = grid[ic];
				
				if ( cell.type.isResource )
				{
					continue;
				}
				
				int sum = 0;
				
				if ( getCell(x - 1, y + 1).type.isResource )
					sum += 1;
				if ( getCell(x, y + 1).type.isResource )
					sum += 2;
				if ( getCell(x + 1, y + 1).type.isResource )
					sum += 4;
				if ( getCell(x - 1, y).type.isResource )
					sum += 8;
				if ( getCell(x + 1, y).type.isResource )
					sum += 16;
				if ( getCell(x - 1, y - 1).type.isResource )
					sum += 32;
				if ( getCell(x, y - 1).type.isResource )
					sum += 64;
				if ( getCell(x + 1, y - 1).type.isResource )
					sum += 128;
				
				if (sum == 0)
				{
					continue;
				}
				
				int iv = ic * 4;
				int it = quadCount * 6;
				
				triangles[it + 0] = iv + 0;
				triangles[it + 1] = iv + 1;
				triangles[it + 2] = iv + 2;
				triangles[it + 3] = iv + 2;
				triangles[it + 4] = iv + 3;
				triangles[it + 5] = iv + 0;
				
				quadCount++;
				
				//Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y + 1));
				//Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y));
				
				float ax = getBorderIndex(sum);
				float ay = borderAtlasY;
				frontUVs[iv + 0] = new Vector2(atlasStepX * ax		, atlasStepY * ay);
				frontUVs[iv + 1] = new Vector2(atlasStepX * ax		, atlasStepY * (ay + 1));
				frontUVs[iv + 2] = new Vector2(atlasStepX * (ax + 1), atlasStepY * (ay + 1));
				frontUVs[iv + 3] = new Vector2(atlasStepX * (ax + 1), atlasStepY * ay);
			}
		}
		
		int[] curTriangles = new int[quadCount * 6];
		System.Array.Copy(triangles, curTriangles, quadCount * 6);
		frontMesh.triangles = curTriangles;
		frontMesh.uv = frontUVs;
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
			Debug.Log(sum);
			return 13; // error
		}
	}
	
	void Update()
	{
		if (editMode)
			updateEditor();
		
		updateMainUVs();
		updateFrontUVs();
		
		updateGlobules();
	}
	
	void updateGlobules()
	{
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = getMousePos();
            if (isInBounds(mousePos))
            {
                Cell cell = getCell(mousePos);
                if (cell.isArtere)
                {
                    Globule globule = new Globule(this, new Vector2((int)mousePos.x, (int)mousePos.y));
                    globule.type = RessourceType.None;
                    globule.movement.speed = UnityEngine.Random.Range(particleSpeedMin, particleSpeedMax);

                    globules.Add(globule);
                }
            }
        }

        foreach (var globule in globules)
        {
            globule.UpdateMovement();
        }
	}
	

	// EDITOR CRAP
	
	public bool editMode;
	[BrushType]
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

    public bool drawGarry;
	public bool drawGizmos;
	public bool drawGrid;
	public bool drawCells;
	public bool debugCurCell;
	public string debugText;
	
	void OnDrawGizmos()
	{
        if (drawGarry)
        {
            DrawDebugGarry();
            return;
        }

		if (!drawGizmos)
			return;

        foreach (var glob in globules)
        {
            glob.DrawGizmo();
        }
		
		

		
		if (drawGrid)
		{
			for (int x = 0; x < width + 1; x++)
			{
				Debug.DrawLine(new Vector2(x, 0), new Vector2(x, height));
			}
			
			for (int y = 0; y < height + 1; y++)
			{
				Debug.DrawLine(new Vector2(0, y), new Vector2(width, y));
			}
		}
		
		
		if (drawCells)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Cell cell = getCell(x, y);
	
					Gizmos.color = cell.color;
					Gizmos.DrawCube(new Vector2(x, y) + new Vector2(0.5f, 0.5f), Vector2.one);
				}
			}
		}
		
		
		if (debugCurCell)
		{
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
				
				
				
				Cell cell = getCell(x, y);
				int ic = x + y * width;
				
				debugText = x + "," + y + " (" + ic + "): ";
				if ( cell.type.isResource )
				{
					debugText += "resource";
				}
				else
				{
					int sum = 0;
					
					if ( getCell(x - 1, y + 1).type.isResource )
						sum += 1;
					if ( getCell(x, y + 1).type.isResource )
						sum += 2;
					if ( getCell(x + 1, y + 1).type.isResource )
						sum += 4;
					if ( getCell(x - 1, y).type.isResource )
						sum += 8;
					if ( getCell(x + 1, y).type.isResource )
						sum += 16;
					if ( getCell(x - 1, y - 1).type.isResource )
						sum += 32;
					if ( getCell(x, y - 1).type.isResource )
						sum += 64;
					if ( getCell(x + 1, y - 1).type.isResource )
						sum += 128;
					
					debugText += sum;
				}
			}
			else
			{
				debugText = "";
			}
		}
	}

    private void DrawDebugGarry()
    {
        foreach (var glob in globules)
        {
            glob.DrawGizmo();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = getCell(x, y);

                Gizmos.color = cell.color;
                DrawFlowArrows(cell, x, y);
            }
        }
    }

    public void DrawFlowArrows(Cell cell, int x, int y)
    {
        Vector3 cellCenter = new Vector2(x, y) + new Vector2(0.5f, 0.5f);
        cellCenter.z = -2;
        Gizmos.color = Color.red;
        if ((cell.flow & CellFlowDirection.Up) == CellFlowDirection.Up)
        {
            Gizmos.DrawCube(cellCenter + FlowDirectionToVector(CellFlowDirection.Up) * 0.35f, Vector3.one * 0.1f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Up) * 0.4f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Down) * 0.2f);
        }
        if ((cell.flow & CellFlowDirection.Down) == CellFlowDirection.Down)
        {
            Gizmos.DrawCube(cellCenter + FlowDirectionToVector(CellFlowDirection.Down) * 0.35f, Vector3.one * 0.1f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Down) * 0.4f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Up) * 0.2f);
        }
        if ((cell.flow & CellFlowDirection.Left) == CellFlowDirection.Left)
        {
            Gizmos.DrawCube(cellCenter + FlowDirectionToVector(CellFlowDirection.Left) * 0.35f, Vector3.one * 0.1f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Left) * 0.4f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Right) * 0.2f);
        }
        if ((cell.flow & CellFlowDirection.Right) == CellFlowDirection.Right)
        {
            Gizmos.DrawCube(cellCenter + FlowDirectionToVector(CellFlowDirection.Right) * 0.35f, Vector3.one * 0.1f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Right) * 0.4f);
            Gizmos.DrawRay(cellCenter, FlowDirectionToVector(CellFlowDirection.Left) * 0.2f);
        }
    }

    public Vector3 FlowDirectionToVector(CellFlowDirection direction)
    {
        if (direction == CellFlowDirection.Up)
            return new Vector3(0, 1);
        if (direction == CellFlowDirection.Down)
            return new Vector3(0, -1);
        if (direction == CellFlowDirection.Left)
            return new Vector3(-1, 0);
        if (direction == CellFlowDirection.Right)
            return new Vector3(1, 0);

        return Vector3.zero;
    }
}
