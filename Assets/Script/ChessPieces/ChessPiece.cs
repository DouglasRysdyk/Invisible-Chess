using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = 0, 
    Pawn = 1, 
    Rook = 2, 
    Knight = 3, 
    Bishop = 4, 
    Queen = 5, 
    King = 6 
}

//Virtual functions that get potential move and get special move for this piece 

public class ChessPiece : MonoBehaviour
{
    //1 = black
    public int team; 
    public int currentX; 
    public int currentY; 
    public ChessPieceType type;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one; //Scale down pieces on death 

    private void Start()
    {
        transform.rotation = Quaternion.Euler((team == 0 ) ? Vector3.zero : new Vector3(0, 180, 0));
    }

    private void Update()
    {
        //The 10 here at the end is arbitrary.  I can change it if I want.  Right now it's "quite fast." 
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    //Needs to be virtual to be overriden 
    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Highlighting the middle pieces for testing purposes.  
        //Why is this still here?  
        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));

        return r;
    }

    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        return SpecialMove.None; 
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
            transform.position = desiredPosition;
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }
}

/*
- "Virtual" means I'm able to override it is I need to? 
*/



