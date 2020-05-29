using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class PointList
{
    private List<Vector3> mPoints;
    private PointMesh mPointsToRender;
    private static readonly float EPSILON = 1e-5f;
    private static readonly float SCALE = 0.00001f;

    public List<Vector3> Points
    {   get { return mPoints; } }

    public PointMesh PointsToRender
    {
        get { return mPointsToRender; }
        set { mPointsToRender = value; } 
    }

    public PointList(PointMesh pm)
    {
        mPoints = new List<Vector3>();
        mPointsToRender = pm;
    }

    public PointList(List<Vector3> points)
    {
        mPoints = new List<Vector3>();
        Add(points);
    }

    public List<Vector3> SavePoints()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (Vector3 v in mPoints)
        {
            points.Add(new Vector3(v.x, v.y, v.z));
        }
        return points;
    }

    public int Count
    {
        get { return mPoints.Count; }
    }


    public void Add(Vector3 v, bool addtomesh = false)
    {
        mPoints.Add(v);
        if (addtomesh && (mPointsToRender != null))
        {
            mPointsToRender.Add(v);
        }
    }

    public void Add(List<Vector3> points, bool addtomesh = true)
    {
        foreach (Vector3 v in points)
        {
            Add(v, addtomesh);
        }
    }

    public bool Remove(Vector3 v)
    {
        return mPoints.Remove(v);
    }

    public void Clear(bool clearmesh = false)
    {
        mPoints.Clear();
        if (clearmesh && (mPointsToRender != null))
        {
            mPointsToRender.Clear();
        }
    }

    public void ReadPoints(TextAsset pointsfile)
    {
        mPoints.Clear();
        string data = pointsfile.ToString();
        StringReader sr = new StringReader(data);
        string line = sr.ReadLine();
        float numpoints = float.Parse(line);

        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split('\t');
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            Add(new Vector3(x / SCALE, y / SCALE, 0));
        }
        SortByX();
    }

    public void Display(bool makemesh = false)
    {
        if (mPointsToRender != null)
        {
            if (makemesh)
            {
                mPointsToRender.MakeMesh(mPoints);
            }
            else
            {
                mPointsToRender.Display();
            }
        }
    }

    public void NewPoints(Rect bounds, int n)
    {
        float size = bounds.width;
        for (int i = 0; i < n; i++)
        {
            float x = Random.value - 0.5f;
            float y = Random.value - 0.5f;
            Vector3 v = new Vector3(size * x + bounds.x,
                                     size * y + bounds.y,
                                     0);
            mPoints.Add(v);
        }
        Display();
    }

    public void SortByX()
    {
        mPoints = SortByX(mPoints);
    }

    /*
     * Sort the points based on X coordinate
     */
    public static List<Vector3> SortByX(List<Vector3> points)
    {
        int n = points.Count;
        int m = n / 2;
        if (n <= 1)
        {
            return points;
        }
        List<Vector3> left = points.GetRange(0, m);
        List<Vector3> right = points.GetRange(m, n - m);

        if (left.Count > 1)
        {
            left = SortByX(left);
        }
        if (right.Count > 1)
        {
            right = SortByX(right);
        }
        return Merge(left, right);
    }

    /*
     * Merge two point lists based on the X coordinate
     */
    public static List<Vector3> Merge(List<Vector3> left, List<Vector3> right)
    {
        List<Vector3> result = new List<Vector3>();
        int lofs = 0;
        int rofs = 0;

        while ((lofs < left.Count) && (rofs < right.Count))
        {
            Vector3 v1 = left[lofs];
            Vector3 v2 = right[rofs];

            if (v1.x >= v2.x)
            {
                result.Add(left[lofs++]);
            }
            else
            {
                result.Add(right[rofs++]);
            }
        }

        // Either left or right may have elements left; consume them.
        // (Only one of the following loops will actually be entered.)
        while (lofs < left.Count)
        {
            result.Add(left[lofs++]);
        }
        while (rofs < right.Count)
        {
            result.Add(right[rofs++]);
        }
        return result;
    }

}


