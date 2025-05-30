using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkObject blockPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitializeTerrain()
    {
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                RPC_SpawnCube(new Vector3(i,0,j), EblockType.Erba,1);
            }
        }
    }

    [Rpc(sources:RpcSources.All, targets:RpcTargets.StateAuthority)]
    public void RPC_SpawnCube(Vector3 position, EblockType type, int indestructable)
    {
        NetworkObject spawnedBlockGO = Runner.Spawn(blockPrefab, position);
        spawnedBlockGO.GetComponent<Block>().InitializeBlock(type);
        spawnedBlockGO.GetComponent<Block>().Indestructible = indestructable;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
