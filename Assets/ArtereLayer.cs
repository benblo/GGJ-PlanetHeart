using UnityEngine;
using System.Collections;

public class ArtereLayer : MonoBehaviour
{
    public SpriteLayer layer;
	public WorldGrid grid;

    void Start()
    {
        Reset();
    }

    void Update()
    {
    }

	public void Reset()
	{
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Cell cell = grid.getCell(x, y);
                if (cell.isArtere && cell.flow != CellFlowDirection.None)
                {
                    int index = 0;

                    Cell nCell;
                    nCell = grid.getCell(x - 1, y);
                    if (EnumHelper.Has(nCell.flow, CellFlowDirection.Right) ||
                        EnumHelper.Has(cell.flow, CellFlowDirection.Left))
                        index += 1;
                    nCell = grid.getCell(x, y + 1);
                    if (EnumHelper.Has(nCell.flow, CellFlowDirection.Down) ||
                        EnumHelper.Has(cell.flow, CellFlowDirection.Up))
                        index += 2;
                    nCell = grid.getCell(x + 1, y);
                    if (EnumHelper.Has(nCell.flow, CellFlowDirection.Left) ||
                        EnumHelper.Has(cell.flow, CellFlowDirection.Right))
                        index += 4;
                    nCell = grid.getCell(x, y - 1);
                    if (EnumHelper.Has(nCell.flow, CellFlowDirection.Up) ||
                        EnumHelper.Has(cell.flow, CellFlowDirection.Down))
                        index += 8;

                    Sprite s = new Sprite();
                    s.atlasPos.x = index;
                    s.atlasPos.y = 1;
                    s.pos = new Vector2(x, y) + new Vector2(0.5f, 0.5f);
                    s.size = new Vector2(1, 1);

                    Debug.Log("Update Aretere Layer: Add sprite");
                    layer.sprites.Add(s);
                }
            }
        }
        layer.buildMesh();
	}
}
