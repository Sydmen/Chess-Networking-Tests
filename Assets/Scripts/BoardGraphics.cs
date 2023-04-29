using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardGraphics : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Color darkSquareColor;
    [SerializeField] private Color lightSquareColor;

    [SerializeField] private float pieceSize = 1;

    GameManager gameManager;
    Board board;

    Sprite[] pieceTextures;
    GameObject[,] pieceGameObjects;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        board = gameManager.board;
    }

    //Create board (temporary)
    public void CreateChessboard()
    {
        GameObject chessBoardHolder = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        chessBoardHolder.transform.name = "Chessboard";

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject newSquare = Instantiate(squarePrefab, new Vector2(x, y) * board.SquareSize, Quaternion.identity);

                newSquare.transform.name = $"Square {x},{y}";
                newSquare.transform.parent = chessBoardHolder.transform;
                newSquare.transform.localScale = new Vector3(board.SquareSize, board.SquareSize, 1);
                newSquare.GetComponent<SpriteRenderer>().color = (x + y) % 2 == 0 ? darkSquareColor : lightSquareColor;
            }
        }

        chessBoardHolder.transform.position = new Vector3(0, 0, 10);

        CreatePieceGraphics(pieceSize);
    }

    //Create game objects for all pieces
    public void CreatePieceGraphics(float pieceScale)
    {
        pieceTextures = Resources.LoadAll<Sprite>("ChessPiecePlaceholder");
        pieceGameObjects = new GameObject[8, 8];

        GameObject piecesParent = new GameObject();
        piecesParent.transform.name = "Pieces";

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                //There is a piece
                if(board.GetPiece(x,y) != null)
                {
                    int textureIndex = board.GetPiece(x, y)._piece - 1;
                    if (board.GetPiece(x, y)._team == Piece.Black) textureIndex += 6;

                    GameObject pieceGameObject = new GameObject();

                    pieceGameObject.transform.position = new Vector2(x, y) * board.SquareSize;
                    pieceGameObject.transform.localScale = new Vector3(pieceScale, pieceScale, 1);
                    pieceGameObject.transform.parent = piecesParent.transform;

                    SpriteRenderer pieceRenderer = pieceGameObject.AddComponent<SpriteRenderer>();
                    Sprite pieceSprite = pieceTextures[textureIndex];
                    pieceRenderer.sprite = pieceSprite;

                    pieceGameObjects[x, y] = pieceGameObject;
                }
            }
        }
    }

    public GameObject GetPieceObject(Vector2Int pos)
    {
        return pieceGameObjects[pos.x, pos.y];
    }

    public void MovePieceObject(Vector2Int originalPos, Vector2Int newPos)
    {
        GameObject objectToMove = pieceGameObjects[originalPos.x, originalPos.y];
        SetPieceObject(newPos, objectToMove);
        pieceGameObjects[originalPos.x, originalPos.y] = null;
    }

    public void SetPieceObject(Vector2Int pos, GameObject piece)
    {
        //Destroy old object at that position
        if (pieceGameObjects[pos.x, pos.y] != null && piece != pieceGameObjects[pos.x, pos.y])
            Destroy(pieceGameObjects[pos.x, pos.y]);

        pieceGameObjects[pos.x, pos.y] = piece;
        piece.transform.position = (Vector2)pos * board.SquareSize;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board == null) return;

                if (board.GetPiece(x, y) != null)
                    Handles.Label(new Vector2(x * board.SquareSize, y * board.SquareSize), $"{board.GetPiece(x, y)._piece}, {board.GetPiece(x, y)._team}");
            }
        }
    }
}