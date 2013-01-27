using UnityEngine;
using System.Collections;

public class Heart
{
    public Vector2 position;
    public Cell cell;
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

    public Heart(WorldGrid _world, Cell _cell, Vector2 _position)
    {
        position = _position;
        worldGrid = _world;
        cell = _cell;

        heartLevel = 0;

        ressourceConsumer = new RessourceConsumer();
        ressourceConsumer.createTypes = RessourceType.None;
        ressourceConsumer.consumTypes = RessourceType.Heart;
        ressourceConsumer.OnRessourceConsumed += new RessourceConsumer.RessourceEvent(ressourceConsumer_OnRessourceConsumed);

        cell.ressourceConsumer = ressourceConsumer;

        globuleToSpawn = 5;
        globuleSpawnNextTime = Time.time + Random.Range(globuleSpawnMinDelay, globuleSpawnMaxDelay);
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
        }

        if (globuleToSpawn > 0 && Time.time > globuleSpawnNextTime && worldGrid.globules.Count < maxGlobuleCountByLevel * (heartLevel + 1))
        {
            SpawnGlobule(position + new Vector2(0, 1));
            globuleToSpawn--;

            globuleSpawnNextTime = Time.time + Random.Range(globuleSpawnMinDelay, globuleSpawnMaxDelay);
        }
    }

    public void SpawnGlobule(Vector2 pos)
    {
        var globule = new Globule(worldGrid, pos);
        globule.type = RessourceType.None;
        globule.movement.speed = UnityEngine.Random.Range(worldGrid.particleSpeedMin, worldGrid.particleSpeedMax);

        worldGrid.globules.Add(globule);
    }
}
