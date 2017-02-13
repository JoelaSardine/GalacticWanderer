using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBoardActivable : ActivableObject {

    public Transform InsideSpaceShip;
    public Transform OutsideSpaceShip;

    public override void Activate()
    {

        InsideSpaceShip.gameObject.SetActive(false);
        OutsideSpaceShip.gameObject.SetActive(true);
    }
}
