using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sudoku : Singleton<Sudoku>
{
    public SudokuTile[,] tiles = new SudokuTile[9, 9];
    [SerializeField] private int suffleAmount = 0;
    [SerializeField] private int hiddingAmount = 0;
    [SerializeField] private int puzzleAmount;

    private void Start()
    {
        // s ver
        //for (int i = 0; i < 9; i++)
        //{
        //    for (int j = 0; j < 9; ++j)
        //    {
        //        Debug.Log(i * 9 + j);
        //        tiles[i, j] = transform.GetChild(i * 9 + j).GetComponent<SudocuTile>();
        //    }
        //}

        //// b ver (group) - success
        //for (int i = 0; i < 9; i++)
        //{
        //    for (int j = 0; j < transform.GetChild(i).childCount; ++j)
        //    {
        //        Debug.Log(j);
        //        tiles[i, j] = transform.GetChild(i).GetChild(j).GetComponent<SudocuTile>();
        //    }
        //}

        //// b ver (group) - success
        //for (int a = 0; a < 9; a++)
        //{
        //    for (int b = 0; b < 9; b++)
        //    {
        //        for (int l = 0; l < 9; l += 3) // l i = 세로
        //        {
        //            for (int i = 0; i < 3; ++i)
        //            {
        //                for (int j = 0; j < 9; j += 3) // j k = 가로
        //                {
        //                    for (int k = 0; k < 3; ++k)
        //                    {
        //                        tiles[a, b] = transform.GetChild(l + i).GetChild(j + k).GetComponent<SudokuTile>();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        puzzleAmount = hiddingAmount;

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

        Init();
    }

    public void down()
    {
        puzzleAmount--;

        if (puzzleAmount == 0)
        {
            Debug.Log("R겜 끝");
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
