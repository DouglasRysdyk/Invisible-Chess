using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTES : MonoBehaviour {}
/*
TODO: 
- Exact bookmark -- https://youtu.be/8Cp9UDl6nFM?t=994 
- I dunno if the tutorial will go over Promoting
- There is some sort of out of bounds error involving the pawns that I haven't run into yet, but makes sense and others say it exists.  
- Are the Queen and King in the correct positions?  
*/

/*
References: 
- Code source -- https://www.youtube.com/watch?v=FtGy7J8XD90 
- Used low poly chess set from Unity -- https://assetstore.unity.com/packages/3d/props/free-low-poly-chess-set-116856 
- Convert assets from one render pipeline to another -- https://www.youtube.com/watch?v=nB0r0c-SIVg 
*/

/*
Errors:
transform.position assign attempt for 'RookDark(Clone)' is not valid. Input position is { Infinity, 0.200000, 3.500000 }.
UnityEngine.Transform:set_position (UnityEngine.Vector3)
Chessboard:PositionSinglePiece (int,int,bool) (at Assets/Script/Chessboard.cs:184)
Chessboard:PositionAllPieces () (at Assets/Script/Chessboard.cs:173)
Chessboard:Awake () (at Assets/Script/Chessboard.cs:32)

Happens whenever you get a pawn to the opposite side.  
IndexOutOfRangeException: Index was outside the bounds of the array.
Pawn.GetAvailableMoves (ChessPiece[,]& board, System.Int32 tileCountX, System.Int32 tileCountY) (at Assets/Script/ChessPieces/Pawn.cs:13)
Chessboard.Update () (at Assets/Script/Chessboard.cs:87)
*/

/*
Notes: 
- This video has a really simple way of doing turns between two players.  It can probably be scaled up easily, but more importantly when he does the multiplayer videos it will hopefully be expanded on.  
  - https://www.youtube.com/watch?v=8Cp9UDl6nFM&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=6 
- Quick and dirty UI stuff -- https://youtu.be/8Cp9UDl6nFM?t=400 
- He did a bunch of off screen work to make the pieces look nicer including some additional code -- https://youtu.be/8Cp9UDl6nFM?t=1157 
*/




