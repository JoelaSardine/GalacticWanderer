using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
	new Rigidbody rigidbody;

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
		rigidbody = GetComponent<Rigidbody>();
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
			rigidbody.AddForce(transform.forward * currentAcceleration);
		}
		if (Input.GetKey(KeyCode.S))
		{ // Freiner
			rigidbody.AddForce(-transform.forward * currentAcceleration);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{ // Plonger
			rigidbody.AddTorque(transform.right * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{ // Redresser
			rigidbody.AddTorque(-transform.right * rotationSpeed);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{ // Tourner à gauche 
			rigidbody.AddTorque(-transform.up * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{ // Tourner à droite
			rigidbody.AddTorque(transform.up * rotationSpeed);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{ // Pencher à gauche 
			rigidbody.AddTorque(transform.forward * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{ // Pencher à droite
			rigidbody.AddTorque(-transform.forward * rotationSpeed);
		}
        if(Input.GetKey(KeyCode.E))
        { // retourner dans le vaisseau
            InsideSpaceShip.gameObject.SetActive(true);
            OutsideSpaceShip.gameObject.SetActive(false);
        }
	}

    void ToggleEngine(bool toggle)
    {
        rigidbody.useGravity = !toggle;
        _particle1.SetActive(toggle);
        _particle2.SetActive(toggle);
    }
}
