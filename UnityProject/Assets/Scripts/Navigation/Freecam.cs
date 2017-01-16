using UnityEngine;
using System.Collections;

public class Freecam : MonoBehaviour
{
	public float rotationSpeed = 90.0f;
	public float movementSpeed = 10.0f;
	
	private float baseSpeed;
	private Vector2 rotation = Vector2.zero;
	private CursorLockMode savedCursorMode;
	
	private new Camera camera { get { return GetComponent<Camera>(); } }

	void Awake()
	{
		baseSpeed = movementSpeed;
	}

	void OnEnable()
	{
		savedCursorMode = Cursor.lockState;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void OnDisable()
	{
		Cursor.lockState = savedCursorMode;
	}

	void Update()
	{
		// Rotation control
		rotation.x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
		rotation.y += Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
		rotation.y = Mathf.Clamp(rotation.y, -90.0f, 90.0f);

		transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotation.y, Vector3.left);

		// Speed control
		if (Input.GetMouseButtonDown(2)) {
			movementSpeed = baseSpeed;
		} else {
			movementSpeed *= (1.0f + Input.GetAxis("Mouse ScrollWheel"));
		}

		// Movement control
		transform.position += transform.forward * movementSpeed
								* Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * movementSpeed 
								* Input.GetAxis("Horizontal") * Time.deltaTime;
		if (Input.GetKey(KeyCode.A)) {
			transform.position += transform.up * movementSpeed / 2.0f * Time.deltaTime;
		} else if (Input.GetKey(KeyCode.E)) {
			transform.position -= transform.up * movementSpeed / 2.0f * Time.deltaTime;
		}
	}
	
	void OnDrawGizmos()
	{
		Matrix4x4 savedMatrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, movementSpeed, movementSpeed / 10.0f, camera.aspect);
		Gizmos.matrix = savedMatrix;
	}
}