using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    public int mapSize = 100;

    private Vector2[] map;
    public GameObject groundPrefab;
    public GameObject surroundingsPrefab;
    public GameObject trapPrefab;
    public GameObject itemPrefab;

    public Vector2 spawnPosition = Vector2.zero;

    [Range(0,100)]
    public float trapsCoeficient = 60;
    public float trapDistanceAwayFromSpawn = 5;

    [Range(0, 100)]
    public float itemsCoeficient = 60;
    public float itemsDistanceAwayFromSpawn = 5;

	void Start () {

        map = new Vector2[mapSize];

        map[0] = spawnPosition;
        
        GameObject start =  ((GameObject)Instantiate(groundPrefab, Vector2.zero, Quaternion.identity));
        start.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(run());
	}

    IEnumerator run()
    {
        int counter = 1;
        while (counter < mapSize)
        {
            Vector2 randomPos = GetRandomAdjacentTile(map[counter-1]);
            if (!IsPositionTaken(randomPos))
            {
                Instantiate(groundPrefab, randomPos, Quaternion.identity);
                map[counter] = randomPos;
            }
            counter++;
            yield return null;
        }
        GameObject end = ((GameObject)Instantiate(groundPrefab, map[counter-1], Quaternion.identity));
        end.GetComponent<SpriteRenderer>().color = Color.red;
        GenerateSorroundings();
        GenerateTraps();
        GenerateItems();
        Debug.Log("Done!");
    }

    void GenerateSorroundings()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (!IsPositionTaken(map[i] + new Vector2(0, 1)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(0, 1), Quaternion.identity);
            if (!IsPositionTaken(map[i] + new Vector2(0, -1)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(0, -1), Quaternion.identity);
            if (!IsPositionTaken(map[i] + new Vector2(1, 0)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(1, 0), Quaternion.identity);
            if (!IsPositionTaken(map[i] + new Vector2(-1, 0)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(-1, 0), Quaternion.identity);
        }
    }

    void GenerateTraps()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (Random.Range(0, 100) < trapsCoeficient && GetDistanceToSpawn(map[i]) > trapDistanceAwayFromSpawn)
            {
                if (GetCrossNeighboursCount(map[i]) == 2 && GetXNeighboursCount(map[i]) == 0 )
                {
                    Instantiate(trapPrefab, map[i], Quaternion.identity);
                }
            }
        }
    }
    //Item Generation is not fully working
    private void GenerateItems()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (Random.Range(0, 100) < itemsCoeficient && GetDistanceToSpawn(map[i]) > itemsDistanceAwayFromSpawn)
            {
                if (GetCrossNeighboursCount(map[i]) == 2 && GetXNeighboursCount(map[i]) == 1    )
                {
                    Instantiate(itemPrefab, map[i], Quaternion.identity);
                }
            }
        }
    }
    private int GetCrossNeighboursCount(Vector2 _mapPosition)
    {
        int counter = 0;

        if(IsPositionTaken(_mapPosition + new Vector2(0,1)))
            counter++;
        if (IsPositionTaken(_mapPosition + new Vector2(0, -1)))
            counter++;
        if (IsPositionTaken(_mapPosition + new Vector2(1, 0)))
            counter++;
        if (IsPositionTaken(_mapPosition + new Vector2(-1, 0)))
            counter++;
        return counter;
    }
    private int GetXNeighboursCount(Vector2 _mapPosition)
    {
        int counter = 0;
        if (IsPositionTaken(_mapPosition + new Vector2(1, 1)))
            counter++;
        if (IsPositionTaken(_mapPosition + new Vector2(1, -1)))
            counter++;
        if (IsPositionTaken(_mapPosition + new Vector2(-1, 1)))
            counter++;
        if (IsPositionTaken(_mapPosition + new Vector2(-1, -1)))
            counter++;
        return counter;
    }

    private Vector2 GetRandomAdjacentTile(Vector2 _currentPosition)
    {
        int i = 0;
        int multiplier = 1;
        Vector2 randomDirection = GetRandomDirection(_currentPosition);
        while (IsPositionTaken(randomDirection))
        {
            randomDirection = GetRandomDirection(_currentPosition);
            if (i > 4)
            {
                do
                {
                    randomDirection = GetFirstAdjacentTileAvailable(_currentPosition, multiplier);
                    multiplier++;
                } while (IsPositionTaken(randomDirection)) ;
                return randomDirection;
            }
            i++;
        }
        return randomDirection;

    }
    private Vector2 GetFirstAdjacentTileAvailable(Vector2 _currentPosition, int multiplier)
    {
        int randomSign = (Random.Range(0, 2) > 0) ? 1 : -1;
        int i = multiplier * randomSign;
        if (!IsPositionTaken(_currentPosition + new Vector2(i, 0)))
            return _currentPosition + new Vector2(i, 0);
        if (!IsPositionTaken(_currentPosition + new Vector2(-i, 0)))
            return _currentPosition + new Vector2(-i, 0);
        if (!IsPositionTaken(_currentPosition + new Vector2(0, -i)))
            return _currentPosition + new Vector2(0,-i);
        else
            return _currentPosition + new Vector2(0, i);
    }
    private float GetDistanceToSpawn(Vector2 _position)
    {
        return (_position - spawnPosition).SqrMagnitude();
    }
    private Vector2 GetRandomDirection(Vector2 _currentPosition)
    {
        Vector2 randomDirection = _currentPosition;
        //Vertical?
        if (Random.Range(0, 2) > 0)
            randomDirection.y += (Random.Range(0, 2) > 0) ? 1 : -1 ;//Positive? then negative
        else//Then Horizontal
            randomDirection.x += (Random.Range(0, 2) > 0) ? 1 : -1;//Positive? then negative
        return randomDirection ;
    }
    private bool IsPositionTaken(Vector2 _position)
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == _position)
                return true;
        }
        return false;
    }
}
