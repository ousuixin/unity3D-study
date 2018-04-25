using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承ActionManager，拥有一个抛飞碟动作（通过运动学实现）
public class KinematicActionManager : IActionManager
{
    private static KinematicActionManager instance;
    private Material myRed, myOrange, myGreen, myBlue;

    private KinematicActionManager()
    {
        myRed = Resources.Load("Materials/myRed", typeof(Material)) as Material;
        myOrange = Resources.Load("Materials/myOrange", typeof(Material)) as Material;
        myGreen = Resources.Load("Materials/myGreen", typeof(Material)) as Material;
        myBlue = Resources.Load("Materials/myBlue", typeof(Material)) as Material;
    }

    public static KinematicActionManager getInstance()
    {
        if (instance == null)
        {
            instance = new KinematicActionManager();
        }
        return instance;
    }

    public IEnumerator launchDisks(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject disk = DiskFactory.getInstance().getUseableDisk();
            disk.transform.position = new Vector3(0, -5f, 0);
            disk.GetComponent<MeshRenderer>().material = getMaterial(num);
            if (disk.GetComponent<ParabolicMovement>() == null)
            {
                disk.AddComponent<ParabolicMovement>();
            } else
            {
                Object.Destroy(disk.GetComponent("ParabolicMovement"));
                disk.AddComponent<ParabolicMovement>();
            }

            float temp = 2.0f / num;
            yield return new WaitForSeconds(temp);
        }
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
