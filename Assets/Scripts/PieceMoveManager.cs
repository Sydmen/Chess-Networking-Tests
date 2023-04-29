using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoveManager
{
    //Nasty
    //But I need those indexes to line up in the generate sliding moves function
    //Sometimes you gotta compromise how sexy it looks for convience
    //I should stop programming
    Vector2Int[] SlidingOffsets = {new Vector2Int(1,0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
                                    new Vector2Int(1,1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1)};

    Vector2Int[] KnightOffsets = {new Vector2Int(-2,1), new Vector2Int(-1, 2), new Vector2Int(1, 2), new Vector2Int(2, 1),
                                    new Vector2Int(-2,-1), new Vector2Int(-1, -2), new Vector2Int(1, -2), new Vector2Int(2, -1)};

    //Store an instance of the current board to test for checkmates later
    Board currentBoard;

    List<Move> moves;
    bool friendlyColor;
    public List<Move> GenerateMoves(Board board, bool color)
    {
        currentBoard = board;
        moves = new List<Move>();
        friendlyColor = color;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (currentBoard.GetPiece(x,y) == null) continue;

                int piece = currentBoard.GetPiece(x, y)._piece;
                if (piece != 0 && currentBoard.GetPiece(x, y)._team == friendlyColor)
                {
                    //Come back to change this later, adding a function to check for it feels sort of stupid imo
                    if (piece == Piece.Queen || piece == Piece.Bishop || piece == Piece.Rook)
                    {
                        GenerateSlidingMoves(new Vector2Int(x, y), piece);
                    }

                    if(piece == Piece.Pawn)
                    {
                        GeneratePawnMoves(new Vector2Int(x, y));
                    }

                    if (piece == Piece.King)
                    {
                        GenerateKingMoves(new Vector2Int(x, y));
                    }

                    if(piece == Piece.Knight)
                    {
                        GenerateKnightMoves(new Vector2Int(x, y));
                    }
                }
            }
        }

        return moves;
    }

    //Made it's own function so that only the player checks it once
    //The check function uses a move manager to go for check
    //--------------------
    //This function will be the reason i kill myself
    //FUCK OOP
    public List<Move> GetLegalMoves(List<Move> movesToCheck)
    {
        int index = movesToCheck.Count;
        for (int i = movesToCheck.Count - 1; i >= 0; i--)
        {
            //its cool to make a new class because this only runs every now and then and gets discarded anyways
            //I think

            //Dear any programmer who may have the displeasure of looking at my code, I'm sorry
            //With love <3
            Board hypotheticalBoard = new Board(currentBoard.GetAllPieces());
            Move move = movesToCheck[i];
            hypotheticalBoard.MakeMove(move);

            //Check if fake move still ends with a check
            if (hypotheticalBoard.InCheck(currentBoard.ColorToPlay))
            {
                movesToCheck.Remove(move);
            }
        }

        if(movesToCheck.Count == 0)
        {
            Debug.Log("Checkmate!");
        }

        return movesToCheck;
    }

    //TO DO
    //Add checks for checkmate
    //Or getting out of check
    public void GenerateKingMoves(Vector2Int startSquare)
    {
        for (int i = 0; i < SlidingOffsets.Length; i++)
        {
            Vector2Int targetSquare = startSquare + SlidingOffsets[i];
            if (!Board.CordWithinBounds(targetSquare)) continue;

            Piece pieceOnSquare = currentBoard.GetPiece(targetSquare);

            //Friendly fire is a no no
            if (pieceOnSquare != null && pieceOnSquare._team == friendlyColor) continue;

            moves.Add(new Move(startSquare, targetSquare));
        }
    }

    //TO DO
    //Castling 
    public void GenerateSlidingMoves(Vector2Int startSquare, int piece)
    {
        int startIndex = (piece == Piece.Bishop) ? 4 : 0;
        int endIndex = (piece == Piece.Rook) ? 4 : 8;

        for (int i = startIndex; i < endIndex; i++)
        {
            for (int n = 0;  n < 8;  n++)
            {
                //Get in bounds target
                Vector2Int targetSquare = startSquare + (SlidingOffsets[i] * (n+1));
                if (!Board.CordWithinBounds(targetSquare)) break;

                Piece pieceOnSquare = currentBoard.GetPiece(targetSquare);

                //Friendly fire is a no no
                if (pieceOnSquare != null && pieceOnSquare._team == friendlyColor) break;

                moves.Add(new Move(startSquare, targetSquare));

                //Break if it's an enemy
                if (pieceOnSquare != null && pieceOnSquare._team != friendlyColor) break;
            }
        }
    }

    public void GenerateKnightMoves(Vector2Int startSquare)
    {
        for (int i = 0; i < KnightOffsets.Length; i++)
        {
            Vector2Int targetSquare = startSquare + KnightOffsets[i];
            if (!Board.CordWithinBounds(targetSquare)) continue;

            Piece pieceOnSquare = currentBoard.GetPiece(targetSquare);

            //Friendly fire is a no no
            if (pieceOnSquare != null && pieceOnSquare._team == friendlyColor) continue;

            moves.Add(new Move(startSquare, targetSquare));
        }
    }

    //TO DO
    //En passant
    public void GeneratePawnMoves(Vector2Int startSquare)
    {
        int pawnStartX = currentBoard.ColorToPlay ? 1 : 6;
        int pawnDirection = currentBoard.ColorToPlay ? 1 : -1;

        //Normal forward
        Vector2Int forwardPosition = startSquare + new Vector2Int(pawnDirection, 0);

        if(Board.CordWithinBounds(forwardPosition) && currentBoard.GetPiece(forwardPosition) == null)
        {
            moves.Add(new Move(startSquare, forwardPosition));
        }

        //Double forward
        if (startSquare.x == pawnStartX)
        {
            moves.Add(new Move(startSquare, new Vector2Int(forwardPosition.x + pawnDirection, startSquare.y)));
        }

        //Kill pawn (up)
        forwardPosition.y += 1;
        if(Board.CordWithinBounds(forwardPosition) &&
            currentBoard.GetPiece(forwardPosition) != null &&
            currentBoard.GetPiece(forwardPosition)._team != friendlyColor)
        {
            moves.Add(new Move(startSquare, forwardPosition));
        }

        //Kill pawn (down)
        forwardPosition.y -= 2;
        if (Board.CordWithinBounds(forwardPosition) &&
            currentBoard.GetPiece(forwardPosition) != null &&
            currentBoard.GetPiece(forwardPosition)._team != friendlyColor)
        {
            moves.Add(new Move(startSquare, forwardPosition));
        }

    }
}

public struct Move
{
    public readonly Vector2Int start;
    public readonly Vector2Int end;

    public Move(Vector2Int start, Vector2Int end)
    {
        this.start = start;
        this.end = end;
    }
}