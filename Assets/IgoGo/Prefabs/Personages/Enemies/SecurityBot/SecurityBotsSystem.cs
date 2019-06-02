using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBotsSystem : MonoBehaviour {

    public GameObject bot;
    public Vector3Int size;

    private SecurityBot[,,] bots;

	// Use this for initialization
	void Start () {
        InstanceBots();
    }

    void Update () {
	}

    private void InstanceBots()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.size = size;
        box.center = new Vector3((size.x - 1) / 2, (size.y - 1) / 2, (size.z - 1) / 2);
        bots = new SecurityBot[size.x * 2 - 1, size.y * 2 - 1, size.z * 2 -1];
        Vector3 pos = new Vector3(0, 0, 0);
        for (int i = 0; i < bots.GetLength(0); i++)
        {
            pos = new Vector3(pos.x, 0, 0);
            for (int j = 0; j < bots.GetLength(1); j++)
            {
                pos = new Vector3(pos.x, pos.y, 0);
                for (int k = 0; k < bots.GetLength(2); k++)
                {
                    GameObject instanceBot = Instantiate(bot, transform);
                    instanceBot.transform.localPosition = pos;
                    pos += new Vector3(0, 0, 0.5f);
                    bots[i, j, k] = instanceBot.GetComponent<SecurityBot>();
                }
                pos += new Vector3(0, 0.5f, 0);
            }
            pos += new Vector3(0.5f, 0, 0);
        }
    }

    public void SetTarget(Vector3 target, CharacterReactions player)
    {
        for (int i = 0; i < bots.GetLength(0); i++)
        {
            for (int j = 0; j < bots.GetLength(1); j++)
            {
                for (int k = 0; k < bots.GetLength(2); k++)
                {
                    bots[i, j, k].target = target;
                    bots[i, j, k].player = player;
                }
            }
        }
    }

    public void DefaultActive()
    {
        for (int i = 0; i < bots.GetLength(0); i++)
        {
            for (int j = 0; j < bots.GetLength(1); j++)
            {
                for (int k = 0; k < bots.GetLength(2); k++)
                {
                    bots[i, j, k].ToDeault(); ;
                }
            }
        }
    }

    public void SetActive(int value)
    {
        for (int i = 0; i < bots.GetLength(0); i++)
        {
            for (int j = 0; j < bots.GetLength(1); j++)
            {
                for (int k = 0; k < bots.GetLength(2); k++)
                {
                    bots[i, j, k].move = value;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = new Vector3(transform.position.x + size.x / 2, transform.position.y + size.y / 2, transform.position.z + size.z / 2);
        Gizmos.DrawCube(pos, size);
    }
}
