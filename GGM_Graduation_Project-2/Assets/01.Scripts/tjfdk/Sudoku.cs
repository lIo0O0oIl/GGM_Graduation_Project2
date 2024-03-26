using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sudoku : MonoBehaviour
{
    static public Sudoku Instance;

    public SudokuTile[,] tiles = new SudokuTile[9, 9];
    [SerializeField] private int suffleAmount = 0;
    [SerializeField] private int hiddingAmount = 0;
    [SerializeField] private int puzzleAmount;

    [SerializeField] private Folder lockFolder;

    private void Awake()
    {
        Instance = this;
        TileSet();
    }

    private void Start()
    {
    }

    private void TileSet()
    {
        int x = 0, y = 0;
        int nx = 0, ny = 0;

        for (int i = 0; i < 9; ++i)
        {
            y = ny;
            for (int j = 0; j < 9; ++j)
            {
                tiles[i, j] = transform.GetChild(x).GetChild(y).GetComponent<SudokuTile>();

                y++;
                if ((j + 1) % 3 == 0)
                {
                    x++;
                    y = ny;
                }
            }

            ny += 3;
            if (ny == 9)
                ny = 0;

            if ((i + 1) % 3 == 0)
                nx = i + 1;

            if (x % 3 == 0)
                x = nx;
        }
    }

    public void Setting()
    {
        Debug.Log("½ºµµÄí ÁøÀÔ");

        puzzleAmount = hiddingAmount;

        Clear();
        Init();
    }

    private void Clear()
    {
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                tiles[i, j].ClearSetting();
            }
        }
    }

    public void down()
    {
        puzzleAmount--;

        if (puzzleAmount == 0)
        {
            Debug.Log("R°× ³¡");
            FileManager.instance.PuzzleLockBackClick();     // ÆÛÁñÆÇ³Ú ²¨ÁÖ±â
            lockFolder.PuzzleClear();       // ÆÛÁñ Å¬¸®¾îµÊ
        }
    }

    private void Init()
    {
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                tiles[i, j].Init((i * 3 + i / 3 + j) % 9 + 1);
            }
        }

        Suffle(suffleAmount);
        Hide(hiddingAmount);
    }

    private void Suffle(int _suffleAmount)
    {
        for (int i = 0; i < _suffleAmount; ++i)
        {
            int value1 = Random.Range(1, 10);
            int value2 = Random.Range(1, 10);

            if (value1 == value2)
                continue;

            Mix(value1, value2);
        }
    }

    private void Mix(int _value1, int _value2)
    {
        int x1 = 0, x2 = 0;
        int y1 = 0, y2 = 0;

        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (tiles[i, j].GetCorrect() == _value1)
                {
                    x1 = i;
                    y1 = j;
                }
                if (tiles[i, j].GetCorrect() == _value2)
                {
                    x2 = i;
                    y2 = j;
                }
            }
            tiles[x1, y1].Init(_value2);
            tiles[x2, y2].Init(_value1);
        }
    }

    private void Hide(int _hideAmount)  
    {
        for (int i = 0; i < _hideAmount; ++i)
        {
            int x = 0, y = 0;

            do
            {
                x = Random.Range(0, 9);
                y = Random.Range(0, 9);
            }   while (tiles[x, y].isHide);

            Debug.Log(x + " " + y);
            tiles[x, y].Hide();
        }
    }
}
