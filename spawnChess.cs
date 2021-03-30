using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnChess : MonoBehaviour
{

    public Board board;
    public View view;
    public GameObject piece;
    // Start is called before the first frame update
    void Start()
    {
        board = new Board();
        view = gameObject.AddComponent(typeof(View)) as View;
        view.model = board;
        view.model.newGame();
        view.setPieces1();
        
    }
}