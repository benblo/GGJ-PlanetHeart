using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tree
{
    public Vector2 position;
    public Cell cell;
    WorldGrid worldGrid;
    RessourceConsumer ressourceConsumer;

    public int heartToGiveMax = 3;
    public int heartToGive;

    public int ressourcesForGrowth = 2;
    public int currentRessources;
    public int growthLevel = 0;
    public int maxLevel = 6;
	
	public TreeType type;
	public float animOffset;

    public Tree(WorldGrid _world, Cell _cell, Vector2 _position)
    {
        position = _position;
        worldGrid = _world;
        cell = _cell;

        ressourceConsumer = new RessourceConsumer();
        ressourceConsumer.createTypes = RessourceType.None;
        ressourceConsumer.consumTypes = RessourceType.Blue | RessourceType.Green;
        ressourceConsumer.OnRessourceConsumed += new RessourceConsumer.RessourceEvent(ressourceConsumer_OnRessourceConsumed);
        ressourceConsumer.OnRessourceCreated += new RessourceConsumer.RessourceEvent(ressourceConsumer_OnRessourceCreated);

        cell.ressourceConsumer = ressourceConsumer;
		
		type = _world.lifeLayer.treeTypes[ Random.Range(0, _world.lifeLayer.treeTypes.Length) ];
		maxLevel = type.growthCount - 1;
		animOffset = Random.value;
		
        worldGrid.trees.Add(this);
    }

    bool ressourceConsumer_OnRessourceCreated(RessourceType type)
    {
        heartToGive--;
        if(heartToGive == 0)
            growthLevel--;

        return true;
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
        int ressourceForLevel = ressourcesForGrowth + 1 * growthLevel;
        if (currentRessources >= ressourceForLevel)
        {
            currentRessources -= ressourceForLevel;
            growthLevel++;

            if (growthLevel == maxLevel)
            {
                heartToGive = heartToGiveMax;
            }
        }

        if (growthLevel == maxLevel)
        {
            ressourceConsumer.consumTypes = RessourceType.None;
            ressourceConsumer.createTypes = RessourceType.Heart;
        }
        else
        {
            ressourceConsumer.createTypes = RessourceType.None;
            ressourceConsumer.consumTypes = RessourceType.Blue | RessourceType.Green;
        }
    }
}
