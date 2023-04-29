using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public const int None = 0;
    public const int King = 1;
    public const int Queen = 2;
    public const int Pawn = 3;
    public const int Rook = 4;
    public const int Bishop = 5;
    public const int Knight = 6;

    public const bool White = false;
    public const bool Black = true;

    //Instance variables
    //White = false, black = true
    public int _piece;
    public bool _team;

    public Piece(int piece, bool team)
    {
        _piece = piece;
        _team = team;
    }
}
