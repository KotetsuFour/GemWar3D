using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{

    public static float HALF_LENGTH = 0.5f;

    private void OnEnable()
    {
        draw(1);
    }
    public void draw(float height)
    {

        Mesh mesh = new Mesh
        {
            name = "TileMesh"
        };

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(-HALF_LENGTH, height, -HALF_LENGTH));
        vertices.Add(new Vector3(-HALF_LENGTH, height, HALF_LENGTH));
        vertices.Add(new Vector3(HALF_LENGTH, height, HALF_LENGTH));
        vertices.Add(new Vector3(HALF_LENGTH, height, -HALF_LENGTH));

        List<int> triangles = new List<int>(new int[] { 0, 1, 2, 0, 2, 3 });

        List<Vector3> normals = new List<Vector3>();
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);

        if (height > 0)
        {
            //Front
            vertices.Add(new Vector3(-HALF_LENGTH, height, -HALF_LENGTH)); //4
            vertices.Add(new Vector3(HALF_LENGTH, height, -HALF_LENGTH)); //5
            vertices.Add(new Vector3(-HALF_LENGTH, 0, -HALF_LENGTH)); //6
            vertices.Add(new Vector3(HALF_LENGTH, 0, -HALF_LENGTH)); //7

            triangles.AddRange(new int[] { 4, 7, 6, 4, 5, 7 });

            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));

            //Left
            vertices.Add(new Vector3(-HALF_LENGTH, height, -HALF_LENGTH)); //8
            vertices.Add(new Vector3(-HALF_LENGTH, height, HALF_LENGTH)); //9
            vertices.Add(new Vector3(-HALF_LENGTH, 0, -HALF_LENGTH)); //10
            vertices.Add(new Vector3(-HALF_LENGTH, 0, HALF_LENGTH)); //11

            triangles.AddRange(new int[] { 8, 10, 9, 9, 10, 11 });

            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));

            //Back
            vertices.Add(new Vector3(-HALF_LENGTH, height, HALF_LENGTH)); //12
            vertices.Add(new Vector3(HALF_LENGTH, height, HALF_LENGTH)); //13
            vertices.Add(new Vector3(-HALF_LENGTH, 0, HALF_LENGTH)); //14
            vertices.Add(new Vector3(HALF_LENGTH, 0, HALF_LENGTH)); //15

            triangles.AddRange(new int[] { 12, 14, 13, 13, 14, 15 });

            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));

            //Right
            vertices.Add(new Vector3(HALF_LENGTH, height, HALF_LENGTH)); //16
            vertices.Add(new Vector3(HALF_LENGTH, height, -HALF_LENGTH)); //17
            vertices.Add(new Vector3(HALF_LENGTH, 0, HALF_LENGTH)); //18
            vertices.Add(new Vector3(HALF_LENGTH, 0, -HALF_LENGTH)); //19

            triangles.AddRange(new int[] { 16, 18, 17, 17, 18, 19 });

            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
