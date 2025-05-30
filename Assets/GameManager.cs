using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject blockPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<MainMenuController>().shouldLoad==false)
        {
            for (int j = 0; j < 30; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    GameObject spawnedBlockGO = Instantiate(blockPrefab);
                    spawnedBlockGO.transform.position = new Vector3(j, 0, i);
                    spawnedBlockGO.GetComponent<Block>().InitializeBlock(EblockType.Erba);
                    spawnedBlockGO.GetComponent<Block>().indestructible = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
