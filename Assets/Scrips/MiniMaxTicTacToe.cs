using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class MiniMaxTicTacToe : MonoBehaviour
{
    private GameStatus gameStatus;
    private Button button;

    void Start()
    {
        gameStatus = GameObject.Find("Tic Tac Toe").GetComponent<GameStatus>();
        gameStatus.NextMove += BestMove;
    }
    public void DisableScript()
    {
        // Tắt chức năng của script ở đây
        enabled = false;
    }

    public void EnableScript()
    {
        // Bật chức năng của script ở đây
        enabled = true;
    }

    // Kiểm tra tất cả các ô có thế đi ở bước tiếp theo
    private void BestMove(string[,] board) 
    {
        int bestScore = int.MinValue;
        Point bestMove;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (board[x, y] == "")
                {
                    board[x, y] = "O";
                    int score = MiniMax(board, 0, false);
                    board[x, y] = "";
                    // Nếu có 2 ô có điểm cao nhất bằng nhau thì lấy ô được duyệt trước làm bước đi tiếp theo
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

    // Tính điểm cho bước đi ở ô được gọi
    private int MiniMax(string[,] boardStatus, int depth, bool isMaximizing) 
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
        // Nước đi có thể chiến thắng
        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (boardStatus[x, y] == "")
                    {
                        boardStatus[x, y] = "O";
                        int score = MiniMax(boardStatus, depth++, !isMaximizing);
                        boardStatus[x, y] = "";
                        bestScore = Math.Max(bestScore, score);
                    }
                }
            }
            return bestScore;
        }
        // Đánh giá bước đi tệ nhất cho bản thân (nước đi có thể khiến cho đối thủ chiến thắng)
        else
        {
            int bestScore = int.MaxValue;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (boardStatus[x, y] == "")
                    {
                        boardStatus[x, y] = "X";
                        int score = MiniMax(boardStatus, depth++, !isMaximizing);
                        boardStatus[x, y] = "";
                        bestScore = Math.Min(bestScore, score);
                    }
                }
            }
            return bestScore;
        }
    }

    // Đánh dấu hình O vào ô bestMove và câp nhật lại trạng thái ô sau khi đi
    private void UpdateBestMoveCell(Point bestMove)
    {
        int cellNumber = bestMove.X + bestMove.Y * 3;
        TicTacToeCellManager ticTacToeCellManager = transform.GetChild(cellNumber).GetComponent<TicTacToeCellManager>();
        ticTacToeCellManager.UpdateCellStatus();
    }
}
