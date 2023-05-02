using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Curve
{
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 pointC;
    private int _amount = 10;
    
    public Curve(Vector2 _pointA, Vector2 _pointB, Vector2 _pointC)
    {
        pointA = _pointA;
        pointB = _pointB;
        pointC = _pointC;
    }
    
     IEnumerable<Vector2> GetCurvePoints(float amount)
    {
        float ratio = 1f / amount;
        
        float t = 0;
        while (t < 1) 
        {
            t = Mathf.MoveTowards(t, 1, ratio);
            yield return CurvePoint(t);
        }
    }
    
    Vector2 CurvePoint(float t)
    {
        Vector2 AB = Vector2.Lerp(pointA, pointB, t);
        Vector3 BC = Vector2.Lerp(pointB, pointC, t);
        Vector2 curvePoint = Vector3.Lerp(AB, BC, t);
        return curvePoint;
    }
    
    public void CreateLine(LineRenderer lr)
    {
        var points = GetCurvePoints(_amount);
        lr.positionCount = _amount;
        lr.SetPositions(points.Select(point => (Vector3) point).ToArray());
    }
    
    
    
   
}
