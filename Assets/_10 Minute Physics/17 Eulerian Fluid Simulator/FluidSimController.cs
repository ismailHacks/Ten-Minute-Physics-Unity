using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluidSimulator;

//Most basic fluid simulator
//Based on: "How to write an Eulerian Fluid Simulator with 200 lines of code" https://matthias-research.github.io/pages/tenMinutePhysics/
//Eulerian means we simulate the fluid in a grid - not by using particles (Lagrangian). One can also use a combination of both methods
//Can simulate both liquids and gas
//Assume incompressible fluid with zero viscosity (inviscid) which are good approximations for water and gas
public class FluidSimController : MonoBehaviour
{
    //Public
    public Material fluidMaterial;


    //Private
    private Scene scene;

    private DisplayFluid displayFluid;

    private FluidUI fluidUI;



    private void Start()
    {    
        scene = new Scene();

        displayFluid = new DisplayFluid(fluidMaterial);

        fluidUI = new FluidUI(this);

        //SetupScene(Scene.SceneNr.WindTunnel);

        //SetupScene(Scene.SceneNr.Tank);
    }



    private void Update()
    {
        //Display the fluid
        displayFluid.TestDraw();
    }



    private void FixedUpdate()
    {
        return;
    
        //Simulate the fluid
        if (!scene.isPaused)
        {
            scene.fluid.Simulate(scene.dt, scene.gravity, scene.numIters, scene.overRelaxation);

            scene.frameNr++;
        }
    }



    //Init the simulation after a button has been pressed
    public void SetupScene(Scene.SceneNr sceneNr = Scene.SceneNr.Tank)
    {
        scene.sceneNr = sceneNr;
        scene.obstacleRadius = 0.15f;
        scene.overRelaxation = 1.9f;

        scene.dt = Time.fixedDeltaTime;
        scene.numIters = 40;

        //How detailed the simulation is in height (y) direction
        int res = 100;

        if (sceneNr == Scene.SceneNr.Tank)
        {
            res = 50;
        }
        else if (sceneNr == Scene.SceneNr.HighResWindTunnel)
        {
            res = 200;
        }


        //The height of the simulation is 1 m (in the tutorial) but the guy is also setting simHeight = 1.1 and domainHeight = 1 so Im not sure which is which. But he says 1 m in the video
        float simHeight = 1f;

        //The size of a cell
        float h = simHeight / res;

        //How many cells do we have
        //y is up
        int numY = Mathf.FloorToInt(simHeight / h);
        //Twice as wide
        int numX = 2 * numY;

        //Density of the fluid (water)
        float density = 1000f;

        FluidSim f = scene.fluid = new FluidSim(density, numX, numY, h);

        if (sceneNr == Scene.SceneNr.Tank)
        {
            SetupTank(f);
        }
        else if (sceneNr == Scene.SceneNr.WindTunnel || sceneNr == Scene.SceneNr.HighResWindTunnel)
        {
            SetupWindTunnel(f, sceneNr);
        }
        else if (sceneNr == Scene.SceneNr.Paint)
        {
            SetupPaint();
        }
    }



    private void SetupTank(FluidSim f)
    {
        int n = f.numY;

        //Add a solid border
        for (int i = 0; i < f.numX; i++)
        {
            for (int j = 0; j < f.numY; j++)
            {
                //Fluid
                float s = 1f;

                if (i == 0 || i == f.numX - 1 || j == 0)
                {
                    s = 0f;
                }

                f.s[i * n + j] = s;
            }
        }

        scene.gravity = -9.81f;
        scene.showPressure = true;
        scene.showSmoke = false;
        scene.showStreamlines = false;
        scene.showVelocities = false;
    }



    private void SetupWindTunnel(FluidSim f, Scene.SceneNr sceneNr)
    {
        int n = f.numY;

        //Wind velocity
        float inVel = 2f;

        for (int i = 0; i < f.numX; i++)
        {
            for (int j = 0; j < f.numY; j++)
            {
                //Fluid
                float s = 1f;

                if (i == 0 || j == 0 || j == f.numY - 1)
                {
                    //Solid
                    s = 0f;
                }

                f.s[i * n + j] = s;

                //Add constant velocity in the first column
                if (i == 1)
                {
                    f.u[i * n + j] = inVel;
                }
            }
        }

        //Add smoke
        float pipeH = 0.1f * f.numY;

        int minJ = Mathf.FloorToInt(0.5f * f.numY - 0.5f * pipeH);
        int maxJ = Mathf.FloorToInt(0.5f * f.numY + 0.5f * pipeH);

        for (var j = minJ; j < maxJ; j++)
        {
            f.m[j] = 0f; //Why is this 0???
        }


        //Position the obstacle
        //The obstacle in the demo is only reset if we click on wind tunnel button
        //Otherwise it has the same position as last scene
        SetObstacle(0.4f, 0.5f, true);


        scene.gravity = 0f; //???
        scene.showPressure = false;
        scene.showSmoke = true;
        scene.showStreamlines = false;
        scene.showVelocities = false;

        if (sceneNr == Scene.SceneNr.HighResWindTunnel)
        {
            //scene.dt = 1.0 / 120.0;
            scene.numIters = 100;
            scene.showPressure = true;
        }
    }



    private void SetupPaint()
    {
        scene.gravity = 0f;
        scene.overRelaxation = 1f;
        scene.showPressure = false;
        scene.showSmoke = true;
        scene.showStreamlines = false;
        scene.showVelocities = false;
        scene.obstacleRadius = 0.1f;
    }



    //Position and obstacle in the fluid and make it interact with the fluid if it has a velocity
    private void SetObstacle(float x, float y, bool reset)
    {
        //So we can give the fluid a velocity by moving around the obstacle
        float vx = 0f;
        float vy = 0f;

        if (!reset)
        {
            vx = (x - scene.obstacleX) / scene.dt;
            vy = (y - scene.obstacleY) / scene.dt;
        }

        scene.obstacleX = x;
        scene.obstacleY = y;

        float r = scene.obstacleRadius;
        
        FluidSim f = scene.fluid;
        
        int n = f.numY;
        
        //float cd = Mathf.Sqrt(2f) * f.h;

        //Mark cells as obstacle
        for (int i = 1; i < f.numX - 2; i++)
        {
            for (int j = 1; j < f.numY - 2; j++)
            {
                f.s[i * n + j] = 1f;

                float dx = (i + 0.5f) * f.h - x;
                float dy = (j + 0.5f) * f.h - y;

                //Is the cell within the obstacle?
                if (dx * dx + dy * dy < r * r)
                {
                    f.s[i * n + j] = 0f;

                    if (scene.sceneNr == Scene.SceneNr.Paint)
                    {
                        f.m[i * n + j] = 0.5f + 0.5f * Mathf.Sin(0.1f * scene.frameNr);
                    }
                    else
                    {
                        f.m[i * n + j] = 1f;
                    }

                    //Give the fluid a velocity if we have moved it
                    f.u[i * n + j] = vx;
                    f.u[(i + 1) * n + j] = vx;
                    f.v[i * n + j] = vy;
                    f.v[i * n + j + 1] = vy;
                }
            }
        }

        scene.showObstacle = true;
    }



    //UI
    private void OnGUI()
    {
        fluidUI.DisplayUI(scene);
    }
}
