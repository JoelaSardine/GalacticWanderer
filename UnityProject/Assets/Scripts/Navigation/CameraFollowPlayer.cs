using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{

    public Transform player;

	Rigidbody playerRB;

    //GameStateManager gameStateManagerScript;
    Camera cam;

    Vector3 lookAtposition;
    Transform TargetPosition;

	float distOffset;

    //ParticleSystem speedFx;

    void Awake()
    {
        //gameStateManagerScript = FindObjectOfType<GameStateManager>();
        cam = GetComponent<Camera>();
        GameObject PosTarget = new GameObject("CameraTarget");
        TargetPosition = PosTarget.transform;
        TargetPosition.position = transform.position;
        TargetPosition.SetParent(player);
		//speedFx = transform.GetChild(0).GetComponent<ParticleSystem>();

		playerRB = player.GetComponent<Rigidbody>();
		distOffset = Vector3.Distance(transform.position, player.position);

    }

    void Update ()
    {
		if (playerRB.velocity != Vector3.zero)
		{
			transform.position = player.position - playerRB.velocity.normalized * distOffset;
			transform.LookAt(transform.position + playerRB.velocity.normalized * distOffset, player.up);
			transform.position += transform.up * 5.0f;
		}

		//transform.position = Vector3.Lerp(transform.position, TargetPosition.position, Time.deltaTime * 6);
		//transform.rotation = Quaternion.Lerp(transform.rotation, TargetPosition.rotation, Time.deltaTime * 6);



		//transform.LookAt(player);

		//cam.fieldOfView = Mathf.Lerp(50, 100, gameStateManagerScript.playerCurrentSpeed);

		//ParticleSystem.EmissionModule emmMod = speedFx.emission;
		//emmMod.rate = Mathf.Lerp(-50, 50, gameStateManagerScript.playerCurrentSpeed);


	}
}
