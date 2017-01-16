using UnityEngine;
using System.Collections;

public class SpaceshipController : MonoBehaviour
{
	new Rigidbody rigidbody;

	public float rotationSpeed;
	public float acceleration;

	public GameObject _particle1;
	public GameObject _particle2;
	
	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}
	
	void Update()
	{
		if (Input.GetKey(KeyCode.A))
		{ // Démarrer les moteurs
			rigidbody.useGravity = false;
			_particle1.SetActive(true);
			_particle2.SetActive(true);
		}
		if (Input.GetKey(KeyCode.E))
		{ // Couper les moteurs
			rigidbody.useGravity = true;
			_particle1.SetActive(false);
			_particle2.SetActive(false);
		}

		if (Input.GetKey(KeyCode.Z))
		{ // Accélérer
			rigidbody.AddForce(transform.forward * acceleration);
		}
		if (Input.GetKey(KeyCode.S))
		{ // Freiner
			rigidbody.AddForce(-transform.forward * acceleration);
		}

		if (Input.GetKey(KeyCode.O))
		{ // Plonger
			rigidbody.AddTorque(transform.right * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.L))
		{ // Redresser
			rigidbody.AddTorque(-transform.right * rotationSpeed);
		}

		if (Input.GetKey(KeyCode.K))
		{ // Tourner à gauche 
			rigidbody.AddTorque(-transform.up * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.M))
		{ // Tourner à droite
			rigidbody.AddTorque(transform.up * rotationSpeed);
		}

		if (Input.GetKey(KeyCode.I))
		{ // Pencher à gauche 
			rigidbody.AddTorque(transform.forward * rotationSpeed);
		}
		if (Input.GetKey(KeyCode.P))
		{ // Pencher à droite
			rigidbody.AddTorque(-transform.forward * rotationSpeed);
		}
	}
}
