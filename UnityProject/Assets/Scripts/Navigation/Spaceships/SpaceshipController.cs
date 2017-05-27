using UnityEngine;
using GalacticWanderer.Managers;

public class SpaceshipController : MonoBehaviour
{
	private Rigidbody rb;

	public float rotationSpeed;
	public float acceleration;
    public float boostAcceleration;

    public Transform InsideSpaceShip;
    public Transform OutsideSpaceShip;
    public GameObject _particle1;
	public GameObject _particle2;

    public bool engineEnabled;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void Update()
	{

	    if (Input.GetKeyUp(KeyCode.Space))
	    {
	        engineEnabled = !engineEnabled;
	        ToggleEngine(engineEnabled);
	    }

	    float currentAcceleration;
	    if (Input.GetKey(KeyCode.LeftShift))
	    {
	        currentAcceleration = boostAcceleration;
	    }
	    else
	    {
	        currentAcceleration = acceleration;
	    }

		if (Input.GetKey(KeyCode.Z))
		{ // Accélérer
			rb.AddForce(transform.forward * currentAcceleration);
		}
		if (Input.GetKey(KeyCode.S))
		{ // Freiner
			rb.AddForce(-transform.forward * currentAcceleration);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{ // Plonger
			rb.AddTorque(transform.right * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{ // Redresser
			rb.AddTorque(-transform.right * rotationSpeed);
		}

		if (Input.GetKey(KeyCode.Q))
		{ // Tourner à gauche 
			rb.AddTorque(-transform.up * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.D))
		{ // Tourner à droite
			rb.AddTorque(transform.up * rotationSpeed);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{ // Pencher à gauche 
			rb.AddTorque(transform.forward * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{ // Pencher à droite
			rb.AddTorque(-transform.forward * rotationSpeed);
		}
        if(Input.GetKeyUp(KeyCode.E))
        { // retourner dans le vaisseau
			GameManager.instance.SetPhase(GamePhase.InsideShip);
			/*
            InsideSpaceShip.gameObject.SetActive(true);
            OutsideSpaceShip.gameObject.SetActive(false);
			*/
        }
	}

    void ToggleEngine(bool toggle)
    {
        rb.useGravity = !toggle;
        _particle1.SetActive(toggle);
        _particle2.SetActive(toggle);
    }
}
