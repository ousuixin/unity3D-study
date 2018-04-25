using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//继承ActionManager，拥有一个抛飞碟动作（通过物理力学实现）
public class PhysisActionManager : IActionManager
{
    private static PhysisActionManager instance;
    private Material myRed, myOrange, myGreen, myBlue;

    private PhysisActionManager()
    {
        myRed = Resources.Load("Materials/myRed", typeof(Material)) as Material;
        myOrange = Resources.Load("Materials/myOrange", typeof(Material)) as Material;
        myGreen = Resources.Load("Materials/myGreen", typeof(Material)) as Material;
        myBlue = Resources.Load("Materials/myBlue", typeof(Material)) as Material;
    }

    public static PhysisActionManager getInstance()
    {
        if (instance == null)
        {
            instance = new PhysisActionManager();
        }
        return instance;
    }
    
    //第n轮的10次trial每一次都是发射n个飞盘，相邻两个飞盘发射时间间隔为2/n
    public IEnumerator launchDisks(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject disk = DiskFactory.getInstance().getUseableDisk();
            disk.transform.position = new Vector3(0, -5f, 0);
            disk.GetComponent<MeshRenderer>().material = getMaterial(num);

            Vector3 force = getRandomForce();
            disk.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            float temp = 2.0f / num;
            yield return new WaitForSeconds(temp);
        }
    }

    Vector3 getRandomForce()
    {
        int x = UnityEngine.Random.Range(-30, 30);
        int y = UnityEngine.Random.Range(15, 40);
        int z = UnityEngine.Random.Range(15, 40);
        return new Vector3(x, y, z);
    }
    
    //获取材质
    private Material getMaterial(int i)
    {
        i = i % 4;
        if (i == 1)
        {
            return myRed;
        }
        else if (i == 2)
        {
            return myOrange;
        }
        else if (i == 3)
        {
            return myGreen;
        }
        else
        {
            return myBlue;
        }
    }
}