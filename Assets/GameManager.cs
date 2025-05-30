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

    public IEnumerator InitializeTerrain()
    {
        for (int j = 0; j < 20; j++)
        {
            Debug.Log("j");
            for (int i = 0; i < 30; i++)
            {
                Debug.Log("i");
                SpawnCube(new Vector3(i,0,j), (int)EblockType.Erba,1);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    [Rpc(sources:RpcSources.All, targets:RpcTargets.StateAuthority)]
    public void RPC_SpawnCube(Vector3 position, int type, int indestructable)
    {
        Debug.Log(nameof(RPC_SpawnCube));
        SpawnCube(position, type, indestructable);
    }

    private void SpawnCube(Vector3 position, int type, int indestructable)
    {
        Debug.Log(nameof(SpawnCube) + position.ToString());
        NetworkObject spawnedBlockGO = Runner.Spawn(blockPrefab, position);
        spawnedBlockGO.GetComponent<Block>().InitializeBlock((EblockType)type);
        spawnedBlockGO.GetComponent<Block>().Indestructible = indestructable;
        spawnedBlockGO.transform.position = position;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
