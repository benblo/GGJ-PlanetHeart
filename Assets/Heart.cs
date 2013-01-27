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
    }

}
