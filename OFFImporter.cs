/*
Copyright (c) 2018 Michel Kuhlburger

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text;

public class OFFImporter {

    private StringBuilder sb;

    public OFFImporter()
    {
        sb = new StringBuilder();
    }

    int GoToNewLine(ref string str, int begin = 0)
    {
        int i = begin;
        while (i < str.Length && str[i] != '\n')
            ++i;
        ++i;
        return i;
    }

    int GetNaturalFromString(ref string str, ref int index)
    {
        sb.Remove(0, sb.Length);
        while(index < str.Length && !Char.IsDigit(str[index]))
        {
            if (str[index] == '#')
            {
                index = GoToNewLine(ref str, index);
            }
            else
            {
                ++index;
            }
        }
        if (index == str.Length)
        {
            return -1;
        }

        while (index < str.Length && Char.IsDigit(str[index]))
        {
            sb.Append(str[index]);
            ++index;
        }

        return Int32.Parse(sb.ToString());
    }

    bool IsFloatChar(char c)
    {
        if (Char.IsDigit(c))
            return true;
        if (c == '.')
            return true;
        if (c == '-')
            return true;
        if (c == 'e' || c == 'E')
            return true;
        return false;
    }

    float GetFloatFromString(ref string str, ref int index)
    {
        sb.Remove(0, sb.Length);

        while (index < str.Length && !IsFloatChar(str[index]))
        {
            if (str[index] == '#')
            {
                index = GoToNewLine(ref str, index);
            }
            else
            {
                ++index;
            }
        }
        if (index == str.Length)
        {
            return 0.0f;
        }

        while (index < str.Length && IsFloatChar(str[index]))
        {
            sb.Append(str[index]);
            ++index;
        }

        return float.Parse(sb.ToString());

    }


    public Mesh Import(string fileData)
    {
        int numVertices = 0, numFaces = 0, numEdges = 0;
        int i = 0;
        numVertices = GetNaturalFromString(ref fileData, ref i);
        numFaces = GetNaturalFromString(ref fileData, ref i);
        numEdges = GetNaturalFromString(ref fileData, ref i);


        Vector3[] vertices = new Vector3[numVertices];

        for(int j = 0;j < numVertices; ++j)
        {
            float x, y, z;
            x = GetFloatFromString(ref fileData, ref i);
            y = GetFloatFromString(ref fileData, ref i);
            z = GetFloatFromString(ref fileData, ref i);
            vertices[j] = new Vector3(x, y, z);
        }


        int[][] faces = new int[numFaces][];
        int indexCount = 0;

        int k = 0;

        for (int j = 0; j < numFaces; ++j)
        {
            int faceVericeNumber = GetNaturalFromString(ref fileData, ref i);
            faces[j] = new int[faceVericeNumber];
            if(faceVericeNumber == 3)
            {
                indexCount += 3;
            }
            else
            {
                if (faceVericeNumber == 4)
                {
                    indexCount += 6;
                }
            }
            
            for(k=0;k< faceVericeNumber; ++k)
            {
                faces[j][k] = GetNaturalFromString(ref fileData, ref i);
            }
        }

        int[] triangles = new int[indexCount];
        k = 0;

        for(int j = 0; j < numFaces; ++j)
        {
            for(int l = 0; l < 3; ++l)
            {
                triangles[k] = faces[j][l];
                ++k;
            }
            if(faces[j].Length == 4)
            {
                triangles[k] = faces[j][0];
                triangles[k + 1] = faces[j][2];
                triangles[k + 2] = faces[j][3];
                k += 3;
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
	
}
