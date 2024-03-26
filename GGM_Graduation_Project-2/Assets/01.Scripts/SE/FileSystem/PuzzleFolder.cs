using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleFolder : MonoBehaviour
{
    public int myPuzzleNum;     // 1번이 스토쿠, 2번이 8퍼즐

    public void OpenPuzzle()
    {
        FileManager.instance.OpenPuzzleLock(myPuzzleNum);
        if (myPuzzleNum == 1)
        {
            Sudoku.Instance.Setting();
        }
        else if (myPuzzleNum == 2)
        {
            // 민서 퍼즐 인잇할거 넣어주기
        }
    }
}
