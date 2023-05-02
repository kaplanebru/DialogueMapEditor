using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Cam : MonoBehaviour
{
    public float offset = 0.1f;
    public float ease = 0.1f;
    private Vector3 camStartPos;
    void Awake()
    {
        camStartPos = transform.position;
    }

    void StartCamFromCheckPoint(Point startPoint)
    {
        transform.position = UpdateCamPos(startPoint);
    }
    private void OnEnable()
    {
        EventBus.OnPointSelection += MoveCamera;
        EventBus.OnGameStart += StartCamFromCheckPoint;
    }

    private void OnDisable()
    {
        EventBus.OnPointSelection -= MoveCamera;
        EventBus.OnGameStart -= StartCamFromCheckPoint;
    }


    IEnumerator MoveCamRoutine(Point selectedPoint)
    {
        Vector3 newPos = UpdateCamPos(selectedPoint);
        while (transform.position.y < newPos.y - 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, ease);
            yield return new WaitForFixedUpdate();
        }
    }
    void MoveCamera(Point selectedPoint)
    {
        //transform.position = UpdateCamPos(selectedPoint);
        StartCoroutine(nameof(MoveCamRoutine), selectedPoint);
    }

    Vector3 UpdateCamPos(Point selectedPoint)
    {
        float newPosY = selectedPoint.transform.position.y * offset + camStartPos.y;
        return new Vector3(camStartPos.x, newPosY, camStartPos.z);
        
    }
}
