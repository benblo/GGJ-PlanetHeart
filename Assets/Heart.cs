using UnityEngine;
using System.Collections;

public class Heart
{
    WorldGrid worldGrid;
    RessourceConsumer ressourceConsumer;

    public int heartConsumed;

    public int heartForLevel = 15;
    public int heartLevel;

    public float globuleSpawnMinDelay = 3;
    public float globuleSpawnMaxDelay = 7;
    public float globuleSpawnNextTime;
    public int globuleToSpawn;

    public int maxGlobuleCountByLevel = 10;

    public Heart(WorldGrid _world)
    {
        worldGrid = _world;

        heartLevel = 0;

        globuleToSpawn = 5;
        globuleSpawnNextTime = Time.time + Random.Range(globuleSpawnMinDelay, globuleSpawnMaxDelay);

        SetupLevel1();
    }

    bool ressourceConsumer_OnRessourceConsumed(RessourceType type)
    {
        heartConsumed++;
        return true;
    }

    public void Update()
    {
        if (heartConsumed > heartForLevel)
        {
            heartConsumed = 0;
            heartLevel++;

            if (heartLevel == 1)
            {
                SetupLevel2();
            }
        }

        if (globuleToSpawn > 0 && Time.time > globuleSpawnNextTime && worldGrid.globules.Count < maxGlobuleCountByLevel * (heartLevel + 1))
        {
            if (heartLevel == 0)
            {
                globuleToSpawn--;
                SpawnGlobule(new Vector2(21, 32));
            }
            else if (heartLevel == 1)
            {
                globuleToSpawn--;
                SpawnGlobule(new Vector2(21, 30));
            }

            globuleSpawnNextTime = Time.time + Random.Range(globuleSpawnMinDelay, globuleSpawnMaxDelay);
        }
    }

    public void SetupLevel1()
    {
        ressourceConsumer = new RessourceConsumer();
        ressourceConsumer.createTypes = RessourceType.None;
        ressourceConsumer.consumTypes = RessourceType.Heart;
        ressourceConsumer.OnRessourceConsumed += new RessourceConsumer.RessourceEvent(ressourceConsumer_OnRessourceConsumed);
        Cell consumerCell = worldGrid.getCell(21, 33);

        consumerCell.ressourceConsumer = ressourceConsumer;

        Cell cell;
        cell = worldGrid.getCell(20, 34);
        cell.flow = CellFlowDirection.Down;
        cell.isArtere = true;

        cell = worldGrid.getCell(20, 33);
        cell.flow = CellFlowDirection.Down;
        cell.isArtere = true;

        cell = worldGrid.getCell(20, 32);
        cell.flow = CellFlowDirection.Right;
        cell.isArtere = true;

        cell = worldGrid.getCell(21, 32);
        cell.flow = CellFlowDirection.Right;
        cell.isArtere = true;

        cell = worldGrid.getCell(22, 32);
        cell.flow = CellFlowDirection.Up;
        cell.isArtere = true;

        cell = worldGrid.getCell(22, 33);
        cell.flow = CellFlowDirection.Up;
        cell.isArtere = true;

        cell = worldGrid.getCell(22, 34);
        cell.flow = CellFlowDirection.Up;
        cell.isArtere = true;
		
		worldGrid.artereLayer.Reset();
		worldGrid.musicControl.SetupLevel1();
    }

    public void SetupLevel2()
    {
        ressourceConsumer = new RessourceConsumer();
        ressourceConsumer.createTypes = RessourceType.None;
        ressourceConsumer.consumTypes = RessourceType.Heart;
        ressourceConsumer.OnRessourceConsumed += new RessourceConsumer.RessourceEvent(ressourceConsumer_OnRessourceConsumed);
        Cell consumerCell = worldGrid.getCell(21, 29);

        consumerCell.ressourceConsumer = ressourceConsumer;

        Cell cell;
        cell = worldGrid.getCell(20, 28);
        cell.flow = CellFlowDirection.Up;
        cell.isArtere = true;

        cell = worldGrid.getCell(20, 29);
        cell.flow = CellFlowDirection.Up;
        cell.isArtere = true;

        cell = worldGrid.getCell(20, 30);
        cell.flow = CellFlowDirection.Right;
        cell.isArtere = true;

        cell = worldGrid.getCell(21, 30);
        cell.flow = CellFlowDirection.Right;
        cell.isArtere = true;

        cell = worldGrid.getCell(22, 30);
        cell.flow = CellFlowDirection.Down;
        cell.isArtere = true;

        cell = worldGrid.getCell(22, 29);
        cell.flow = CellFlowDirection.Down;
        cell.isArtere = true;

        cell = worldGrid.getCell(22, 28);
        cell.flow = CellFlowDirection.Down;
        cell.isArtere = true;

        globuleToSpawn = 10;
		
		worldGrid.artereLayer.Reset();
		worldGrid.musicControl.SetupLevel2();
    }

    public void SpawnGlobule(Vector2 pos)
    {
        var globule = new Globule(worldGrid, pos);
        globule.type = RessourceType.None;
        globule.movement.speed = UnityEngine.Random.Range(worldGrid.particleSpeedMin, worldGrid.particleSpeedMax);

        worldGrid.globules.Add(globule);
    }
}