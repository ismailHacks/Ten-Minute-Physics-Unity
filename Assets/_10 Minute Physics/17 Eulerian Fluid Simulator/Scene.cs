using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Settings for the fluid simulation and a ref to the fluid simulation itself
public class Scene
{
    //The tutorial is using an int: tank (0), wind tunnel (1), paint (2), highres wind tunnel (3)
    public enum SceneNr
    {
        Tank, WindTunnel, Paint, HighResWindTunnel
    }

    //public int sceneNr = 0;

    public SceneNr sceneNr;

    //Display settings
    public bool showStreamlines = false;
    public bool showVelocities = false;
    public bool showPressure = false;
    public bool showSmoke = true;

    //Simulation settings
    public bool useOverRelaxation = true; //Is not in the tutorial but needs to be there to make Unity's toggles work

    public float dt;

    public int numIters = 100;

    public float gravity = -9.81f;

    public float overRelaxation = 1.9f;

    public Fluid fluid;

    //Useful for debugging 
    public int frameNr = 0;

    public bool isPaused = false;

    //Obstacles
    public bool showObstacle = false;

    public float obstacleX = 0f;
    public float obstacleY = 0f;

    public float obstacleRadius = 0.15f;
}
