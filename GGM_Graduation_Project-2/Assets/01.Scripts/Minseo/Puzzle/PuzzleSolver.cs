using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleState : IComparable<PuzzleState>
{
    public int[,] Board { get; } // 퍼즐의 상태를 나타내는 2차원 배열
    public int GCost { get; } // 시작 상태로부터의 이동 비용 (현재까지의 이동 횟수)
    public int HCost { get; } // 현재 상태로부터 목표 상태까지의 예상 이동 비용 (휴리스틱)

    public PuzzleState(int[,] board, int gCost, int hCost)
    {
        Board = board;
        GCost = gCost;
        HCost = hCost;
    }

    // A* 알고리즘을 위해 F 값 계산
    public int FCost => GCost + HCost;

    // 퍼즐 상태를 출력
    public void Print()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                Console.Write(Board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    // IComparable 인터페이스 구현
    public int CompareTo(PuzzleState other)
    {
        return FCost.CompareTo(other.FCost);
    }
}


public class PuzzleSolver : MonoBehaviour
{
    // 이동 가능한 인접한 타일의 위치
    private static readonly int[,] moves = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

    // A* 알고리즘을 사용하여 최소 이동 횟수를 계산하는 함수
    public static int CalculateMinimumMoves(int[,] initialState, int[,] goalState)
    {
        HashSet<string> visited = new HashSet<string>();
        Queue<PuzzleState> openSet = new    Queue<PuzzleState>();

        // 시작 상태를 큐에 추가
        PuzzleState startState = new PuzzleState(initialState, 0, CalculateHeuristic(initialState, goalState));
        openSet.Enqueue(startState);

        while (openSet.Count > 0)
        {
            PuzzleState currentState = openSet.Dequeue();

            // 현재 상태가 목표 상태인지 확인
            if (IsEqual(currentState.Board, goalState))
            {
                return currentState.GCost; // 최소 이동 횟수 반환
            }

            // 현재 상태에서 가능한 모든 다음 상태를 탐색
            foreach (PuzzleState nextState in GetNextStates(currentState, goalState))
            {
                string nextStateHash = HashState(nextState.Board);

                // 방문한 상태인지 확인
                if (!visited.Contains(nextStateHash))
                {
                    visited.Add(nextStateHash);
                    openSet.Enqueue(nextState);
                }
            }
        }
        return -1;
    }

    // 두 상태가 같은지 확인하는 함수
    private static bool IsEqual(int[,] state1, int[,] state2)
    {
        for (int i = 0; i < state1.GetLength(0); i++)
        {
            for (int j = 0; j < state1.GetLength(1); j++)
            {
                if (state1[i, j] != state2[i, j])
                    return false;
            }
        }
        return true;
    }

    // 상태를 해시로 변환하는 함수
    private static string HashState(int[,] state)
    {
        string hash = "";

        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
            {
                hash += state[i, j].ToString();
            }
        }

        return hash;
    }

    // 현재 상태에서 가능한 다음 상태를 반환하는 함수
    private static List<PuzzleState> GetNextStates(PuzzleState currentState, int[,] goalState)
    {
        List<PuzzleState> nextStates = new List<PuzzleState>();

        int emptyTileX = -1;
        int emptyTileY = -1;

        // 빈 타일의 위치를 찾음
        for (int i = 0; i < currentState.Board.GetLength(0); i++)
        {
            for (int j = 0; j < currentState.Board.GetLength(1); j++)
            {
                if (currentState.Board[i, j] == 0)
                {
                    emptyTileX = i;
                    emptyTileY = j;
                    break;
                }
            }
        }

        // 현재 상태에서 가능한 다음 상태를 생성
        for (int i = 0; i < moves.GetLength(0); i++)
        {
            int newX = emptyTileX + moves[i, 0];
            int newY = emptyTileY + moves[i, 1];

            if (IsValidPosition(newX, newY, currentState.Board.GetLength(0), currentState.Board.GetLength(1)))
            {
                int[,] newBoard = (int[,])currentState.Board.Clone();
                newBoard[emptyTileX, emptyTileY] = newBoard[newX, newY];
                newBoard[newX, newY] = 0;

                PuzzleState nextState = new PuzzleState(newBoard, currentState.GCost + 1, CalculateHeuristic(newBoard, goalState));
                nextStates.Add(nextState);
            }
        }

        return nextStates;
    }

    // 유효한 위치인지 확인하는 함수
    private static bool IsValidPosition(int x, int y, int maxX, int maxY)
    {
        return x >= 0 && x < maxX && y >= 0 && y < maxY;
    }

    private static int CalculateHeuristic(int[,] currentState, int[,] goalState)
    {
        int heuristic = 0;

        for (int i = 0; i < currentState.GetLength(0); i++)
        {
            for (int j = 0; j < currentState.GetLength(1); j++)
            {
                int value = currentState[i, j];

                if (value != 0) // 빈 타일은 고려하지 않음
                {
                    // 현재 타일의 위치를 찾음
                    for (int x = 0; x < goalState.GetLength(0); x++)
                    {
                        for (int y = 0; y < goalState.GetLength(1); y++)
                        {
                            if (goalState[x, y] == value)
                            {
                                // 현재 타일의 목표 위치와의 맨해튼 거리를 계산하여 휴리스틱에 추가
                                heuristic += Math.Abs(i - x) + Math.Abs(j - y);
                                break;
                            }
                        }
                    }
                }
            }
        }

        return heuristic;
    }
}
