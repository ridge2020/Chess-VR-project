using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece{
    public string color = "";
    public GameObject objPrefab = null;
    public GameObject obj = null;
    //public Vector3 position;
    public string name = "";
    public char symbol = 'a'; 
    public char column = 'a'; // a - h
    public bool empassant = false;
    public int row = 0; // 1 - 8
    public Square square = null;
    public bool moved = false;
    public bool justMoved = false;
    //return all squares that can be moved to

    public void print(){
        Debug.Log(name);
    }

    public void tempMove(Square square, Board board)
    {
        if (!empassant)
        {
        }
        else
        {   //if piece em passant is detected
            if (board.playerTurn() == "white")
            {
                square.getDown(board).piece = null; // piece at attack is replaced by this
            }
            else
            {
                square.getUp(board).piece = null;
            }
        }
        Square comingFrom = board.get("" + this.column + this.row);
        square.piece = comingFrom.piece; // piece at attack is replaced by this
        comingFrom.piece = null; // the square moved from piece has moved
        board.setPositions();
        board.turn++;
    }

    public List<Square> legalMoves(Board board)
    {
        Debug.Log("Start legal moves");
        //whose turn
        string color = board.playerTurn();
        Board copyBoard = new Board(board); // create copy of board
        Debug.Log("GOT COPY OF BOARD");
        List<Square> moves = move(board); //get pieces moves
        Debug.Log("GOT PIECE MOVES");
        foreach(Square sq in moves){
            sq.print();
        }
        //Board copyBoard = new Board(this);
        List<Square> removeSquares = new List<Square>();
        if(this.color == color)
        {
            foreach (Square square in moves)
            {
                copyBoard = new Board(board); // create copy of board
                copyBoard.get("" + this.column + this.row).piece.tempMove(copyBoard.get("" + square.column + square.row), copyBoard);
                string newColor = copyBoard.playerTurn();
                int check = copyBoard.checkChecker();
                if (newColor == "white" && (check == 2 || check == -1))
                {
                    removeSquares.Add(square);//illegal move
                }
                else if (newColor == "black" && (check == 2 || check == 1))
                {
                    removeSquares.Add(square);
                }
            }
        }
        else
        {
            moves = new List<Square>();
        }


        foreach(Square square in removeSquares){
            moves.Remove(square);
        }

        Debug.Log("END LEGAL MOVES");
        foreach(Square sq in moves){
            sq.print();
        }
        return moves;
    }

    public void makeMove(Square square, Board board)
    {
        Debug.Log("START MAKE MOVE");
        //check if square is in moves
        List<Square> moves = legalMoves(board);
        if (moves.Contains(square))
        {
            if (!empassant)
            {
            }
            else
            {
                if (board.playerTurn() == "white")
                {
                    square.getDown(board).piece = null; // piece at attack is replaced by this
                }
                else
                {
                    square.getUp(board).piece = null;
                }
            }
            Square comingFrom = board.get("" + this.column + this.row);
            square.piece = comingFrom.piece; // piece at attack is replaced by this
            comingFrom.piece = null; // the square moved from piece has moved
            board.setPositions();
            board.turn++;
            //move piece to attack
        } else {
            Debug.Log("Invalid move!");
        }
        Debug.Log("END MAKE MOVE");

    }

    public GameObject getPrefab(){
        return objPrefab;
    }
    //evaluate moves of the piece
    public virtual List<Square> move(Board board){
        return new List<Square>();
    }

}

public class Pawn : Piece{
    public Pawn(string color){
        name = "Pawn";
        symbol = 'P'; // can't null char
        this.color = color;
        if(color == "white"){
            objPrefab = Resources.Load("PawnLight") as GameObject;
        } else{
            objPrefab = Resources.Load("PawnDark") as GameObject;
        }
    }

    public override List<Square> move(Board board)
    {
        empassant = false;
        List<Square> ls = new List<Square>();

        Square cur = board.get("" + this.column + this.row);
        bool attack = false;
        if (this.color == "black")
        {
            //check for diagonal attack (downward left and right)
            //downRight
            if (cur.getDownRight(board) != null && attack == false)
            {
                //logic for piece attack
                if (cur.getDownRight(board).piece != null)
                {
                    if (cur.getDownRight(board).piece.color != this.color)
                    {
                        ls.Add(cur.getDownRight(board));//attack stop search
                        attack = true;
                    }
                    else
                    {
                        attack = true;//piece if our color, no attack (don't add move)
                    }
                }
            }
            //downLeft
            cur = board.get("" + this.column + this.row);
            attack = false;
            if (cur.getDownLeft(board) != null && attack == false)
            {
                //logic for piece attack
                if (cur.getDownLeft(board).piece != null)
                {
                    if (cur.getDownLeft(board).piece.color != this.color)
                    {
                        ls.Add(cur.getDownLeft(board));//attack stop search
                        attack = true;
                    }
                    else
                    {
                        attack = true;//piece if our color, no attack (don't add move)
                    }
                }
            }
            //move down
            //down
            cur = board.get("" + this.column + this.row);
            attack = false;
            if (cur.getDown(board) != null && attack == false)
            { // move once
              //logic for piece attack
                if (cur.getDown(board).piece != null)
                {
                    attack = true;
                }
                else
                {
                    ls.Add(cur.getDown(board)); // if no piece in way and square, add
                }
            }
            if (cur.getDown(board) != null && attack == false && this.moved == false)
            { // move twice
                if (cur.getDown(board).getDown(board) != null)
                {
                    //logic for piece attack
                    if (cur.getDown(board).getDown(board).piece != null)
                    {

                    }
                    else
                    {
                        ls.Add(cur.getDown(board).getDown(board)); // if no piece in way and square, add
                    }
                }
            }
            //check for empasse

        }
        else if (color == "white")
        {
            //check for diagonal attack (upward left and right)
            //topRight
            if (cur.getTopRight(board) != null && attack == false)
            {
                //logic for piece attack
                if (cur.getTopRight(board).piece != null)
                {
                    if (cur.getTopRight(board).piece.color != this.color)
                    {
                        ls.Add(cur.getTopRight(board));//attack stop search
                        attack = true;
                        //attackPiece = cur.getTopRight(board);
                    }
                    else
                    {
                    }
                }
            }
            //topLeft
            cur = board.get("" + this.column + this.row);
            attack = false;
            if (cur.getTopLeft(board) != null && attack == false)
            {
                //logic for piece attack
                if (cur.getTopLeft(board).piece != null)
                {
                    if (cur.getTopLeft(board).piece.color != this.color)
                    {
                        ls.Add(cur.getTopLeft(board));//attack
                        attack = true;
                        //attackPiece = cur.getTopLeft(board);
                    }
                    else
                    {
                    }
                }
            }
            // move up
            cur = board.get("" + this.column + this.row);
            attack = false;
            if (cur.getUp(board) != null && attack == false)
            {
                //logic for piece attack
                if (cur.getUp(board).piece != null)
                {
                    attack = true;
                }
                else
                {
                    ls.Add(cur.getUp(board)); // if no piece in way and square, add
                }
            }
            if (cur.getUp(board) != null && attack == false && moved == false)
            {
                if (cur.getUp(board).getUp(board) != null)
                {
                    //logic for piece attack
                    if (cur.getUp(board).getUp(board).piece != null)
                    {

                    }
                    else
                    {
                        ls.Add(cur.getUp(board).getUp(board)); // if no piece in way and square, add
                    }
                }
            }
            //THIS IS CODED FOR WHITE. BLACK NEEDS BOTTEM LEFT AND RIGHT
            //check for em passant
            cur = board.get("" + this.column + this.row);
            if (row == 5 && board.playerTurn() == "white")
            {
                if (cur.getTopLeft(board) != null)
                {
                    if (cur.getLeft(board).piece != null)
                    {
                        if (cur.getLeft(board).piece.symbol == 'P' && cur.getLeft(board).piece.color != this.color)
                        {
                            if (cur.getLeft(board).piece.justMoved == true)
                            {
                                ls.Add(cur.getTopLeft(board)); // if no piece in way and square, add
                                Debug.Log("EMPASSANT!");
                                empassant = true;
                            }
                        }
                    }
                }
                if (cur.getTopRight(board) != null)
                {
                    if (cur.getRight(board).piece != null)
                    {
                        if (cur.getRight(board).piece.symbol == 'P' && cur.getRight(board).piece.color != this.color)
                        {
                            if (cur.getRight(board).piece.justMoved == true)
                            {
                                ls.Add(cur.getTopRight(board)); // if no piece in way and square, add
                                empassant = true;
                                Debug.Log("EMPASSANT!");

                            }
                        }
                    }
                }
            }
            cur = board.get("" + this.column + this.row);
            if (row == 4 && board.playerTurn() == "black")
            {
                if (cur.getDownLeft(board) != null)
                {
                    if (cur.getLeft(board).piece != null)
                    {
                        if (cur.getLeft(board).piece.symbol == 'P' && cur.getLeft(board).piece.color != this.color)
                        {
                            if (cur.getLeft(board).piece.justMoved == true)
                            {
                                ls.Add(cur.getDownLeft(board)); // if no piece in way and square, add
                                Debug.Log("EMPASSANT!");
                                empassant = true;
                            }
                        }
                    }
                }
                if (cur.getDownRight(board) != null)
                {
                    if (cur.getRight(board).piece != null)
                    {
                        if (cur.getRight(board).piece.symbol == 'P' && cur.getRight(board).piece.color != this.color)
                        {
                            if (cur.getRight(board).piece.justMoved == true)
                            {
                                ls.Add(cur.getDownRight(board)); // if no piece in way and square, add
                                empassant = true;
                                Debug.Log("EMPASSANT!");

                            }
                        }
                    }
                }
            }
        }

        return ls;
    }
}

public class Knight : Piece{
    
    public Knight(string color){
        name = "Knight";
        symbol = 'N';
        this.color = color;
        if(color == "white"){
            objPrefab = Resources.Load("KnightLight") as GameObject;
        } else{
            objPrefab = Resources.Load("KnightDark") as GameObject;
        }
        //this.column = column;
        //this.row = row;
    }

    public override List<Square> move(Board board){ // pass it  'this' from board
        List<Square> ls = new List<Square>();
        //Bishops move in diagonals on the board. 

        //REWRITE THIS TO BE MORE EFFICIENT
        //instead of using get
        //add to column and row and check if it exists.

        //move until you find a piece 
        //if piece, based on color/piece , attack / check
        //right
        Square cur = board.get("" + this.column + this.row); //get square piece is on
        bool attack = false;
        //right, up, up
        if(cur.getRight(board) != null){
                if(cur.getRight(board).getUp(board) != null){
                    if(cur.getRight(board).getUp(board).getUp(board) != null){
                        if(cur.getRight(board).getUp(board).getUp(board).piece != null){
                            if(cur.getRight(board).getUp(board).getUp(board).piece.color != this.color){
                                ls.Add(cur.getRight(board).getUp(board).getUp(board));//attack stop search
                                attack = true;
                            }
                    } else {
                        ls.Add(cur.getRight(board).getUp(board).getUp(board));//no piece on square so move.
                    }
                }
            }
        }
        //right, down, down
        if(cur.getRight(board) != null){
            if(cur.getRight(board).getDown(board) != null){
                if(cur.getRight(board).getDown(board).getDown(board) != null){
                    if(cur.getRight(board).getDown(board).getDown(board).piece != null){
                        if(cur.getRight(board).getDown(board).getDown(board).piece.color != this.color){
                            ls.Add(cur.getRight(board).getDown(board).getDown(board));//attack stop search
                            attack = true;
                        }
                    } else {
                        ls.Add(cur.getRight(board).getDown(board).getDown(board));//no piece on square so move.
                    }
                }
            }
        }
        //left, up, up
        if(cur.getLeft(board) != null){
            if(cur.getLeft(board).getUp(board) != null){
                if(cur.getLeft(board).getUp(board).getUp(board) != null){
                    if(cur.getLeft(board).getUp(board).getUp(board).piece != null){
                        if(cur.getLeft(board).getUp(board).getUp(board).piece.color != this.color){
                            ls.Add(cur.getLeft(board).getUp(board).getUp(board));//attack stop search
                            attack = true;
                        }
                    } else {
                        ls.Add(cur.getLeft(board).getUp(board).getUp(board));//no piece on square so move.
                    }
                }
            }
        }
        //left, down, down
        if(cur.getLeft(board) != null){
                if(cur.getLeft(board).getDown(board) != null){
                    if(cur.getLeft(board).getDown(board).getDown(board) != null){
                        if(cur.getLeft(board).getDown(board).getDown(board).piece != null){
                            if(cur.getLeft(board).getDown(board).getDown(board).piece.color != this.color){
                                ls.Add(cur.getLeft(board).getDown(board).getDown(board));//attack stop search
                                attack = true;
                            }
                        } else {
                            ls.Add(cur.getLeft(board).getDown(board).getDown(board));//no piece on square so move.
                    }
                }
            }
        }
        //right, right, up
        if(cur.getRight(board) != null){
            if(cur.getRight(board).getRight(board) != null){
                if(cur.getRight(board).getRight(board).getUp(board) != null){
                    if(cur.getRight(board).getRight(board).getUp(board).piece != null){
                        if(cur.getRight(board).getRight(board).getUp(board).piece.color != this.color){
                            ls.Add(cur.getRight(board).getRight(board).getUp(board));//attack stop search
                            attack = true;
                        }
                    } else {
                        ls.Add(cur.getRight(board).getRight(board).getUp(board));//no piece on square so move.
                    }
                }
            }
        }
        //right right down
        if(cur.getRight(board) != null){
            if(cur.getRight(board).getRight(board) != null){
                if(cur.getRight(board).getRight(board).getDown(board) != null){
                    if(cur.getRight(board).getRight(board).getDown(board).piece != null){
                        if(cur.getRight(board).getRight(board).getDown(board).piece.color != this.color){
                            ls.Add(cur.getRight(board).getRight(board).getDown(board));//attack stop search
                            attack = true;
                        }
                    } else {
                        ls.Add(cur.getRight(board).getRight(board).getDown(board));//no piece on square so move.
                    }
                }
            }
        }
        //left left down
        if(cur.getLeft(board) != null){
            if(cur.getLeft(board).getLeft(board) != null){
                if(cur.getLeft(board).getLeft(board).getDown(board) != null){
                    if(cur.getLeft(board).getLeft(board).getDown(board).piece != null){
                        if(cur.getLeft(board).getLeft(board).getDown(board).piece.color != this.color){
                            ls.Add(cur.getLeft(board).getLeft(board).getDown(board));//attack stop search
                            attack = true;
                        }
                    } else {
                        ls.Add(cur.getLeft(board).getLeft(board).getDown(board));//no piece on square so move.
                    }
                }
            }
        }
        //left left up
        if(cur.getLeft(board) != null){
            if(cur.getLeft(board).getLeft(board) != null){
                if(cur.getLeft(board).getLeft(board).getUp(board) != null){
                    if(cur.getLeft(board).getLeft(board).getUp(board).piece != null){
                        if(cur.getLeft(board).getLeft(board).getUp(board).piece.color != this.color){
                            ls.Add(cur.getLeft(board).getLeft(board).getUp(board));//attack stop search
                            attack = true;
                        }
                    } else {
                        ls.Add(cur.getLeft(board).getLeft(board).getUp(board));//no piece on square so move.
                    }
                }
            }
        }
        return ls;
    }
    
}

public class Bishop : Piece{
    public Bishop(string color){
        name = "Bishop";
        symbol = 'B';
        this.color = color;
        if(color == "white"){
            objPrefab = Resources.Load("BishopLight") as GameObject;
        } else{
            objPrefab = Resources.Load("BishopDark") as GameObject;
        }
        //this.column = column;
        //this.row = row;
    }

    public override List<Square> move(Board board){ // pass it  'this' from board
        List<Square> ls = new List<Square>();
        //Bishops move in diagonals on the board. 

        //move until you find a piece 
        //if piece, based on color/piece , attack / check
        //up right
        Square cur = board.get("" + this.column + this.row);
        bool attack = false;
        while(cur.getTopRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getTopRight(board).piece != null){
                if(cur.getTopRight(board).piece.color != this.color){
                    ls.Add(cur.getTopRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getTopRight(board)); // if no piece in way and square, add
            }
            cur = cur.getTopRight(board);
        }
        //up left
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getTopLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getTopLeft(board).piece != null){
                if(cur.getTopLeft(board).piece.color != this.color){
                    ls.Add(cur.getTopLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getTopLeft(board)); // if no piece in way and square, add
            }
            cur = cur.getTopLeft(board);
        }
        //down left
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getDownLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDownLeft(board).piece != null){
                if(cur.getDownLeft(board).piece.color != this.color){
                    ls.Add(cur.getDownLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDownLeft(board)); // if no piece in way and square, add
            }
            cur = cur.getDownLeft(board);
        }
        //down right
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getDownRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDownRight(board).piece != null){
                if(cur.getDownRight(board).piece.color != this.color){
                    ls.Add(cur.getDownRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDownRight(board)); // if no piece in way and square, add
            }
            cur = cur.getDownRight(board);
        }
        return ls;
    }
}

public class Rook : Piece{
    public Rook(string color){
        name = "Rook";
        symbol = 'R';
        this.color = color;
        if(color == "white"){
            objPrefab = Resources.Load("RookLight") as GameObject;
        } else{
            objPrefab = Resources.Load("RookDark") as GameObject;
        }
        //this.column = column;
        //this.row = row;
    }
   public override List<Square> move(Board board){ // pass it  'this' from board
        List<Square> ls = new List<Square>();
        //Rooks move in left,right,up,down on the board. 
        //move until you find a piece 
        //if piece, based on color/piece , attack / check
        //up right
        Square cur = board.get("" + this.column + this.row);
        bool attack = false;
        //right
        while(cur.getRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getRight(board).piece != null){
                if(cur.getRight(board).piece.color != this.color){
                    ls.Add(cur.getRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getRight(board)); // if no piece in way and square, add
            }
            cur = cur.getRight(board);
        }
        //left
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getLeft(board).piece != null){
                if(cur.getLeft(board).piece.color != this.color){
                    ls.Add(cur.getLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getLeft(board)); // if no piece in way and square, add
            }
            cur = cur.getLeft(board);
        }
        //up
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getUp(board) != null && attack == false){
            //logic for piece attack
            if(cur.getUp(board).piece != null){
                if(cur.getUp(board).piece.color != this.color){
                    ls.Add(cur.getUp(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getUp(board)); // if no piece in way and square, add
            }
            cur = cur.getUp(board);
        }
        //down
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getDown(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDown(board).piece != null){
                if(cur.getDown(board).piece.color != this.color){
                    ls.Add(cur.getDown(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDown(board)); // if no piece in way and square, add
            }
            cur = cur.getDown(board);
        }
        return ls;
    }
}

public class Queen : Piece{
    public Queen(string color){
        name = "Queen";
        symbol = 'Q';
        this.color = color;
        if(color == "white"){
            objPrefab = Resources.Load("QueenLight") as GameObject;
        } else{
            objPrefab = Resources.Load("QueenDark") as GameObject;
        }
        objPrefab.transform.localScale += new Vector3(.05f, .05f, .05f);
        //this.column = column;
        //this.row = row;
    }

    public override List<Square> move(Board board){ // pass it  'this' from board
        List<Square> ls = new List<Square>();
        //Bishops move in diagonals on the board. 

        //move until you find a piece 
        //if piece, based on color/piece , attack / check
        //up right
        Square cur = board.get("" + this.column + this.row);
        bool attack = false;
        while(cur.getTopRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getTopRight(board).piece != null){
                if(cur.getTopRight(board).piece.color != this.color){
                    ls.Add(cur.getTopRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getTopRight(board)); // if no piece in way and square, add
            }
            cur = cur.getTopRight(board);
        }
        //up left
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getTopLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getTopLeft(board).piece != null){
                if(cur.getTopLeft(board).piece.color != this.color){
                    ls.Add(cur.getTopLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getTopLeft(board)); // if no piece in way and square, add
            }
            cur = cur.getTopLeft(board);
        }
        //down left
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getDownLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDownLeft(board).piece != null){
                if(cur.getDownLeft(board).piece.color != this.color){
                    ls.Add(cur.getDownLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDownLeft(board)); // if no piece in way and square, add
            }
            cur = cur.getDownLeft(board);
        }
        //down right
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getDownRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDownRight(board).piece != null){
                if(cur.getDownRight(board).piece.color != this.color){
                    ls.Add(cur.getDownRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDownRight(board)); // if no piece in way and square, add
            }
            cur = cur.getDownRight(board);
        }
        //right
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getRight(board).piece != null){
                if(cur.getRight(board).piece.color != this.color){
                    ls.Add(cur.getRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getRight(board)); // if no piece in way and square, add
            }
            cur = cur.getRight(board);
        }
        //left
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getLeft(board).piece != null){
                if(cur.getLeft(board).piece.color != this.color){
                    ls.Add(cur.getLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getLeft(board)); // if no piece in way and square, add
            }
            cur = cur.getLeft(board);
        }
        //up
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getUp(board) != null && attack == false){
            //logic for piece attack
            if(cur.getUp(board).piece != null){
                if(cur.getUp(board).piece.color != this.color){
                    ls.Add(cur.getUp(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getUp(board)); // if no piece in way and square, add
            }
            cur = cur.getUp(board);
        }
        //down
        cur = board.get("" + this.column + this.row);
        attack = false;
        while(cur.getDown(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDown(board).piece != null){
                if(cur.getDown(board).piece.color != this.color){
                    ls.Add(cur.getDown(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDown(board)); // if no piece in way and square, add
            }
            cur = cur.getDown(board);
        }
        return ls;
    }
}

public class King : Piece{
    public King(string color){
        name = "King";
        symbol = 'K';
        this.color = color;
        if(color == "white"){
            objPrefab = Resources.Load("KingLight") as GameObject;
        } else{
            objPrefab = Resources.Load("KingDark") as GameObject;
        }
        //this.column = column;
        //this.row = row;
    }


    public override List<Square> move(Board board){ // pass it  'this' from board
        List<Square> ls = new List<Square>();
        //Bishops move in diagonals on the board. 

        //move until you find a piece 
        //if piece, based on color/piece , attack / check
        //right
        Square cur = board.get("" + this.column + this.row); //get square piece is on
        bool attack = false;
        if(cur.getRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getRight(board).piece != null){
                if(cur.getRight(board).piece.color != this.color){
                    ls.Add(cur.getRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getRight(board)); // if no piece in way and square, add
            }
        }
        //left
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getLeft(board).piece != null){
                if(cur.getLeft(board).piece.color != this.color){
                    ls.Add(cur.getLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getLeft(board)); // if no piece in way and square, add
            }
        }
    //up
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getUp(board) != null && attack == false){
            //logic for piece attack
            if(cur.getUp(board).piece != null){
                if(cur.getUp(board).piece.color != this.color){
                    ls.Add(cur.getUp(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getUp(board)); // if no piece in way and square, add
            }
        }
        //down
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getDown(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDown(board).piece != null){
                if(cur.getDown(board).piece.color != this.color){
                    ls.Add(cur.getDown(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDown(board)); // if no piece in way and square, add
            }
        }
        //topRight
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getTopRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getTopRight(board).piece != null){
                if(cur.getTopRight(board).piece.color != this.color){
                    ls.Add(cur.getTopRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getTopRight(board)); // if no piece in way and square, add
            }
        }
        //topLeft
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getTopLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getTopLeft(board).piece != null){
                if(cur.getTopLeft(board).piece.color != this.color){
                    ls.Add(cur.getTopLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getTopLeft(board)); // if no piece in way and square, add
            }
        }
        //downRight
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getDownRight(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDownRight(board).piece != null){
                if(cur.getDownRight(board).piece.color != this.color){
                    ls.Add(cur.getDownRight(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDownRight(board)); // if no piece in way and square, add
            }
        }
        //downLeft
        cur = board.get("" + this.column + this.row);
        attack = false;
        if(cur.getDownLeft(board) != null && attack == false){
            //logic for piece attack
            if(cur.getDownLeft(board).piece != null){
                if(cur.getDownLeft(board).piece.color != this.color){
                    ls.Add(cur.getDownLeft(board));//attack stop search
                    attack = true;
                } else {
                    attack = true;//piece if our color, no attack (don't add move)
                }
            } else {
                ls.Add(cur.getDownLeft(board)); // if no piece in way and square, add
            }
        }
        return ls;
    }

}



//CONVErT CHAR TO STRING OF COLUMN EVERYWHERE SO IT CAN BE NULL


//MAKE NULL ' '
//NULL IS EQUIVALENT TO ' '
//SO NOT VALID

public class Square{
    public char column = 'z';
    public int row = 0;
    public string color = "";
    public Piece piece = null;
    public Square(char column = 'z', int row = 9, string color = "", Piece piece = null){
        this.column = column;
        this.row = row;
        this.color = color;
        this.piece = piece;
    }
    
    public string name(){
        string name = "";
        name += column;
        name += row;
        return name;
    }

    public void print(){
        Debug.Log("" + column + row.ToString());
    }




    public Square getRight(Board board){
        string key = "";
        Square move = new Square();
        if((int)this.column - 65 < 7 && (int)this.column - 65 >= 0){ //if column is h then false if columnb = a-g will work /true   a - 65 = 0
            move.row = this.row; //this square row is number
            key+=(char)((int)this.column+1); //column is int increases and converted to char
            key+=move.row;
            move = board.get(key);
        } else {
            move = null;
        }
        return move;
    }
    public Square getLeft(Board board){
        string key = "";
        Square move = new Square();
        if((int)this.column - 65 < 8 && (int)this.column - 65 > 0){ // if column is a then false. if column b-h will work
            move.row = this.row; //this square row is number
            key+=(char)((int)this.column-1); //column is int increases and converted to char
            key+=move.row;
            move = board.get(key);
        } else {
            move = null;
        }
        return move;
    }
    public Square getUp(Board board){
        string key = "";
        Square move = new Square();
        if(this.row + 1 <= 8 && this.row >= 0){
            move.row = this.row + 1;
            key+=this.column;
            key+=move.row;
            move = board.get(key);
        } else {
            move = null;
        }
        return move;
    }
    public Square getDown(Board board){
        string key = "";
        Square move = new Square();
        if(this.row <= 8 && this.row - 1 > 0){
            move.row = this.row - 1;
            key+=this.column;
            key+=move.row;
            move = board.get(key);
        } else {
            move = null;
        }
        return move; 
    }
    public Square getTopRight(Board board){
        Square square = new Square();
        if(getRight(board) != null){
           if(getRight(board).getUp(board) != null){
                square = getRight(board).getUp(board);
           } else {
               square = null;
           }
        } else {
            square = null;
        }
        return square;
    }
    public Square getTopLeft(Board board){
        Square square = new Square();
        if(getLeft(board) != null){
           if(getLeft(board).getUp(board) != null){
                square = getLeft(board).getUp(board);
           } else {
               square = null;
           }
        } else {
            square = null;
        }
        return square;   
         }
    public Square getDownRight(Board board){
        Square square = new Square();
        if(getRight(board) != null){
           if(getRight(board).getDown(board) != null){
                square = getRight(board).getDown(board);
           } else {
               square = null;
           }
        } else {
            square = null;
        }
        return square;  
        }
    public Square getDownLeft(Board board){
        Square square = new Square();
        if(getLeft(board) != null){
           if(getLeft(board).getDown(board) != null){
                square = getLeft(board).getDown(board);
           } else {
               square = null;
           }
        } else {
            square = null;
        }
        return square;  
        }


}

public class Board {
    public int turn = 1; // if 0 then whites turn
    public int check = 0;
    public string color = "red";
    public Square[,] board = new Square[8, 8];


    public string playerTurn()
    {
        // determine who is moving
        string color = "white";
        if (turn % 2 == 0)
        {
            color = "black";
        }
        return color;
    }

    public Board() {
        // Initialize each 
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                
                //each square is assigned a row value of 1 - 8
                //and a column of A-H
                if((i*8 + j + i) % 2 == 0){
                    board[i,j] = new Square((char)(j + 65), 8 - i, "white");
                } else {
                    board[i,j] = new Square((char)(j + 65), 8 - i, "black");
                }
            }
        }
        
    }

    //copy constructor
    public Board(Board Originalboard){
        turn = Originalboard.turn;
        // Initialize each 
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                //each square is assigned a row value of 1 - 8
                //and a column of A-H
                if((i*8 + j + i) % 2 == 0){
                    board[i,j] = new Square((char)(j + 65), 8 - i, "white");
                } else {
                    board[i,j] = new Square((char)(j + 65), 8 - i, "black");
                }
            }
        }

        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if(Originalboard.board[i, j].piece != null){
                    if(Originalboard.board[i, j].piece.symbol == 'P'){
                        board[i, j].piece = new Pawn(Originalboard.board[i, j].piece.color);
                        board[i, j].piece.moved = Originalboard.board[i, j].piece.moved;
                    } else if (Originalboard.board[i, j].piece.symbol == 'N'){
                        board[i, j].piece = new Knight(Originalboard.board[i, j].piece.color);
                        board[i, j].piece.moved = Originalboard.board[i, j].piece.moved;
                    } else if (Originalboard.board[i, j].piece.symbol == 'B'){
                        board[i, j].piece = new Bishop(Originalboard.board[i, j].piece.color);
                        board[i, j].piece.moved = Originalboard.board[i, j].piece.moved;
                    } else if (Originalboard.board[i, j].piece.symbol == 'R'){
                        board[i, j].piece = new Rook(Originalboard.board[i, j].piece.color);
                        board[i, j].piece.moved = Originalboard.board[i, j].piece.moved;
                    } else if (Originalboard.board[i, j].piece.symbol == 'Q'){
                        board[i, j].piece = new Queen(Originalboard.board[i, j].piece.color);
                        board[i, j].piece.moved = Originalboard.board[i, j].piece.moved;
                    } else if (Originalboard.board[i, j].piece.symbol == 'K'){
                        board[i, j].piece = new King(Originalboard.board[i, j].piece.color);
                        board[i, j].piece.moved = Originalboard.board[i, j].piece.moved;
                    }
                }
            }
        }
        setPositions(); //assign every piece it's column and row
    }

    //based on the state of the game, set the peices on the board
    /**
    public void setPieces(){
        setPositions(); // all pieces assigned their position
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if(board[i,j].piece != null){
                    Destroy(board[i,j].piece.obj);//remove piece from square.
                    //ind square in game
                    string col = "" + (char)(j + 65);
                    string ro =  "" + (8 - i);
                    //what piece is on the square?
                    Piece pie = get(col + ro).piece; //gets piece on square
                    GameObject obj = pie.getPrefab() as GameObject;
                    obj.transform.localScale = new Vector3(50.0f, 50.0f, 50.0f);
                    Vector3 pos = GameObject.Find(col + ro).transform.position;
                    obj.transform.position = new Vector3(pos.x,pos.y,pos.z);
                     // Instantiate at position (0, 0, 0) and zero rotation.
                    board[i,j].piece.obj = Instantiate(obj, pos, Quaternion.identity); // get king
                }
            }
        }
    }
    **/

    //sets board for start of game
    public void newGame(){
        string color = "black";
        //set black
        //set black rooks
        board[0, 0].piece = new Rook(color);
        board[0, 7].piece = new Rook(color);
        //setcolorknights
        board[0, 1].piece = new Knight(color);
        board[0, 6].piece = new Knight(color);
        //setcolorbishops
        board[0, 2].piece = new Bishop(color);
        board[0, 5].piece = new Bishop(color);
        //setcolorqueen
        board[0, 3].piece = new Queen(color);
        //setcolorking
        board[0, 4].piece = new King(color);
        //setcolor pawns
        for(int i = 0; i < 8; i++){
            board[1, i].piece = new Pawn(color);
        }
        color = "white";
        //set white
        //set white rooks
        board[7, 0].piece = new Rook(color);
        board[7, 7].piece = new Rook(color);
        //set white knights
        board[7, 1].piece = new Knight(color);
        board[7, 6].piece = new Knight(color);
        //set white bishops
        board[7, 2].piece = new Bishop(color);
        board[7, 5].piece = new Bishop(color);

        //set white queen
        board[7, 3].piece = new Queen(color);
        //set white king
        board[7, 4].piece = new King(color);
        //set white pawns
        for(int i = 0; i < 8; i++){
            board[6, i].piece = new Pawn(color);
        }

        setPositions();
    }

    //every piece on the board is assign its column and row
    public void setPositions(){
        //set every pieces pos attributes
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if (board[i, j].piece != null) {
                    board[i, j].piece.column = board[i, j].column;
                    board[i, j].piece.row = board[i, j].row;
                }
            }
        }
    }

    public void print(){
        string bo = "";
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if(board[i,j].piece != null){
                    bo += board[i,j].piece.symbol;
                } else {
                    bo += 'O';
                }
            }
                bo += '\n';
        }
        Debug.Log(bo);
    }

    //give command in notation 'xE4'
    public void testTest(string command){
        //Board copyBoard = new Board(this); // create copy of board
        Board copyBoard = new Board(this);
        //copyBoard = new Board(this);
        Piece movingPiece = new Piece(); // piece that is being moved
        Square movingTo = new Square(); // based on input movingTo is assigned
        Square movingFrom = new Square(); // based on input movingFrom is assigned
        int count = 0; // how many pieces can make the move

        // determine who is moving
        string color = "white";
        if(turn % 2 == 0){
            color = "black";
        }

        List<Piece> pieces = new List<Piece>();
        //get all pieces the command is wanting to move 'Bc3' - 'B' for bishop.. ect.
        char moving = command[0]; 
        if(moving == 'x' || command.Length == 2){ // a pawn is moved
            pieces = copyBoard.getPieces('P', color); //get pieces from copyboard
                foreach(Piece piece in pieces){
                    List<Square> squares = piece.move(this);
                    if(squares != null){ // if not null then a pawn that can make the move exists
                        if(command[0] != 'x'){ // a pawn is taking  xE3  xC1
                            if(squares.Contains(get("" + command[0] + command[1]))){
                                count++; // found a pawn that can make the move
                                movingPiece = piece;
                                //move
                                movingTo = copyBoard.get("" + command[0] + command[1]); // assign square moving to
                                movingFrom = copyBoard.get("" + movingPiece.column + movingPiece.row); // assign square moving from
                                
                                // movingPiece.moved = true; // mark piece as moved
                                // movingTo.piece = movingPiece; // piece moves to square
                                // movingFrom.piece = null; // piece is moved so remove it from previous square
                            }
                        } else { // if command doesn't start with x and length is 2 ( a pawn is moving ) E3   C1
                            if(squares.Contains(get("" + command[1] + command[2]))){
                                count++; // found a pawn that can make the move
                                movingPiece = piece;

                                //move
                                movingTo = copyBoard.get("" + command[1] + command[2]); // assign square moving to
                                movingFrom = copyBoard.get("" + movingPiece.column + movingPiece.row); // assign square moving from
                                
                                //removed moving **
                            }
                        }
                    }
                }
        } else if (false){
         //issue with having only uppercase
         //have to have lowercase to know the difference between b (column) and B bishop
         //GxF7 ExF7

        } else { //use get pieces to find the pieces trying to move
            //not a pawn, other piece is moving
            pieces = getPieces(moving, color); // the first character (command[0] .. moving) must be a piece or a castle attempt
            if(command[1] == 'x'){ //attack like BxE7 KxE1
            //one piece attacks


            } else if(command[2] == 'x'){ // attack like R3xE1   RGxE1
                                          // the second character acts as indicator
                                          //find the rook that have this as their column/row
            //if we find more than one of the same type of piece that can move to the square then new command ********
            //if we count above 1 piece that can make the move then we can't take it
            } else { //moving ( command[1] and [2] should be square) KE3  RE2
                //are they in the pieces move list?
                //check all pieces that can move to the square
                foreach(Piece piece in pieces){
                    List<Square> squares = piece.move(this);
                    if(squares != null){
                        if(squares.Contains(get("" + command[1] + command[2]))){
                            count+=1;
                            movingPiece = piece;

                            //move
                            movingTo = copyBoard.get("" + command[1] + command[2]);
                            movingFrom = copyBoard.get("" + movingPiece.column + movingPiece.row);
                            // movingTo.piece = movingPiece;
                            // movingFrom.piece = null;
                        }
                    }
                }
                //get("" + command[0] + command[2]).    
            }
        }
        //ABOVE IS FINDING THE SQUARES AND PIECE THAT IS MOVING
        //TEST THE LEGALITY OF MOVE
        //use copy of board
        movingPiece.moved = true; // mark piece as moved
        if(movingPiece.symbol == 'P'){
            movingPiece.justMoved = true; // for em passant
        }
        movingTo.piece = movingPiece; // piece moves to square
        movingFrom.piece = null; // piece is moved so remove it from previous square
        copyBoard.setPositions(); //set every pieces column and row
        copyBoard.turn++;
        int check = copyBoard.checkChecker(); //checkChecker (if player who moved is in check then illegal)
        
        //Debug.Log("Check number: " + check);
        if(check == 0){
            this.board = copyBoard.board;//legal make the move on the real board
        } else if ( check == -1 ){ // black check
            if(color == "black"){
                //Debug.Log("illegal");//illegal
            } else {
                this.board = copyBoard.board;//legal make the move on the real board
            }
        } else if ( check == 1 ){
            if(color == "white"){
                Debug.Log("illegal"); //illegal
            } else {
                this.board = copyBoard.board;//legal make the move on the real board
            }
        }
        //Debug.Log(count); //number of pieces that can make the move
        turn++;
        setPositions(); //set every pieces column and row appropriately
        Debug.Log(movingPiece.name + movingPiece.column + movingPiece.row + movingPiece.moved);
        Debug.Log(this.get("" +  movingPiece.column + movingPiece.row).piece.moved);
        Debug.Log(copyBoard.get("" +  movingPiece.column + movingPiece.row).piece.moved);
        //return this; // return modified board ( RIGHT NOW THIS MODIFIES THE ACTUAL BOARD so take a copy of it before using this function)
    }

    public List<Piece> getPieces(char symbol, string color){
        List<Piece> pieces = new List<Piece>();
            for(int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(board[i, j].piece != null){
                        if(board[i, j].piece.color == color && board[i,j].piece.symbol == symbol){//get all pawns
                            pieces.Add(board[i,j].piece);
                        }
                    }
                }
            }
        foreach(Piece piece in pieces){
            //Debug.Log(piece.name);
        }
        return pieces;
    }
    //pass square you want

    //more effective way of getting square is converting
    //the pos provided to index
    public Square get(string pos){ 
        //get column
        char column = pos[0];
        //get row
        int row = (int)char.GetNumericValue(pos[1]); // convert string to int
        bool found = false;
        Square square = null;
        //add a check for pos a-h 1-8
        //so we don't have an infinite loop
        int i = 0;
        while(!found){
            for(int j = 0; j < 8; j++){ //go
                if(column == board[i, j].column && row == board[i, j].row){
                    square = board[i, j];
                    found = true;
                }
            }
            i++;
        }
        return square;
    }

    public int checkChecker(){
        //determine whose turn it is
        string color = "white";
        if(turn % 2 == 0){
            color = "black";
        }

        int check = 0; // no check
                       // -1 for black
                       // 1 for white
                       // 2 for both (illegal)
        bool whiteCheck = false;
        bool blackCheck = false;

        List<Square> squares = new List<Square>(); // all squares to inspect
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                squares.Add(board[i, j]);
            }
        }

        //get king positions
        Square blackKing = new Square();
        Square whiteKing = new Square(); // these are reassigned in the next for loop
        foreach(Square square in squares){ // get kings squares
            if(square.piece != null){
                if(square.piece.symbol == 'K'){
                    //Debug.Log("" + square.piece.color + square.piece.column + square.piece.row);
                    if(square.piece.color == "black"){
                        blackKing = square;
                    } else {
                        whiteKing = square;


                    }

                }
            }
        }

        //checks if any piece can attack King
        foreach(Square square in squares){
            if(square.piece != null){
                Piece attacker = square.piece;
                List<Square> moves = attacker.move(this); //get move list for piece
                if(attacker.color == "black"){
                    if(moves.Contains(whiteKing)){ //if piece can move to whiteKings square
                        //Debug.Log("Attack on the white king");

                        whiteCheck = true;
                        
                    }
                } else { //attacker is white
                    if(moves.Contains(blackKing)){ //if piece can move to blackKings square
                        //Debug.Log("Attack on the black king"); 


                       
                        blackCheck = true;
                    } 
                }
            }
        }
        if(blackCheck && whiteCheck){
            check = 2;
        } else if (blackCheck){
            check = -1;
        } else if (whiteCheck){
            check = 1;
        }
        return check;
    }



    //return bool : if move is legal and sucessfully completed
    public bool movePiece(string pos){
        return true;
    }

    //PASS THE PIECE YOU WANT TO MOVE
    //FIND SQUARE IT IS ON
    //FIND PATH BY CALLING PIECES MOVE FUNCTION


}

// Controller

//VIEW
public class View : MonoBehaviour{
    public Board model = new Board();
    public List<GameObject> pieces = new List<GameObject>();

    public View(){
        setPieces();
    }

    public void createView(){

    }

    public Quaternion rotate = Quaternion.Euler(0,0,0);
    //based on the state of the game, set the peices on the board
    public void setPieces(){
        clearPieces();
        model.setPositions(); // all pieces assigned their position
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if(model.board[i,j].piece != null){
                    Destroy(model.board[i,j].piece.obj);//remove piece from square.
                    //ind square in game
                    string col = "" + (char)(j + 65);
                    string ro =  "" + (8 - i);
                    //what piece is on the square?
                    Piece pie = model.get(col + ro).piece; //gets piece on square
                    GameObject obj = pie.getPrefab() as GameObject;
                    obj.transform.localScale = new Vector3(50.0f, 50.0f, 50.0f);
                    if (pie.symbol == 'N')
                    {
                        if (pie.color == "black")
                            rotate = Quaternion.Euler(0, 180, 0);
                        else
                            rotate = Quaternion.Euler(0, 0, 0);
                    }
                    Vector3 pos = GameObject.Find(col + ro).transform.position;
                    obj.transform.position = new Vector3(pos.x,pos.y,pos.z);
                    model.board[i, j].piece.objPrefab.name = ("" + model.board[i, j].piece.symbol + model.board[i, j].column + model.board[i, j].row);
                    // Instantiate at position (0, 0, 0) and zero rotation.
                    model.board[i,j].piece.obj = Instantiate(obj, pos, rotate);
       

                    //LET VIEWER HAVE CONTROL OF ALL PIECES IT SPAWNS
                    //GIVE IT A HANDLE TO ALL OF THEM
                    pieces.Add(model.board[i,j].piece.obj);
                }
            }
        }
    }

    

    public void setPieces1()
    {
        clearPieces();
        model.setPositions(); // all pieces assigned their position
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (model.board[i, j].piece != null)
                {
                    Destroy(model.board[i, j].piece.obj);//remove piece from square.
                    //find square in game
                    string col = "" + (char)(j + 65);
                    string ro = "" + (8 - i);
                    //what piece is on the square?
                    Piece pie = model.get(col + ro).piece; //gets piece on square
                    GameObject obj = pie.getPrefab() as GameObject;
                    obj.transform.localScale = new Vector3(750.0f, 750.0f, 750.0f);
                    if (pie.symbol == 'N')
                    {
                        if(pie.color == "black")
                            rotate = Quaternion.Euler(0, -90, 0);
                        else
                            rotate = Quaternion.Euler(0, 90, 0);
                    }
                    Vector3 pos = GameObject.Find(col + ro).transform.position;
                    obj.transform.position = new Vector3(pos.x, pos.y, pos.z);
                    
                    model.board[i, j].piece.objPrefab.name = ("" + model.board[i, j].piece.symbol + model.board[i, j].column + model.board[i, j].row);
                    // Instantiate at position (0, 0, 0) and zero rotation.
                    model.board[i, j].piece.obj = Instantiate(obj, pos, rotate);
                    //LET VIEWER HAVE CONTROL OF ALL PIECES IT SPAWNS
                    //GIVE IT A HANDLE TO ALL OF THEM
                    pieces.Add(model.board[i, j].piece.obj);
                }
            }
        }
    }

    public void clearPieces(){
        foreach(GameObject piece in pieces){
            Destroy(piece);
        }
    }
}

public class Chess : MonoBehaviour
{

   
    //Board board = new Board();
    // Start is called before the first frame update
    public Board board;
    public View view;
    void Start()
    {
        board = new Board();
        view = gameObject.AddComponent(typeof(View)) as View;
        view.model = board;
        view.model.newGame();
        
        //check test
        /**
        view.model.get("E2").piece.makeMove(view.model.get("E4"), view.model);
        view.model.get("D7").piece.makeMove(view.model.get("D6"), view.model);
        view.model.get("F1").piece.makeMove(view.model.get("B5"), view.model);
        //view.model.get("C7").piece.makeMove(view.model.get("C5"), view.model); // won't allow
                                                                               // invalid
        view.model.get("C7").piece.makeMove(view.model.get("C6"), view.model);
        view.model.get("B5").piece.makeMove(view.model.get("C6"), view.model);

        //print legal moves for player at this point

        **/

        /**
        view.model.get("E2").piece.makeMove(view.model.get("E4"), view.model);
        view.model.get("A7").piece.makeMove(view.model.get("A5"), view.model);
        view.model.get("E4").piece.makeMove(view.model.get("E5"), view.model);
        view.model.get("D7").piece.makeMove(view.model.get("D5"), view.model);
        view.model.get("E5").piece.makeMove(view.model.get("D6"), view.model);
        **/



        view.setPieces();

        /**
         //public int counter = 0;
         //Board board = new Board();
         board.newGame(); // sets pieces for start of standard game
         //Debug.Log(board.get("E8").piece.name);
         //Debug.Log(board.get("E8").piece.color);
         foreach(Square square in board.get("B1").piece.move(board)){
            square.print(); // print each move for piece on C1 (Bishop)
         }
         //board.testTest("BB5");
         //Destroy(board.get("B1").piece.obj);//remove piece from square.
         board.checkChecker();
         //board.testTest("NF3");
         //board.testTest("E5");
         //board.testTest("C6");
         // board.testTest("NC3");
         // board.testTest("D5");
         // board.testTest("xD5");
         // board.testTest("QH5");
         // board.testTest("D7");


         board.setPieces();
         //board.testTest("NC3");
         Debug.Log("TESTEST");
         foreach(Square square in board.get("C7").piece.move(board)){
             square.print();
         }
         **/
    }

    // Update is called once per frame
    int counter = 0;
    //illegal moves
    //string[] moveList = {"A7", "BE6"};
    //string[] moveList = {"C4", "E5", "NC3"};
    //game of the century
    //string[] moveList = {"NF3", "NF6", "C4", "G6", "NC3", "BG7", "D4", "0-0"};
    void Update()
    {
        // counter++;
        // if(counter % 240 == 0){
        //     board.testTest(moveList[counter/240 - 1]);
        //     view.setPieces();
        //     if(counter / 240 == 1){
        //     }
        // }
    }
}
