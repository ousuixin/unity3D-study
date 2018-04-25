using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ParabolicMovement : MonoBehaviour
{
    public float gravity = -50;
    private Vector3 Speed;//物体的速度（矢量）
    private Vector3 gravitySpeed;//物体在重力方向的速度（矢量）
    private Vector3 distanceWalkedBySpeed;//单位时间内物体在初速读方向上走过的距离（矢量）
    private Vector3 distanceWalkedVertical = Vector3.zero;//单位时间内物体在重力方向走过的距离（矢量）
    private float timePassed = 0;//已经过去的时间

    public void Start()
    {
        Speed = getRandomVelocity();
        gravitySpeed = Vector3.zero;

        GetComponent<Rigidbody>().useGravity = false;
        transform.position = new Vector3(0, -5f, 0);
    }
    
    public void Update()
    {
        timePassed += Time.deltaTime;
        gravitySpeed.y = timePassed * gravity;
        distanceWalkedBySpeed = Speed * Time.deltaTime;
        distanceWalkedVertical = gravitySpeed * Time.deltaTime;

        transform.position += distanceWalkedBySpeed + distanceWalkedVertical;
    }

    Vector3 getRandomVelocity()
    {
        int x = UnityEngine.Random.Range(-30, 30);
        int y = UnityEngine.Random.Range(25, 50);
        int z = UnityEngine.Random.Range(25, 50);
        return new Vector3(x, y, z);
    }
    
}