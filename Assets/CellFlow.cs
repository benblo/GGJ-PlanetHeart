using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Point
{
    public int X;
    public int Y;

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return p1.X != p2.X || p1.Y != p2.Y;
    }
}

public class CellFlow : MonoBehaviour
{
    public WorldGrid worldGrid;

    public bool isDrawing;
    public Point lastCellPos;

    void Start()
    {
        for (int x = 0; x < worldGrid.width; x++)
        {
            for (int y = 0; y < worldGrid.height; y++)
            {
                Cell cell = worldGrid.getCell(x, y);
                cell.flow = CellFlowDirection.None;

                int mudTypeIndex = worldGrid.getCellTypeIndexByName("Mud");
                int airTypeIndex = worldGrid.getCellTypeIndexByName("Air");
                if (y == 34)
                {
                    cell.type = worldGrid.cellTypes[mudTypeIndex];
                    cell.typeIndex = mudTypeIndex;
                }
                if (y > 34)
                {
                    cell.type = worldGrid.cellTypes[airTypeIndex];
                    cell.typeIndex = airTypeIndex;
                }
            }
        }
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (worldGrid.isInBounds(mousePos))
        {
            Point currentCellPos = new Point() { X = (int)mousePos.x, Y = (int)mousePos.y };

            if (!isDrawing && Input.GetMouseButtonDown(0))
            {
                Cell startCell = worldGrid.getCell(currentCellPos.X, currentCellPos.Y);
                if (startCell.type.isArtere || startCell.flow != CellFlowDirection.None)
                {
                    //Start digging
                    isDrawing = true;
                    lastCellPos = currentCellPos;
                }
            }
            else if (isDrawing && Input.GetMouseButtonUp(0))
            {
                //End digging
                isDrawing = false;
            }
            else if (isDrawing)
            {
                //Continue digging
                if (currentCellPos != lastCellPos)
                {
                    Cell lastCell = worldGrid.getCell(lastCellPos.X, lastCellPos.Y);
                    Cell cell = worldGrid.getCell(currentCellPos.X, currentCellPos.Y);

                    Debug.Log("Try Digging");
                    Debug.Log("cell.flow: " + cell.flow);
                    Debug.Log("cell.type.canDig: " + cell.type.canDig);

                    //Dig one cell
                    if ((cell.type.canDig || cell.type.isArtere ||  cell.flow != CellFlowDirection.None) && UpdateFlow(lastCellPos, currentCellPos))
                        lastCellPos = currentCellPos;

                    if (cell.flow != CellFlowDirection.None)
                        isDrawing = false;
                }
            }
        }
    }

    /// <summary>
    /// Dig a cell from another one
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private bool UpdateFlow(Point from, Point to)
    {
        int bloodTypeIndex = worldGrid.getCellTypeIndexByName("Blood");

        Cell fromCell = worldGrid.getCell(from.X, from.Y);
        Cell toCell = worldGrid.getCell(to.X, to.Y);

        if (from.X == to.X && lastCellPos.Y == to.Y - 1 && !EnumHelper.Has(toCell.flow, CellFlowDirection.Down))
        {
            toCell.typeIndex = bloodTypeIndex;
            toCell.type = worldGrid.cellTypes[bloodTypeIndex];
            fromCell.flow = fromCell.flow | CellFlowDirection.Up;
            return true;
        }
        else if (from.X == to.X && from.Y == to.Y + 1 && !EnumHelper.Has(toCell.flow, CellFlowDirection.Up))
        {
            toCell.typeIndex = bloodTypeIndex;
            toCell.type = worldGrid.cellTypes[bloodTypeIndex];
            fromCell.flow = fromCell.flow | CellFlowDirection.Down;
            return true;
        }
        else if (from.X == to.X + 1 && from.Y == to.Y && !EnumHelper.Has(toCell.flow, CellFlowDirection.Right))
        {
            toCell.typeIndex = bloodTypeIndex;
            toCell.type = worldGrid.cellTypes[bloodTypeIndex];
            fromCell.flow = fromCell.flow | CellFlowDirection.Left;
            return true;
        }
        else if (from.X == to.X - 1 && from.Y == to.Y && !EnumHelper.Has(toCell.flow, CellFlowDirection.Left))
        {
            toCell.typeIndex = bloodTypeIndex;
            toCell.type = worldGrid.cellTypes[bloodTypeIndex];
            fromCell.flow = fromCell.flow | CellFlowDirection.Right;
            return true;
        }
        return false;
    }
}
