using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenActivable : ActivableObject {

    public Renderer rend;
    public MovieTexture movieTexture;

    private bool screenActive = false;
    private bool onPause;

    private void Start()
    {
        screenActive = false;
    }

    public override void Activate()
    {
        screenActive = true;
        movieTexture.Play();
    }

    private void Update()
    {
        rend.enabled = screenActive;
        if (Input.GetKey(KeyCode.P))
        {
            if (!onPause)
            {
                movieTexture.Pause();
                onPause = true;
            }
            else
            {
                movieTexture.Play();
                onPause = false;
            }
        }
        if (Input.GetKey(KeyCode.F2))
        {
            screenActive = false;
        }
    }
}
