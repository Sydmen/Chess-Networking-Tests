using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;

    BoardGraphics boardGraphics;
    PieceMoveManager moveManager;

    string defaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    private void Awake()
    {
        board = new Board();
        boardGraphics = FindObjectOfType<BoardGraphics>();
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(3.5f * board.SquareSize, 3.5f * board.SquareSize, -10);

        board.SetFromFen("6K1/q7/8/8/8/8/8/6r1");
        board.ColorToPlay = Piece.White;

        boardGraphics.CreateChessboard();
    }
}
