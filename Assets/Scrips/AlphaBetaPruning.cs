using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AlphaBetaPruning : MonoBehaviour
{
    private GameStatus gameStatus;
    // Start is called before the first frame update
    void Start()
    {
        gameStatus = GameObject.Find("Tic Tac Toe").GetComponent<GameStatus>();
        gameStatus.NextMove += BestMove;
    }

    private void BestMove(string[,] board)
    {
        int bestScore = int.MinValue;
        Point bestMove = new Point(-1, -1); // Khởi tạo vị trí di chuyển tốt nhất với giá trị mặc định

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (board[x, y] == "")
                {
                    board[x, y] = "O";
                    int score = AlphaBeta(board, 0, int.MinValue, int.MaxValue, false);
                    board[x, y] = "";

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = new Point(x, y);
                    }
                }
            }
        }
        UpdateBestMoveCell(bestMove);
    }

    // recusivley check every possilbe next move and moves after that untill game end
    private int AlphaBeta(string[,] boardStatus, int depth, int alpha, int beta, bool isMaximizing)
    {
        string result = gameStatus.CheckForWinner(boardStatus);
        if (result != null)
        {
            if (result == "X")
                return -10;
            else if (result == "O")
                return 10;
            else if (result == "tie")
                return 0;
        }

        if (isMaximizing)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (boardStatus[x, y] == "")
                    {
                        boardStatus[x, y] = "O";
                        alpha = Math.Max(alpha, AlphaBeta(boardStatus, depth + 1, alpha, beta, false));
                        boardStatus[x, y] = "";

                        if (beta <= alpha)
                            break; // Beta cut-off
                    }
                }
            }
            return alpha;
        }
        else
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (boardStatus[x, y] == "")
                    {
                        boardStatus[x, y] = "X";
                        beta = Math.Min(beta, AlphaBeta(boardStatus, depth + 1, alpha, beta, true));
                        boardStatus[x, y] = "";

                        if (beta <= alpha)
                            break; // Alpha cut-off
                    }
                }
            }
            return beta;
        }
    }

    private void UpdateBestMoveCell(Point bestMove) // Changes 2d array format to list of 9 game cells
    {
        int cellNumber = bestMove.X + bestMove.Y * 3;
        TicTacToeCellManager ticTacToeCellManager = transform.GetChild(cellNumber).GetComponent<TicTacToeCellManager>();
        ticTacToeCellManager.UpdateCellStatus();
    }
}
