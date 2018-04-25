using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiskFactory : System.Object
{
    private static DiskFactory instance;
    private static List<GameObject> diskList = new List<GameObject>();
    private GameObject disk;

    //实例化飞盘
    public void setDisk(GameObject disk)
    {
        this.disk = disk;
    }

    //获取飞碟工厂的单实例
    public static DiskFactory getInstance()
    {
        if (instance == null)
        {
            instance = new DiskFactory();
        }
        return instance;
    }

    //获取可用的飞碟
    public GameObject getUseableDisk()
    {
        for (int i = 0; i < diskList.Count; i++)
        {
            if (diskList[i].activeInHierarchy == false)
            {
                diskList[i].SetActive(true);
                return diskList[i];
            }
        }
        //如果没有可用的飞碟就创建新的飞碟对象
        diskList.Add(GameObject.Instantiate(disk) as GameObject);
        diskList[diskList.Count - 1].SetActive(true);
        return diskList[diskList.Count - 1];
    }

    //将用完的飞碟回收，并且设置为闲置状态
    public void free(GameObject disk)
    {
        //重置飞碟速度
        disk.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //重置飞碟大小
        disk.transform.localScale = disk.transform.localScale;
        //将飞碟设置为可用（闲置状态）
        disk.SetActive(false);
    }
    
    //返回工厂状态：是否有飞镖在空中（有的话就不能再发射飞镖，必须等到飞镖落地才能发射）
    public bool isLaunching()
    {
        for (int i = 0; i < diskList.Count; i++)
        {
            if(diskList[i].activeInHierarchy == true)
            {
                return true;
            }
        }
        return false;
    }

    //检查飞碟是否落地，如果飞碟落地就进行回收
    public void recycleLanded()
    {
        for (int i = 0; i < diskList.Count; i++)
        {
            if (diskList[i].transform.position.y <= -8)
            {
                free(diskList[i]);
            }
        }
    }
}

public class DiskFactoryBC : MonoBehaviour
{
    public GameObject disk;

    void Awake()
    {
        // 初始化预设对象  
        DiskFactory.getInstance().setDisk(disk);
    }
}