using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory {
    private static MonsterFactory _instance;
    private List<GameObject> monsters;
    private GameObject dragon;

    public static MonsterFactory getInstance() {
        if (_instance == null)
        {
            _instance = new MonsterFactory();
        }
        return _instance;
    }

    public void setPrefabs(GameObject prefab)
    {
        dragon = prefab;
        monsters = new List<GameObject>();
    }

    public GameObject getMonster()
    {
        for (int num = 0; num < monsters.Count; num++)
        {
            if (monsters[num].activeInHierarchy == false)
            {
                monsters[num].SetActive(true);
                return monsters[num];
            }
        }
        Debug.Log("tets");
        monsters.Add(GameObject.Instantiate(dragon) as GameObject);
        monsters[monsters.Count - 1].SetActive(true);
        return monsters[monsters.Count - 1];
    }

    // 由于不需要销毁对象，所以free不用写
    public void free(GameObject temp)
    {
        temp.SetActive(false);
    }
}
