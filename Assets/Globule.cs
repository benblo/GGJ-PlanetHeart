using UnityEngine;
using System.Collections;

public enum GlobuleType
{
    None,
    Yellow,
}

public class GlobuleMovement
{
    public Vector2 curCell, nextCell;
    public float cursor;
    public float speed;
    public float jitterPhase;
    public float jitterPeriod;

    public Vector2 Position
    {
        get { return curCell + (nextCell - curCell) * cursor; }
    }
}

public class Globule
{
    public GlobuleType type;
    public GlobuleMovement movement;

    public WorldGrid worldGrid;

    public Globule(WorldGrid grid, Vector2 cell)
    {
        worldGrid = grid;
        movement = new GlobuleMovement();
        movement.cursor = 1;
        movement.speed = 0;
        movement.jitterPhase = Random.Range(0, Mathf.PI);
        movement.jitterPeriod = 1;
        movement.curCell = cell;
        movement.nextCell = cell;
    }

    public void UpdateMovement()
    {
		float dt = Time.deltaTime;
        movement.cursor += dt * movement.speed;
        movement.jitterPhase += dt / movement.jitterPeriod * Mathf.PI;

        if (movement.cursor >= 1)
        {
            movement.cursor = 1;

            int x = (int)movement.nextCell.x;
            int y = (int)movement.nextCell.y;
            Cell nextCell = worldGrid.getCell(x, y);

            //Move in a random direction
            CellFlowDirection[] dirs = new CellFlowDirection[] { };
            int random = UnityEngine.Random.Range(0, 3);
            if (random == 0)
                dirs = new CellFlowDirection[] { CellFlowDirection.Up, CellFlowDirection.Right, CellFlowDirection.Down, CellFlowDirection.Left };
            if (random == 1)
                dirs = new CellFlowDirection[] { CellFlowDirection.Left, CellFlowDirection.Down, CellFlowDirection.Up, CellFlowDirection.Right };
            if (random == 2)
                dirs = new CellFlowDirection[] { CellFlowDirection.Down, CellFlowDirection.Left, CellFlowDirection.Right, CellFlowDirection.Up };
            if (random == 3)
                dirs = new CellFlowDirection[] { CellFlowDirection.Right, CellFlowDirection.Up, CellFlowDirection.Left, CellFlowDirection.Down };

            for (int i = 0; i < 4; i++)
            {
                CellFlowDirection direction = dirs[i];
                if ((nextCell.flow & direction) == direction)
                {
                    Vector3 dirVec = worldGrid.FlowDirectionToVector(direction);
                    movement.cursor = 0;
                    movement.curCell = movement.nextCell;
                    movement.nextCell = new Vector2(x + dirVec.x, y + dirVec.y);

                    OnChangeCell(nextCell, x, y);
                    break;
                }
            }
        }
    }

    public virtual void OnChangeCell(Cell cell, int x, int y)
    {
        var dirs = new CellFlowDirection[] { CellFlowDirection.Right, CellFlowDirection.Up, CellFlowDirection.Left, CellFlowDirection.Down };
        
        bool isRessource = false;
        foreach(var dir in dirs)
        {
            Cell neighbourCell = worldGrid.getCellNeighbour(x, y, dir);
            if(cell.type.isResource)
                isRessource = true;
        }

        if (worldGrid.getCell(x, y + 1).type.isResource)
            isRessource = true;
        if (worldGrid.getCell(x, y - 1).type.isResource)
            isRessource = true;
        if (worldGrid.getCell(x + 1, y).type.isResource)
            isRessource = true;
        if (worldGrid.getCell(x - 1, y).type.isResource)
            isRessource = true;

        if (isRessource && type == GlobuleType.None)
        {
            type = GlobuleType.Yellow;
        }
    }

    public void DrawGizmo()
    {
        float jitterRadius = 0.1f;
        Vector2 jitterOffset = jitterRadius * new Vector2(Mathf.Cos(movement.jitterPhase), Mathf.Sin(movement.jitterPhase));

        float collectorRadius = 0.25f;
        float ressourceRadius = 0.2f;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(jitterOffset + movement.Position + new Vector2(0.5f, 0.5f), collectorRadius);

        if (type == GlobuleType.Yellow)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(jitterOffset + movement.Position + new Vector2(0.5f, 0.5f), ressourceRadius);
        }
    }
}
