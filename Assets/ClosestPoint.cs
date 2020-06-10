using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestPoint : MonoBehaviour
{
    public bool Test;
    public bool New;
    public bool Step;
    public TextAsset PointsFile;

    private PointMesh mPointsToRender;
    private PointList mPointList;
    private Hull mHull;
    private Rect mBounds = new Rect(0, 0, 6, 6);

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        mPointsToRender = gameObject.GetComponent<PointMesh>() as PointMesh;
        mPointList = new PointList(mPointsToRender);
        mHull = null;
        mPointList.Clear();
        mPointList.ReadPoints(PointsFile);
        mPointList.Display(true);
    }

    private void Update()
    {
        if (New)
        {
            New = false;
            Init();
            StartCoroutine(FindClosestPair());
        }
        else if (Step)
        {
            Step = false;
            StartCoroutine(FindClosestPair());
        }
    }

    IEnumerator NewHull()
    {
        Color c = new Color(Random.value, Random.value, Random.value, 1);
        GameObject line = new GameObject("line");

        yield return new WaitForEndOfFrame();
        mHull = line.AddComponent<Hull>() as Hull;
        line.transform.SetParent(gameObject.transform, false);
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(mHull.MakeHull(mPointList.Points, c));
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        mHull.RemoveHullPoints(mPointList.Points);
        yield return new WaitForEndOfFrame();
        mPointList.Display(true);
    }

    IEnumerator FindClosestPair()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        while (mPointList.Count >= 4)
        {
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(NewHull());
        }
    }

}

   
