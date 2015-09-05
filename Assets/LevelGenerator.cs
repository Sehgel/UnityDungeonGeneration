using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    public int mapSize = 100;

    public Vector2[] map;
    public GameObject groundPrefab;
    public GameObject surroundingsPrefab;

	void Start () {

        map = new Vector2[mapSize];

        map[0] = new Vector2();
        
        GameObject start =  ((GameObject)Instantiate(groundPrefab, Vector2.zero, Quaternion.identity));
        start.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(run());
	}
    void GenerateSorroundings()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if(!IsPositionTaken(map[i] + new Vector2(0,1)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(0, 1), Quaternion.identity);
            if (!IsPositionTaken(map[i] + new Vector2(0, -1)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(0, -1), Quaternion.identity);
            if (!IsPositionTaken(map[i] + new Vector2(1, 0)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(1, 0), Quaternion.identity);
            if (!IsPositionTaken(map[i] + new Vector2(-1, 0)))
                Instantiate(surroundingsPrefab, map[i] + new Vector2(-1, 0), Quaternion.identity);
        }
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
            yield return new WaitForSeconds(0.01f);
        }
        GameObject end = ((GameObject)Instantiate(groundPrefab, map[counter-1], Quaternion.identity));
        end.GetComponent<SpriteRenderer>().color = Color.red;
        GenerateSorroundings();
        Debug.Log("Done!");
    }

    private Vector2 GetRandomAdjacentTile(Vector2 _currentPosition)
    {
        int i = 0;
        int multiplier = 2;
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
    private Vector2 GetRandomDirection(Vector2 _currentPosition)
    {
        Vector2 randomDirection = _currentPosition;
        //Up?
        if (Random.Range(0, 2) > 0)
            randomDirection.y += (Random.Range(0, 2) > 0) ? 1 : -1 ;
        else//Then Down
            randomDirection.x += (Random.Range(0, 2) > 0) ? 1 : -1 ;

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
