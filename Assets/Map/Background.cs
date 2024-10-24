using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private static float Y_COMPONENT = 100f;
    public void draw(float width, float height, int part, Material mat)
    {
        float left = -(width / 2);
        float top = 3 * height / 2;
        float bottom = -(height / 2);
        float right = 3 * width / 2;
        Mesh mesh = new Mesh
        {
            name = "BackgroundMesh"
        };

        List<Vector3> vertices = new List<Vector3>();
        if (part == 0)
        {
            name = "bottom";
            vertices.Add(new Vector3(left, -0.5f, bottom));
            vertices.Add(new Vector3(left, -0.5f, top));
            vertices.Add(new Vector3(right, -0.5f, top));
            vertices.Add(new Vector3(right, -0.5f, bottom));
        }
        else if (part == 1)
        {
            name = "west";
            vertices.Add(new Vector3(left, -0.5f, bottom));
            vertices.Add(new Vector3(left, Y_COMPONENT, bottom));
            vertices.Add(new Vector3(left, Y_COMPONENT, top));
            vertices.Add(new Vector3(left, -0.5f, top));
        }
        else if (part == 2)
        {
            name = "north";
            vertices.Add(new Vector3(left, -0.5f, top));
            vertices.Add(new Vector3(left, Y_COMPONENT, top));
            vertices.Add(new Vector3(right, Y_COMPONENT, top));
            vertices.Add(new Vector3(right, -0.5f, top));
        }
        else if (part == 3)
        {
            name = "east";
            vertices.Add(new Vector3(right, -0.5f, top));
            vertices.Add(new Vector3(right, Y_COMPONENT, top));
            vertices.Add(new Vector3(right, Y_COMPONENT, bottom));
            vertices.Add(new Vector3(right, -0.5f, bottom));
        }
        else
        {
            name = "south";
            vertices.Add(new Vector3(right, -0.5f, bottom));
            vertices.Add(new Vector3(right, Y_COMPONENT, bottom));
            vertices.Add(new Vector3(left, Y_COMPONENT, bottom));
            vertices.Add(new Vector3(left, -0.5f, bottom));
        }

        List<int> triangles = new List<int>(new int[] { 0, 1, 2, 0, 2, 3 });

        List<Vector3> normals = new List<Vector3>();
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);

        List<Vector2> uvs = new List<Vector2>();
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.SetUVs(0, uvs);

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = mat;
    }
}
