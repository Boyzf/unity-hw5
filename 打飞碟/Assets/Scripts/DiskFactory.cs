using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFactory : MonoBehaviour {
	public GameObject disk_prefab = null;

    public List<DiskData> used = new List<DiskData>();
    public List<DiskData> free = new List<DiskData>();

    public GameObject GetDisk(int round)
    {
        int choice = 0;
        int scope1 = 1, scope2 = 4, scope3 = 7;
        float start_y = -10f;
        string tag;
        disk_prefab = null;

        if (round == 1)
        {
            choice = Random.Range(0, scope1);
        }
        else if(round == 2)
        {
            choice = Random.Range(0, scope2);
        }
        else
        {
            choice = Random.Range(0, scope3);
        }
        tag = "blueDisk";
        if(choice <= scope1)
        {
            tag = "redDisk";
        }
        if(choice <= scope2 && choice > scope1)
        {
            tag = "yellowDisk";
        }
        if(choice <= scope3 && choice > scope2)
        {
            tag = "blueDisk";
        }
        for(int i = 0; i < free.Count; i++)
        {
            if(free[i].tag == tag)
            {
                disk_prefab = free[i].gameObject;
                free.Remove(free[i]);
                break;
            }
        }
        if(disk_prefab == null)
        {
            if (tag == "redDisk")
            {
                disk_prefab = Instantiate(Resources.Load<GameObject>("redDisk"), new Vector3(0, start_y, 0), Quaternion.identity);
            }
            else if (tag == "yellowDisk")
            {
                disk_prefab = Instantiate(Resources.Load<GameObject>("yellowDisk"), new Vector3(0, start_y, 0), Quaternion.identity);
            }
            else
            {
                disk_prefab = Instantiate(Resources.Load<GameObject>("blueDisk"), new Vector3(0, start_y, 0), Quaternion.identity);
            }
        }
        used.Add(disk_prefab.GetComponent<DiskData>());
        return disk_prefab;
    }

    public void FreeDisk(GameObject disk) {
        for(int i = 0; i < used.Count; i++) {
            if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID()) {
                used[i].gameObject.SetActive(false);
                free.Add(used[i]);
                used.Remove(used[i]);
                break;
            }
        }
    }

}
