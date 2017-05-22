using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public Transform Body;
    public Transform Eye;
    public Transform MessageCanvas;
    public float speed = 2.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public UnityEngine.UI.Text textMessage;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private ActivableObject activable;
    private Transform Target;

    // Update is called once per frame
    void Update () {
        CharacterMovements();
        InitRayCast();
    }

    void CharacterMovements()
    {
        Vector3 move = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z))
        {
            move += Eye.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            move -= Eye.forward;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            move += Eye.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
        {
            move -= Eye.right;
        }
        move.y = 0;
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        Eye.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        Body.position += Vector3.Normalize(move) * speed * Time.deltaTime;
    }
    void InitRayCast()
    {
        MessageCanvas.GetComponent<UnityEngine.UI.Text>().text = null;
        var hit = new RaycastHit();
        if (Physics.Raycast(Eye.position, Eye.forward, out hit, 1.5f))
        {
            Target = hit.transform;
            activable = Target.GetComponent<ActivableObject>();
            if (activable!=null)
            {
                Activable(Target);
            }
        }
        else
        {
            Target = null;
        }
    }

    void Activable(Transform T)
    {
        MessageCanvas.gameObject.SetActive(true);
        MessageCanvas.GetComponent<UnityEngine.UI.Text>().text = activable.Message.text; 
        if(Input.GetKeyUp(KeyCode.E))
        {
            activable.Activate();
        }
    }
}