/*
TODO: 
- Exact bookmark -- https://youtu.be/TJLoTsM2Go0?t=1586 
  - My version is seriously divergent now.  If I watch anymore I'll be in trouble and alone.  Thankfully I'm pretty confident everything before now has been on point.  So I'll restart the video and try to follow along more closely.  
  - 
*/

/*
Bugs
- BUG -- When white wins the end screen says "White team wins" and "Black team wins".  I think this only happens when the black team wins first.  
  - I'll tackle this is Chessboard > OnResetButton.  I'll get the victory text and disable them both.  
  - If that doesn't work I'll think of something else.  
- BUG -- If you kill a piece and then en passant another the en passant'd piece appears in the wrong place in the graveyard.  
- BUG -- When one side wins the other side doesn't get a game over screen.  
- Bug 1. Reproduced by playing local player, and let Black win, then accept rematch. You'll realize that you can't move White. This is an edge case.
Fix for 1: This happens because in your MoveTo() function, if it's local player you swap currentTeam after every move. If White wins, White makes the last move and currentTeam changes to Black. Notice that you didn't reset currentTeam to White, therefore when you start your 2nd game, isWhite is true but currentTeam is Black, therefore you can't move anything. Hence, the solultion is to set currentTeam = 0 at the end of MoveTo() if it's a localGame and you predict the game is ending (i.e. CheckForCheckmate() returns true).

Bug 2: Reproduced by playing local player -> let any team win -> go back menu -> play again -> at the end of 2nd match "Opponent left" will be shown and button will be disabled. This bug appears for multiplayer as well.
Fix for Bug 2: Bug 2 occurs because inside OnMenuButton(), currentTeam is reset to -1 when u click on the Menu button. After that, rematchButton.interactable is set back to true. However, even later, when OnRematchClient() is called,  interactable will be set to false again. As a recap the code inside OnRematchClient contains 

if (rm.teamId != currentTeam) { // currentTeam is now -1 since u reset it in OnMenuButton
     ....
    rematchButton.interactable = false
}

Fix for Bug 2: To solve this, either you don't reset currentTeam in OnMenuButton, or add "&& currentTeam != -1" as a condition in the if statement above.

Bug 3: Try playing local player on Unity, then open your built game and click Host. You'll realize that everything get very weird.
Fix for Bug 3: The reason for this is that both client and server runs on the same port on your device. I have no fixes for this because I think this is the limitation of this architecture. But I believe this problem only happens if you try to play both local and online on the same device.
*/

/*
Optimizations 
- Using the Canvas as a GameManager right now.  Ideally we'd have a separate Empty GameObject that holds the server and client.  
- Fix the highlight it sucks.  It's significantly far away from the board.  
- Make a branch or a fork where I reimplement the server script as a singleton?
- Maybe switch the Start() back to Awake() in Chessboard.  It's not recommended to switch the order for stuff.  He did this because the game UI was loading too late I think.  
*/

/* 
After Videos: 
- Outline work I have to do for Invisible Chess [1-2]
- Fix bugs 
  - 1 day per bug.  
  - If I can't fix a bug then move on.  
  - If I can't fix any then come back later.  
- 
*/
 
/*
References: 
- Code source -- https://www.youtube.com/watch?v=FtGy7J8XD90 
- Used low poly chess set from Unity -- https://assetstore.unity.com/packages/3d/props/free-low-poly-chess-set-116856 
- Convert assets from one render pipeline to another -- https://www.youtube.com/watch?v=nB0r0c-SIVg 
- Remember -- https://gitlab.com/MichaelDoyon/epitomegames 
*/

/*
Lesson Notes 1: 
- LESSON NOTES 2 ARE SPREAD THROUGHOUT THE SCRIPTS THEMSELVES AT KEY POINTS 
- This video has a really simple way of doing turns between two players.  It can probably be scaled up easily, but more importantly when he does the multiplayer videos it will hopefully be expanded on.  
  - https://www.youtube.com/watch?v=8Cp9UDl6nFM&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=6 
- UI stuff, lots of good things to remember: 
  - https://youtu.be/8Cp9UDl6nFM?t=400 
  - https://www.youtube.com/watch?v=6_o7_fmk2os&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=11 
  - UI is made using Mechanim and Animator?  
    - Animator I know is built into Unity.  
  - The menus are laid out in world space depending on where we want the menus to transition in from.  Interesting.  
- He did a bunch of off screen art work to make the pieces look nicer including some additional code -- https://youtu.be/8Cp9UDl6nFM?t=1157 
- The NetKeepAlive script is a mostly empty script because it is the message that is being sent to and from the server and the client.  The bigger this message is the slower everything is.  
- Look up how to generate a method from one that I just declared like with OnWelcomeServer.  
- Tried connecting to a server that was not being hosted.  
- https://www.youtube.com/watch?v=syWLqjl1YQo&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp&index=9 
  - One could combine black and white into the same checks and simplifying avoid all the team checks by saving the resulting Y value at the top.
    int ourY = (team == 0) ? 0 : 7;

    Then we can do these things:
    Vector2Int[] kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ourY);
    And
    board[3, ourY] == null && board[2, ourY] == null && board[1, ourY] == null)
- Use very basic types when sending information through the internet unless you have a way of deserializing them.  You could write an extension that allows you a certain amount of bytes (needs to be done for both sides) to, say, pass a Vector2 through, but it's simpler to just send basic types.  
- Right now one player makes a move, and then the other player needs to update what happened.  Another approach would be to send a "Request To Move" and then broadcast that move to both players at the same time.  This can be, unsurprisingly, slower, and leads to a noticable delay as both players wait for something to happen.  
*/

/*
Work Notes: 
- MainCamera: 
  - Position = Vector3(0,9,-9)
  - Rotation = Vector3(0,270,0)
- WhiteCamera: 
  - Position = Vector3(0,6,-6)
  - Rotation = Vector3(50.9999924,0,0)
- BlackCamera: 
  - Position = Vector3(0,6,6.25)
  - Rotation = Vector3(50.9999924,180,0)
*/

/*ERROR 
IndexOutOfRangeException: Index was outside the bounds of the array.
GameUI.ChangeCamera (CameraAngle index) (at Assets/Script/GameUI.cs:36)
Chessboard.OnStartClient (NetMessage obj) (at Assets/Script/Chessboard.cs:715)
NetStartGame.ReceivedOnClient () (at Assets/Script/Net/NetMessages/NetStartGame.cs:26)
NetUtility.OnData (Unity.Networking.Transport.DataStreamReader stream, Unity.Networking.Transport.NetworkConnection cnn, Server server) (at Assets/Script/Net/NetMessages/NetUtility.cs:35)
Client.UpdateMessagePump () (at Assets/Script/Net/Client.cs:89)
Client.Update () (at Assets/Script/Net/Client.cs:58)
*/
