using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private Piece[,] Squares;

    public float SquareSize = 1;
    public bool ColorToPlay;
    public Vector2Int whiteKingPos, blackKingPos;

    Piece lastCapturedPiece;

    public Board()
    {
        Squares = new Piece[8, 8];
    }

    //I deadass can't find another way to do this
    public Board(Piece[,] pieces)
    {
        Squares = new Piece[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(pieces[x,y] != null)
                {
                    Squares[x, y] = new Piece(pieces[x, y]._piece, pieces[x, y]._team);
                }
            }
        }
    }

    public void EndMove()
    {
        ColorToPlay = !ColorToPlay;
        PieceMoveManager moveManager = new PieceMoveManager();
        List<Move> possibleMoves = moveManager.GenerateMoves(this, ColorToPlay);

        if(moveManager.GetLegalMoves(possibleMoves).Count <= 0)
        {
            Debug.Log("Game!");
        }
        else
        {
            Debug.Log("There are still valid moves!");
        }
    }

    public void MakeMove(Move move)
    {
        if (GetPiece(move.end) != null)
            lastCapturedPiece = GetPiece(move.end);
        else
            lastCapturedPiece = null;

        Piece pieceToMove = Squares[move.start.x, move.start.y];
        Squares[move.start.x, move.start.y] = null;
        Squares[move.end.x, move.end.y] = pieceToMove;
    }

    //Used for legal move checking
    public void UnmakeMove(Move move)
    {
        Piece pieceToReset = Squares[move.end.x, move.end.y];
        Squares[move.end.x, move.end.y] = null;
        Squares[move.start.x, move.start.y] = pieceToReset;

        if (lastCapturedPiece != null)
        {
            Squares[move.end.x, move.end.y] = lastCapturedPiece;
            lastCapturedPiece = null;
        }
    }

    public void SetFromFen(string fenNotation)
    {
        int x = 0;
        int y = 0;

        Dictionary<string, int> pieceLookup = new Dictionary<string, int>()
        {
            {"k", Piece.King},
            {"q", Piece.Queen },
            {"p", Piece.Pawn },
            {"r", Piece.Rook},
            {"b", Piece.Bishop},
            {"n", Piece.Knight}
        };

        foreach (char square in fenNotation)
        {
            //If is a number
            if (char.IsDigit(square))
            {
                //Making it side to side
                y += (int)char.GetNumericValue(square);
                continue;
            }

            //Next column
            if (square == '/' || y > 7 && x < 8)
            {
                x += 1;
                y = 0;
                continue;
            }

            //Get correct piece and color
            Piece piece = new Piece(pieceLookup[square.ToString().ToLower()], char.IsUpper(square) ? Piece.White : Piece.Black);

            //Set board to piece 
            Squares[x, y] = piece;
            y++;
        }
    }

    public bool InCheck(bool friendlyColor)
    {
        PieceMoveManager manager = new PieceMoveManager();
        List<Move> moves = manager.GenerateMoves(this, !friendlyColor);

        Vector2Int kingPosition = KingPosition(friendlyColor);

        for (int i = 0; i < moves.Count; i++)
        {
            if(moves[i].end == kingPosition)
            {
                return true;
            }
        }

        return false;
    }

    public Vector2Int KingPosition(bool team)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece piece = Squares[x, y];
                if(piece != null && piece._piece == Piece.King && piece._team == team)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(0,0);
    }

    public static bool CordWithinBounds(Vector2Int pos)
    {
        if(pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
        {
            return true;
        }

        return false;
    }

    public Piece GetPiece(Vector2Int pos)
    {
        if (!CordWithinBounds(pos)) return null;

        return Squares[pos.x, pos.y];
    }

    public Piece GetPiece(int x, int y)
    {
        if (!CordWithinBounds(new Vector2Int(x,y))) return null;

        return Squares[x, y];
    }

    public Piece[,] GetAllPieces()
    {
        return Squares;
    }
}
