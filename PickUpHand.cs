using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickUpHand : MonoBehaviour
{
    public float PickupDistance = .3f;
    bool handClosed = false;
    public LayerMask pickupLayer;
    public List<GameObject> indicators = new List<GameObject>();
    public int count = 0;
    
    public SteamVR_Input_Sources handSource = SteamVR_Input_Sources.RightHand;

    Rigidbody holdingTarget;
    Rigidbody lastTarget;
    GameObject temp;
    public bool target = false;

    // Update is called once per frame
    void FixedUpdate()
    {

        if (SteamVR_Actions.default_Grab.GetState(handSource))
            handClosed = true;
        else
            handClosed = false;
        

        if (!handClosed)
        {
            if(target == true)
            {
                count = 0;
                foreach(GameObject Obj in indicators){
                    Destroy(Obj);
                }
                Board board = GameObject.Find("board").GetComponent<Chess>().board;
                View view = GameObject.Find("board").GetComponent<Chess>().view;
                Vector3 pos = lastTarget.transform.position;
                Square closest = new Square();
                float closestDis = 99999;
                float distance = 0;
                Vector3 dist;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        string col = "" + (char)(j + 65);
                        string ro = "" + (8 - i);
                        dist = GameObject.Find(col + ro).transform.position; //square
                        distance = Vector3.Distance(pos, dist);
                        if (closestDis > distance)
                        {
                            closestDis = distance;
                            closest = board.get("" + col + ro);
                        
                        }
                    }
                }
                string command = "" + lastTarget.gameObject.name[1] + lastTarget.gameObject.name[2];
                Square comingfrom = board.get(command);
                closest.print();
                comingfrom.piece.makeMove(closest, board);
                view.setPieces();
                board.print();
                Debug.Log("release");
                target = false;
            }
            Collider[] colliders = Physics.OverlapSphere(transform.position, PickupDistance, pickupLayer);
            if (colliders.Length > 0)
            {
                holdingTarget = colliders[0].transform.root.GetComponent<Rigidbody>();
                holdingTarget.constraints = ~RigidbodyConstraints.FreezePosition;
            }
            else
                holdingTarget = null;
        }
        else
        {
            if (holdingTarget)
            {
                target = true;
                lastTarget = holdingTarget;
                
                //moves object to hand
                holdingTarget.velocity = (transform.position - holdingTarget.transform.position) / Time.fixedDeltaTime;
                if(count == 0){
                    Board board = GameObject.Find("board").GetComponent<Chess>().board;
                    string command = "" + lastTarget.gameObject.name[1] + lastTarget.gameObject.name[2];
                    Square comingfrom = board.get(command);
                    List<Square> moves = comingfrom.piece.legalMoves(board);
                    foreach(Square square in moves){
                        Vector3 pos = GameObject.Find("" + square.column + square.row).transform.position;
                        GameObject model = Instantiate(GameObject.Find("Indicator"), pos, Quaternion.identity);
                        indicators.Add(model);
                    }
                }
                count++;

                //Controls rotation of object in hand
                holdingTarget.maxAngularVelocity = 20;
                Quaternion deltaRot = transform.rotation * Quaternion.Inverse(holdingTarget.transform.rotation);
                Vector3 eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));
                eulerRot *= .95f;
                eulerRot *= Mathf.Deg2Rad;
                holdingTarget.angularVelocity = eulerRot / Time.fixedDeltaTime;

            }
        }
    }


    


}
