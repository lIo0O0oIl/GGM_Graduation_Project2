using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject tile;                        
    [SerializeField]
    private Transform boardTrm;                        

    private List<Tile> tileList;                         

    private int puzzleSize = 3;      
    private float neighborTileDistance = 182;              

    public Vector3 EmptyTilePosition { set; get; }        
    public int Playtime { private set; get; } = 0;     
    public int MoveCount { private set; get; } = 0; 

    private IEnumerator Start()
    {
        tileList = new List<Tile>();

        SpawnTiles();

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(boardTrm.GetComponent<RectTransform>());

        yield return new WaitForEndOfFrame();

        tileList.ForEach(x => x.SetPosition());

        StartCoroutine(Suffle());
    }

    private void SpawnTiles()
    {
        for (int y = 0; y < puzzleSize; ++y)
        {
            for (int x = 0; x < puzzleSize; ++x)
            {
                GameObject clone = Instantiate(this.tile, boardTrm);
                Tile tile = clone.GetComponent<Tile>();

                tile.Setup(this, puzzleSize * puzzleSize, y * puzzleSize + x + 1);

                tileList.Add(tile);
            }
        }
    }

    private IEnumerator Suffle()
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 0.1f;

            int index = UnityEngine.Random.Range(0, puzzleSize * puzzleSize);
            tileList[index].transform.SetAsLastSibling();

            yield return null;
        }
        EmptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().localPosition;
    }

    public void IsMoveTile(Tile tile)
    {
        if (Vector3.Distance(EmptyTilePosition, tile.GetComponent<RectTransform>().localPosition) == neighborTileDistance)
        {
            Vector3 goalPosition = EmptyTilePosition;

            EmptyTilePosition = tile.GetComponent<RectTransform>().localPosition;

            tile.OnMoveTo(goalPosition);

            MoveCount++;
        }
    }
}
