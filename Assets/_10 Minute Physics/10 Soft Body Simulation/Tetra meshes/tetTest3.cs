//Using this test to add tetrahedrons in a cylinder like pattern

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class tetTest3 : TetrahedronData
{
	private readonly float[] verts;

	//Getters
	public override float[] GetVerts => verts;
	public override int[] GetTetIds => tetIds;
	public override int[] GetTetEdgeIds => tetEdgeIds;
	public override int[] GetTetSurfaceTriIds => tetSurfaceTriIds;

	public float cylinderOuterRadius = 1f;
	public float cylinderInnerRadius = 0.4f;

	private double[] vertsCylinder = new double[24];
	private int[] tetIdsCylinder = new int[8];



	public tetTest3()
	{
		float angleBetweenTets = (2*(cylinderOuterRadius-cylinderInnerRadius)*Mathf.Tan(Mathf.Deg2Rad*30))/cylinderOuterRadius;
		Debug.Log(angleBetweenTets);
		angleBetweenTets = 360/(Mathf.Rad2Deg*angleBetweenTets);
		float angleBetweenTetsMod = 360/(int)angleBetweenTets;
		Debug.Log(angleBetweenTetsMod+ "   "+ angleBetweenTets);
		angleBetweenTetsMod = Mathf.Deg2Rad*angleBetweenTetsMod;
		
		for (int j = 1; j < 3; j++)
		{
			for (int i = 0; i < 1; i++)
			{
				float currentAngle = angleBetweenTetsMod*i;
				vertsCylinder[0] = cylinderOuterRadius*Mathf.Sin(currentAngle);
				vertsCylinder[1] = 0;
				vertsCylinder[2] = cylinderOuterRadius*Mathf.Cos(currentAngle);

				vertsCylinder[3] = cylinderInnerRadius*Mathf.Sin(currentAngle+angleBetweenTetsMod/2);
				vertsCylinder[4] = 0;
				vertsCylinder[5] = cylinderInnerRadius*Mathf.Cos(currentAngle+angleBetweenTetsMod/2);

				vertsCylinder[6] = cylinderOuterRadius*Mathf.Sin(currentAngle+angleBetweenTetsMod/2);
				vertsCylinder[7] =0.5f;
				vertsCylinder[8] = cylinderOuterRadius*Mathf.Cos(currentAngle+angleBetweenTetsMod/2);

				vertsCylinder[9] = cylinderOuterRadius*Mathf.Sin(currentAngle+angleBetweenTetsMod);
				vertsCylinder[10] = 0;
				vertsCylinder[11] = cylinderOuterRadius*Mathf.Cos(currentAngle+angleBetweenTetsMod/2);
			}
		}

		verts = new float[vertsCylinder.Length];
		for (int i = 0; i < verts.Length; i++)
		{
			verts[i] = (float)vertsCylinder[i];
        }

	}

	//Provides the ID position of the vertices that make up a tetrahedron
	private int[] tetIds =
	{
		0,1,2,3
	};

	//Provides the connections between each one of the edges in a tetrahedron,
	//unlike tetIds the edges should not be repeated with connecting tetrahedrals as they will be looped over.
	private int[] tetEdgeIds =
	{
		0,1, 0,2, 0,3, 1,2, 1,3, 2,3
	};

	//Provides the connections between all surfaces that are visible in order to render a mesh, must be clockwise done when looking at the surface.
	private int[] tetSurfaceTriIds =
	{
		0,2,1, 1,2,3, 1,3,0, 0,3,2
	};
}
