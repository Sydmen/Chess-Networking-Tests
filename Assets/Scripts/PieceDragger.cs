using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceDragger : MonoBehaviour
{
    GameManager gameManager;
    BoardGraphics boardGraphics;

    Vector2Int initialPiecePos;
    GameObject heldPieceObject;
    Piece heldPiece;
    bool isDragging;

    Board board;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        boardGraphics = FindObjectOfType<BoardGraphics>();
    }

    private void Start()
    {
        board = gameManager.board;
    }

    void Update()
    {
        if (InputManager.Player.LeftMouseButton.triggered)
        {
            TryDragPiece();
        }

        if (isDragging)
        {
            if (InputManager.Player.LeftMouseButton.ReadValue<float>() > 0)
            {
                UpdateDragPiece();
            }
            else
            {
                StopDragPiece();
            }
        }
    }

    private void TryDragPiece()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(InputManager.Player.MousePosition.ReadValue<Vector2>());
        Vector2Int nearestBoardPos = new Vector2Int((int)(mouseWorldPos.x / board.SquareSize + .5), (int)(mouseWorldPos.y / board.SquareSize + .5));

        //Not on board
        if (!Board.CordWithinBounds(nearestBoardPos))
        {
            return;
        }
        else if(board.GetPiece(nearestBoardPos) != null && board.GetPiece(nearestBoardPos)._team == board.ColorToPlay)
        {
            initialPiecePos = nearestBoardPos;
            heldPiece = board.GetPiece(nearestBoardPos);
            heldPieceObject = boardGraphics.GetPieceObject(nearestBoardPos);

            isDragging = true;
        }
    }

    private void UpdateDragPiece()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(InputManager.Player.MousePosition.ReadValue<Vector2>());

        heldPieceObject.transform.position = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
    }

    private void StopDragPiece()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(InputManager.Player.MousePosition.ReadValue<Vector2>());
        Vector2Int nearestBoardPos = new Vector2Int((int)(mouseWorldPos.x / board.SquareSize + .5), (int)(mouseWorldPos.y / board.SquareSize + .5));

        if (!Board.CordWithinBounds(nearestBoardPos))
        {
            CancelDragPiece();
        }
        else
        {
            TryMakeMove(initialPiecePos, nearestBoardPos);
        }

        heldPiece = null;
        heldPieceObject = null;

        isDragging = false;
    }

    private void TryMakeMove(Vector2Int start, Vector2Int target)
    {
        PieceMoveManager moveManager = new PieceMoveManager();

        List<Move> possibleMoves = moveManager.GenerateMoves(board, board.ColorToPlay);
        possibleMoves = moveManager.GetLegalMoves(possibleMoves);

        for (int i = 0; i < possibleMoves.Count; i++)
        {
            if (possibleMoves[i].start == start && possibleMoves[i].end == target)
            {
                MakeMove(possibleMoves[i]);
                return;
            }
        }

        CancelDragPiece();
    }

    private void MakeMove(Move finalMove)
    {
        board.MakeMove(finalMove);
        boardGraphics.MovePieceObject(finalMove.start, finalMove.end);

        initialPiecePos = Vector2Int.zero;

        board.EndMove();
    }

    private void CancelDragPiece()
    {
        heldPieceObject.transform.position = new Vector2(initialPiecePos.x, initialPiecePos.y) * board.SquareSize;
    }

    GUIStyle style = new GUIStyle();
    private void OnGUI()
    {
        style.fontSize = 50;
        style.normal.textColor = Color.white;
        string whoseTurn = board.ColorToPlay ? "Black" : "White";
        GUI.Label(new Rect(0, 0, 500, 100), $"{whoseTurn} to play...", style);
    }
}
