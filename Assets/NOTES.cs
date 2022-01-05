using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTES : MonoBehaviour {}
/*
TODO: 
- Exact bookmark -- https://www.youtube.com/watch?v=6_o7_fmk2os&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=10 
- BUG -- When white wins the end screen says "White team wins" and "Black team wins".  I think this only happens when the black team wins first.  
  - I'll tackle this is Chessboard > OnResetButton.  I'll get the victory text and disable them both.  
  - If that doesn't work I'll think of something else.  
- BUG -- If you kill a piece and then en passant another the en passant'd piece appears in the wrong place in the graveyard.  
*/

/*
References: 
- Code source -- https://www.youtube.com/watch?v=FtGy7J8XD90 
- Used low poly chess set from Unity -- https://assetstore.unity.com/packages/3d/props/free-low-poly-chess-set-116856 
- Convert assets from one render pipeline to another -- https://www.youtube.com/watch?v=nB0r0c-SIVg 
*/

/*
Errors:
*/

/*
Notes: 
- This video has a really simple way of doing turns between two players.  It can probably be scaled up easily, but more importantly when he does the multiplayer videos it will hopefully be expanded on.  
  - https://www.youtube.com/watch?v=8Cp9UDl6nFM&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=6 
- Quick and dirty UI stuff -- https://youtu.be/8Cp9UDl6nFM?t=400 
- He did a bunch of off screen art work to make the pieces look nicer including some additional code -- https://youtu.be/8Cp9UDl6nFM?t=1157 
- https://www.youtube.com/watch?v=syWLqjl1YQo&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=9 
  - One could combine black and white into the same checks and simplifying avoid all the team checks by saving the resulting Y value at the top.
    int ourY = (team == 0) ? 0 : 7;

    Then we can do these things:
    Vector2Int[] kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ourY);
    And
    board[3, ourY] == null && board[2, ourY] == null && board[1, ourY] == null)
*/




