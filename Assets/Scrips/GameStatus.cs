using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    public Action<string[,]> NextMove = null;
    private TurnOrderManager turnOrder;
    public List<Transform> UsedCells;
    public bool GameStarted;
    public bool XTurn;
    public bool GameEnded;
    private string[,] BoardStatus = new string[3, 3] {{"","",""}, // Board cell structure
                                                      {"","",""},
                                                      {"","",""}};

    private Text gameEndingText;

    void Start()
    {
        turnOrder = GameObject.Find("Tic Tac Toe/Change Turn Order Button").GetComponent<TurnOrderManager>();
        gameEndingText = GameObject.Find("Tic Tac Toe/Game Ending Text").GetComponent<Text>();
        GameEnded = false;
        GameStarted = false;
        XTurn = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // Cập nhật thông tin trạng thái trò chơi, thêm ô được click vào danh sách các ô đã đi
    public void AddGridCell(Transform transform)
    {
        // Được sử dụng để reset lại lưới mới khi có thêm ô được click
        UsedCells.Add(transform);
        transform.GetComponent<TicTacToeCellManager>().boardUpdate += UpdateGameStatus;
    }

    // Được gọi khi một ô được click để đổi lượt đi tiếp theo và bắt đầu game
    public void UpdateGameStatus(Point clickedCell, string player) 
    {
        GameStarted = true;
        UpdateBoardStatus(clickedCell, player);
        XTurn = !XTurn;
        string result = CheckForWinner(BoardStatus);
        if (result != null)
        {
            HandelGameEnding(result);
        }
        if (!XTurn && !GameEnded)
        {
            NextMove?.Invoke(BoardStatus);
        }
    }

    // Cập nhật trạng thái bảng khi người chơi click vào một ô trống
    private void UpdateBoardStatus(Point clickedCell, string player)
    {
        BoardStatus[clickedCell.X, clickedCell.Y] = player;
    }

    // Kiểm tra luật chơi nếu có người chiến thắng thì trả về chuỗi kết quả
    public string CheckForWinner(string[,] boardStatus)
    {
        string winner = null;
        for (int i = 0; i < 3; i++)
        {
            // Kiểm tra hàng ngang
            if (ThreeEqualSymbols(boardStatus[i, 0], boardStatus[i, 1], boardStatus[i, 2]))
            {
                winner = boardStatus[i, 0];
            }
            // Kiểm tra đường thẳng
            if (ThreeEqualSymbols(boardStatus[0, i], boardStatus[1, i], boardStatus[2, i]))
            {
                winner = boardStatus[0, i];
            }
        }
        // Kiểm tra nếu hình thành được 1 đường chéo có cùng ký tự
        if (ThreeEqualSymbols(boardStatus[0, 0], boardStatus[1, 1], boardStatus[2, 2])
                || ThreeEqualSymbols(boardStatus[0, 2], boardStatus[1, 1], boardStatus[2, 0]))
        {
            winner = boardStatus[1, 1];
        }
        // Kiểm tra còn ô trống hay không
        if (!boardStatus.OfType<string>().Any(x => x == "") && winner == null)
        {
            return "tie";
        }
        else
        {
            return winner;
        }
    }

    // Kiểm tra nếu 3 biểu tượng không cách nhau bởi khoảng trắng
    private bool ThreeEqualSymbols(string a, string b, string c)
    {
        return (a == b && b == c) && a != "";
    }

    // Xử lí thông báo khi kết thúc game
    private void HandelGameEnding(string result)
    {
        GameEnded = true;
        if (result == "tie")
            gameEndingText.text = "<color=#ffa500ff><b><size=100>YOU TIED</size></b></color>\n<size=50><color=#808080ff>Press R to retry</color></size>";
        else if (result == "X")
            gameEndingText.text = "<color=#008000ff><b><size=100>YOU WIN</size></b></color>\n<size=50><color=#808080ff>Press R to restart</color></size>";
        else
            gameEndingText.text = "<color=#ff0000ff><b><size=100>YOU LOSE</size></b></color>\n<size=50><color=#808080ff>Press R to retry</color></size>";
    }

    private void RestartGame()
    {
        GameEnded = false;
        GameStarted = false;
        XTurn = true;
        gameEndingText.text = "";
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                BoardStatus[x, y] = "";
            }
        }
        foreach (Transform cell in UsedCells)
        {
            cell.GetComponent<TicTacToeCellManager>().ClearCells();
        }
        turnOrder.turnImage.sprite = Resources.Load<Sprite>("Sprites/X");
    }
}
