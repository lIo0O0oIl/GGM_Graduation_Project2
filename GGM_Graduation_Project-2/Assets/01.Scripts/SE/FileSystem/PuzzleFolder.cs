using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleFolder : MonoBehaviour
{
    public int myPuzzleNum;     // 1번이 스토쿠, 2번이 8퍼즐
    
    [SerializeField]
    private Transform board;
    [SerializeField]
    private GameObject puzzlePrefab;

    public void OpenPuzzle()
    {
 /*       for (int i = 3; i < FileManager.instance.puzzlePanel.transform.childCount; i++)
        {
            Destroy(FileManager.instance.puzzlePanel.transform.GetChild(i));
        }*/
        if (myPuzzleNum == 2)
        {
            Instantiate(puzzlePrefab, board);
        }
        FileManager.instance.OpenPuzzleLock(myPuzzleNum);
        if (myPuzzleNum == 1)
        {
            Sudoku.Instance.Setting();
        }
    }
}
