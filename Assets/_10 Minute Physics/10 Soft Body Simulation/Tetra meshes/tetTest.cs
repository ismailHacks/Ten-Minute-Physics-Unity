using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tetTest : TetrahedronData
{
	private readonly float[] verts;

	//Getters
	public override float[] GetVerts => verts;

	public override int[] GetTetIds => tetIds;

	public override int[] GetTetEdgeIds => tetEdgeIds;

	public override int[] GetTetSurfaceTriIds => tetSurfaceTriIds;



	public tetTest()
	{
		//Convert from double to float
		verts = new float[vertsDouble.Length];

		for (int i = 0; i < verts.Length; i++)
		{
			verts[i] = (float)vertsDouble[i];
        }
    }



	//Vertices (x, y, z) come after each other so divide by 3 to get total vertices
	private readonly double[] vertsDouble =
	{
		1f, 0f, 0f, -0.5f, Mathf.Sqrt(3) / 2f, 0f, -0.5f, -Mathf.Sqrt(3) / 2f, 0f, 0f, 0f, Mathf.Sqrt(2)
	};

	private readonly int[] tetIds =
	{
		0,1,2,3
	};

	private readonly int[] tetEdgeIds =
	{
		0,1, 1,2, 2,0, 0,3, 2,3, 1,3
	};

	private readonly int[] tetSurfaceTriIds =
	{
		0,1,2, 0,2,3, 0,3,1, 1,3,2
	};



}
