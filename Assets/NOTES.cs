using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTES : MonoBehaviour {}
/*
TODO: 
- Make it so the black material loads on the black team (right now they're all white).  
- Get rid of the hover clicker glitch.  
- Make the selection thing invisible (use the Multiply feature in the material)/  
- Fix pawns and rooks spawning in the wrong place.  
- Figure out how to position everything properly.  
*/

/*
References: 
- Code source -- https://www.youtube.com/watch?v=FtGy7J8XD90 
- Used low poly chess set from Unity -- https://assetstore.unity.com/packages/3d/props/free-low-poly-chess-set-116856 
- Convert assets from one render pipeline to another -- https://www.youtube.com/watch?v=nB0r0c-SIVg 
- 
*/

/*
Errors:
transform.position assign attempt for 'RookDark(Clone)' is not valid. Input position is { Infinity, 0.200000, 3.500000 }.
UnityEngine.Transform:set_position (UnityEngine.Vector3)
Chessboard:PositionSinglePiece (int,int,bool) (at Assets/Script/Chessboard.cs:184)
Chessboard:PositionAllPieces () (at Assets/Script/Chessboard.cs:173)
Chessboard:Awake () (at Assets/Script/Chessboard.cs:32)
*/




