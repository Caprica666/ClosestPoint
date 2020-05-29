
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hull : MonoBehaviour
{
    private List<Vector3> mHull;
    private LineRenderer mCurLine = null;
    private Color mColor;

    public List<Vector3> Vertices
    {
        get { return mHull; }
    }

    public int Count
    {
        get { return mHull.Count; }
    }

    public void Awake()
	{
		mCurLine = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        mCurLine.useWorldSpace = false;
        mCurLine.widthMultiplier = 0.1f;
        mCurLine.material = new Material(Shader.Find("Unlit/Color"));
        mColor = mCurLine.material.color;
    }

    public IEnumerator MakeHull(List<Vector3> points, Color c)
    {
        List<Vector3> reversed = new List<Vector3>(points);

        reversed.Reverse();
        mColor = c;
        mCurLine.material.color = c;
        mHull = new List<Vector3>();
        mHull = new List<Vector3>();
        yield return StartCoroutine(CalcUpperHull(points.GetEnumerator()));
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CalcLowerHull(reversed.GetEnumerator()));
        mCurLine.positionCount = mHull.Count;
        mCurLine.SetPositions(mHull.ToArray());
        yield return new WaitForEndOfFrame();
    }

    public static float Cross2D(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 u1 = v2 - v1;
        Vector3 u2 = v3 - v2;
        return (u1.x * u2.y) - (u1.y * u2.x);
    }

    IEnumerator CalcUpperHull(IEnumerator iter)
    {
        Vector3 v1;
        Vector3 v2;
        Vector3 v3;

        iter.Reset();
        iter.MoveNext();
        v1 = (Vector3) iter.Current;
        mHull.Add(v1);
        iter.MoveNext();
        v2 = (Vector3) iter.Current;
        mHull.Add((Vector3) iter.Current);
        iter.MoveNext();

        while (true)
        {
            v3 = (Vector3) iter.Current;
            v2 = mHull[mHull.Count - 2];
            v1 = mHull[mHull.Count - 1];
            float cross = Cross2D(v1, v2, v3);
            int last = mHull.Count - 1;

            if (cross > 0)
            {
                mHull.Add(v3);
            }
            else if (last >= 2)
            {
                mHull.RemoveAt(last);
                mCurLine.positionCount = mHull.Count;
                mCurLine.SetPositions(mHull.ToArray());
                yield return new WaitForEndOfFrame();
                continue;
            }
            else
            {
                mHull[last] = v3;
            }
            mCurLine.positionCount = mHull.Count;
            mCurLine.SetPositions(mHull.ToArray());
            if (!iter.MoveNext())
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator CalcLowerHull(IEnumerator iter)
    {
        Vector3 v1;
        Vector3 v2;
        Vector3 v3;
        int start = mHull.Count - 2;

        iter.Reset();
        iter.MoveNext();
        v1 = (Vector3) iter.Current;
        mHull.Add(v1);
        iter.MoveNext();
        v2 = (Vector3) iter.Current;
        mHull.Add(v2);
        iter.MoveNext();

        while (true)
        {
            v1 = mHull[mHull.Count - 1];
            v2 = mHull[mHull.Count - 2];
            v3 = (Vector3) iter.Current;
            Vector3 u1 = v2 - v1;
            Vector3 u2 = v3 - v2;
            float cross = (u1.x * u2.y) - (u1.y * u2.x);
            int last = mHull.Count - 1;

            if (cross > 0)
            {
                mHull.Add(v3);
            }
            else if (last > start)
            {
                mHull.RemoveAt(last);
                mCurLine.positionCount = mHull.Count;
                mCurLine.SetPositions(mHull.ToArray());
                yield return new WaitForEndOfFrame();
                continue;
            }
            else
            {
                mHull[last] = v3;
            }
            mCurLine.positionCount = mHull.Count;
            mCurLine.SetPositions(mHull.ToArray());
            if (!iter.MoveNext())
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }
}
