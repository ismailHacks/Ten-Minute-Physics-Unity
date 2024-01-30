//Using this test to add tetrahedrons in a voxel like way

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class voxelTet : TetrahedronData
{
	//Getters
	//public override float[] GetVerts => verts;
	//public override int[] GetTetIds => tetIds;
	//public override int[] GetTetEdgeIds => tetEdgeIds;
	//public override int[] GetTetSurfaceTriIds => tetSurfaceTriIds;

	private static int noVoxels = 10;
	private static float voxelScale = 0.2f;
	private int globalVoxelCount = 0;
	private int connectionCount = 0;
	private int repeatCount = 0;


	private float[] vertsVoxelMesh = new float[24*noVoxels];
	private int[] tetIdsVoxelMesh = new int[20*noVoxels];
	private int[] tetEdgeIdsVoxelMesh = new int[36*noVoxels];
	private int[] tetSurfaceTriIdsVoxelMesh = new int[48*noVoxels];

	public override float[] GetVerts => vertsVoxelMesh;
	public override int[] GetTetIds => tetIdsVoxelMesh;
	public override int[] GetTetEdgeIds => tetEdgeIdsVoxelMesh;
	public override int[] GetTetSurfaceTriIds => tetSurfaceTriIdsVoxelMesh;

	public voxelTet()
	{
		makeVoxel(0,0,0);
		makeVoxel(1,0,0);
		makeVoxel(2,0,0);
		makeVoxel(3,0,0);
		makeVoxel(0,1,0);
		makeVoxel(1,1,0);
		makeVoxel(1,2,0);
		combineVoxels();
	}

	private void makeVoxel(int posX, int posY, int posZ)
	{
		for (int i = 0; i < verts.Length/3; i++)
		{
			vertsVoxelMesh[3*i+(24*globalVoxelCount)] = (verts[3*i]+posX)*voxelScale;
			vertsVoxelMesh[3*i+1+(24*globalVoxelCount)] = (verts[3*i+1]+posY)*voxelScale;
			vertsVoxelMesh[3*i+2+(24*globalVoxelCount)] = (verts[3*i+2]+posZ)*voxelScale;
		}

		for (int i = 0; i < tetIds.Length; i++)
		{
			tetIdsVoxelMesh[i+20*globalVoxelCount] = tetIds[i]+8*globalVoxelCount;
		}

		for (int i = 0; i < tetEdgeIds.Length; i++)
		{
			tetEdgeIdsVoxelMesh[i+36*globalVoxelCount] = tetEdgeIds[i]+8*globalVoxelCount;
		}

		for (int i = 0; i < tetSurfaceTriIds.Length; i++)
		{
			tetSurfaceTriIdsVoxelMesh[i+48*globalVoxelCount] = tetSurfaceTriIds[i]+8*globalVoxelCount;
		}
		globalVoxelCount++;
	}

	private void combineVoxels()
	{
		for (int i = 0; i < vertsVoxelMesh.Length/3; i++)
		{
			for (int j = 0; j < vertsVoxelMesh.Length/3; j++)
			{
				if(vertsVoxelMesh[3*i]==vertsVoxelMesh[3*j] && vertsVoxelMesh[3*i+1]==vertsVoxelMesh[3*j+1] && vertsVoxelMesh[3*i+2]==vertsVoxelMesh[3*j+2] && i!=j)
				{
					connectionCount++;
					int iPos = 3*i;
					int jPos = 3*j;

					for (int k = 0; k < tetEdgeIdsVoxelMesh.Length; k++)
					{
						if (tetEdgeIdsVoxelMesh[k] == jPos/3)
						{
							tetEdgeIdsVoxelMesh[k] = iPos/3;
						}
					}

					for (int l = 0; l < tetIdsVoxelMesh.Length; l++)
					{
						if (tetIdsVoxelMesh[l] == jPos/3)
						{
							tetIdsVoxelMesh[l] = iPos/3;
						}
					}

					for (int m = 0; m < tetSurfaceTriIdsVoxelMesh.Length; m++)
					{
						if (tetSurfaceTriIdsVoxelMesh[m] == jPos/3)
						{
							tetSurfaceTriIdsVoxelMesh[m] = iPos/3;
						}
					}
					repeatCount++;
				}
			}
		}
		Debug.Log("CC = " + connectionCount);
	}

	//Vertices (x, y, z) for a tetrahedral cube
	//Provides the vertices of each particle in the mesh
	private float[] verts =
	{
		0,0,1,
		1,1,1,
		1,0,0,
		0,1,0,

		0,0,0,
		1,1,0,
		1,0,1,
		0,1,1
	};

	//Provides the ID position of the vertices that make up the tetrahedral cube
	private int[] tetIds =
	{
		0,1,2,3,
		0,3,2,4,
		1,2,3,5,
		0,2,1,6,
		0,1,3,7
	};

	//Provides the connections between each one of the edges in the tetrahedral cube
	//unlike tetIds the edges should not be repeated with connecting tetrahedrals as they will be looped over.
	private int[] tetEdgeIds =
	{
		//0,2, 2,1, 1,0, 1,3, 3,0, 2,3
		0,1, 1,2, 2,0, 0,3, 1,3, 2,3, //Centre Tetrahedron
		0,4, 2,4, 3,4, //Outer Tetrahedron 1
		1,5, 2,5, 3,5, //Outer Tetrahedron 2
		0,6, 1,6, 2,6,  //Outer Tetrahedron 3
		0,7, 1,7, 3,7  //Outer Tetrahedron 4

	};

	//Provides the connections between all surfaces that are visible in order to render a mesh, must be clockwise done when looking at the surface.
	private int[] tetSurfaceTriIds =
	{
		//0,1,3, 1,2,3, 0,3,2, 0,2,1, //Inner Tetrahedron - Don't need to render inner tetrahedron as it is covered by the outer tetrahedrons
		0,3,4, 4,3,2, 0,4,2, 2,3,0, //Outer Tetrahedron 1
		2,3,5, 2,5,1, 3,1,5, 1,3,2, //Outer Tetrahedron 2
		0,6,1, 0,2,6, 1,6,2, 0,1,2, //Outer Tetrahedron 3
		0,7,3, 0,1,7, 1,3,7, 0,3,1  //Outer Tetrahedron 4

	};


}
