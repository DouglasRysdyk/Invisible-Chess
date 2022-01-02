using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Art")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f; 
    [SerializeField] private Vector3 boardCenter = Vector3.zero; 
    [SerializeField] private float deathSize = 0.3f; 
    [SerializeField] private float deathSpacing = 0.3f; 
    [SerializeField] private float dragOffSet = 1.5f; 
    
    [Header("Prefabs && Materials")]
    [SerializeField] private GameObject[] prefabs; 
    [SerializeField] private Material[] teamMaterials; 

    //LOGIC 
    private ChessPiece[,] chessPieces; 
    private ChessPiece currentlyDragging; 
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles; 
    private Camera currentCamera; 
    private Vector2Int currentHover; 
    private Vector3 bounds; 

    private void Awake()
    {
        //Manually match to the art
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)// || !boardActive)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            //When the mouse button is pressed...
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    //Is it our turn? 
                    if (true)
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        //Get list of where I can go and highlight the tiles. 
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        HighlightTiles();
                    }
                }
            }

            //When the mouse button is released...
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                
                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));

                currentlyDragging = null; 
                RemoveHighlightTiles(); 
            }
        }
        else 
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }  

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null; 
                RemoveHighlightTiles(); 
            }
        }

        //If we're dragging a piece 
        if (currentlyDragging)
        {
            //Assumes the board is standing up right.  If it's angled or titled there could be a problem.  Vector3.up would probably need to be changed.  
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);  
            float distance = 0.0f; 
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffSet);
        }
    }

    //Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;  
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];

        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y{1}", x, y));
        tileObject.transform.parent = transform; 

        //Create a 3D object.
        //Need a mesh filter, and a mesh renderer.  
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        //Generate geometry (a triangle)
        //Create 4 corners 
        Vector3[] vertices = new Vector3[4]; 
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds; 
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds; 
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds; 
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds; 

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices; 
        mesh.triangles = tris; 
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        //Generate collider 
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    //Spawn the pieces 
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1; 

        //White team 
        chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);

        //Black team 
        chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type; 
        cp.team = team; 
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return cp; 
    }

    //Positioning 1
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(chessPieces[x,y] != null)
                {
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }

    //Positioning 2
    //NOTE: Force controls how fast the pieces move.  If it's false the pieces will move smoothly to their positions.  If it's true they will instantly teleport instead.  
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].SetPosition (GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    //Highlight Tiles
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

        availableMoves.Clear();
    }

    //Operations 
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos) 
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;
                
        return false; 
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        if (!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
            return false; 

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        //Check if there is another piece on the tile already 
        if (chessPieces[x, y] != null)
        {
            //ocp == Own Chess Piece (change later)
            ChessPiece ocp = chessPieces[x, y]; 

            if (cp.team == ocp.team)
                return false; 

            //If it's the enemy team
            if (ocp.team == 0)
            {
                deadWhites.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(
                    new Vector3 (8 * tileSize, yOffset, -1 * tileSize) 
                    - bounds 
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.forward * deathSpacing) * deadWhites.Count 
                );
            }
            else 
            {
                deadBlacks.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(
                    new Vector3 (-1 * tileSize, yOffset, 8 * tileSize) 
                    - bounds 
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.back * deathSpacing) * deadBlacks.Count 
                );
            }
        }

        chessPieces[x, y] = cp; 
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        //For testing purposes all moves are valid.  
        return true; 
    }
    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        //Break the entire game if this function doesn't work.
        return -Vector2Int.one; 
    }
}

/*
NOTES:
- Source -- https://youtu.be/FtGy7J8XD90?t=1906
  - Stopped at around 31:46.  His code is agnostic but it's harder to follow along without my own assets.  
- Constants are capitalized to make them distinct.  
- Mesh filter and mesh renderer are needed to createa 3D object.  
- Review everything relating to generating geometry.  
- After all of that you still need to add a material to the generated geometry.  
- Then you need to assign the "Normals."  These relate to how light bounces off of the geometry. 
- camera.current is a function that exists, but it probably has a lot of problems associated with it. 
- Can repalce the string.Format("X:{0}. Y:{1}", x, y) with $"X:{x}, Y:{y}" 
*/
