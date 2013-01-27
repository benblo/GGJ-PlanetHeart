using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tree
{
    public Vector2 position;
    public Cell cell;
    WorldGrid worldGrid;
    RessourceConsumer ressourceConsumer;

    public int ressourcesForGrowth = 3;
    public int currentRessources;
    public int growthLevel = 0;

    public int maxLevel = 6;

    public Tree(WorldGrid _world, Cell _cell, Vector2 _position)
    {
        position = _position;
        worldGrid = _world;
        cell = _cell;

        ressourceConsumer = new RessourceConsumer();
        ressourceConsumer.createTypes = RessourceType.None;
        ressourceConsumer.consumTypes = RessourceType.Blue | RessourceType.Green;
        ressourceConsumer.OnRessourceConsumed += new RessourceConsumer.RessourceEvent(ressourceConsumer_OnRessourceConsumed);

        cell.ressourceConsumer = ressourceConsumer;

        worldGrid.trees.Add(this);
    }

    bool ressourceConsumer_OnRessourceConsumed(RessourceType type)
    {
        if (type == RessourceType.Green)
        {
            int probaConsumeGreen = 4;
            if (Random.Range(0, probaConsumeGreen) != 0)
            {
                return false;
            }
            return true;
        }
        else if (type == RessourceType.Blue)
        {
            int probaConsumeBlue = 3;
            if (Random.Range(0, probaConsumeBlue) != 0)
            {
                return false;
            }

            if (growthLevel == maxLevel)
                return false;

            currentRessources++;

            return true;
        }
        return false;
    }

    public void Update()
    {
        int ressourceForLevel = ressourcesForGrowth + 2 * growthLevel;
        if (currentRessources >= ressourceForLevel)
        {
            currentRessources -= ressourceForLevel;
            growthLevel++;

            if (growthLevel == maxLevel)
                ressourceConsumer.consumTypes = RessourceType.None;
        }
    }
}
