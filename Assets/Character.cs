using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public Transform head;
    public GameObject blockPrefab;
    public EblockType typeforNewBlocks;
    public List<EblockType> blockTypesIAmAllowedToUse;
   // public GameObject previewBlock;

    // Start is called before the first frame update
    void Start()
    {
        SetTypeForNewBlocks(blockTypesIAmAllowedToUse.First());
    }

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority == false)
        {
            GetComponent<FirstPersonMovement>().enabled = false;
            GetComponent<Crouch>().enabled = false;
            GetComponent<Jump>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<FirstPersonLook>().enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (HasStateAuthority == false) { return; }


        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if(mouseWheel > 0)
        {
            IncrementTypeForNewBlocks();
        }
        if (mouseWheel < 0)
        {
            DecrementTypeForNewBlocks();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // ActivateMachineGun();
            PlaceABlockWithRaycast();
        }


        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = new Ray(head.position, head.forward);
            RaycastHit infoAboutTheRay;
            bool iHitSomething = Physics.Raycast(ray, out infoAboutTheRay);
            if (iHitSomething)
            {
                //Debug.Log(infoAboutTheRay.collider.gameObject.name);
                Block blockHit = infoAboutTheRay.collider.gameObject.GetComponent<Block>();
                if (blockHit != null)
                {
                    if (blockHit.Indestructible == 0)
                    {
                        blockHit.RPC_SetHp(blockHit.Hp - 1);
                    }
 
                }
            }

        }



    }

    private void PlaceABlockWithRaycast()
    {
        Ray ray = new Ray(head.position, head.forward);
        RaycastHit infoAboutTheRay;
        bool iHitSomething = Physics.Raycast(ray, out infoAboutTheRay);
        if (iHitSomething)
        {
            //Debug.Log(infoAboutTheRay.collider.gameObject.name);
            Block blockHit = infoAboutTheRay.collider.gameObject.GetComponent<Block>();
            if (blockHit != null)
            {
                FindObjectOfType<GameManager>().RPC_SpawnCube(blockHit.gameObject.transform.position + infoAboutTheRay.normal, (int)typeforNewBlocks, 0);
            }
        }
    }

    private void LoadWorld()
    {
        foreach(Block blockToDestroy in FindObjectsOfType<Block>())
        {
            if(blockToDestroy.gameObject.GetComponent<Rotator>() == null)
            {
                Destroy(blockToDestroy.gameObject);
            }
        }
        string jsonSaveFile = PlayerPrefs.GetString("Salvataggio");
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(jsonSaveFile);
        int i = 0;
        foreach(SerializableBlock serializableBlock in saveFile.serializableBlocks)
        {
            i++;
            GameObject spawnedBlockGO = Instantiate(blockPrefab);
            Block spawnedBlock = spawnedBlockGO.GetComponent<Block>();
            spawnedBlockGO.transform.position = serializableBlock.position;
            spawnedBlock.InitializeBlock(serializableBlock.blockType);
            spawnedBlock.RPC_SetHp(serializableBlock.hp);
            spawnedBlock.Indestructible = serializableBlock.indestructible;

        }
        Debug.Log(i);



    }

    public void SaveWorld()
    {
        SaveFile saveFile = new SaveFile();
        // saveFile.serializableBlocks = FindObjectsOfType<Block>().ToList().Select(x => x.GetSerializableBlock()).ToList();
        List<SerializableBlock> serializableBlocksInTheWorld = new List<SerializableBlock>();
        List<Block> blocksList = FindObjectsOfType<Block>().ToList();

        foreach (Block block in blocksList)
        {
            if (block.gameObject.GetComponent<Rotator>() == null)
            {
                serializableBlocksInTheWorld.Add(block.GetSerializableBlock());
            }
        }
        saveFile.serializableBlocks = serializableBlocksInTheWorld;

        string saveJson = JsonUtility.ToJson(saveFile);
        Debug.Log(saveJson);
        PlayerPrefs.SetString("Salvataggio", saveJson);
        PlayerPrefs.Save();


       
       // DeactivateSaveText();
    }


    private void IncrementTypeForNewBlocks()
    {
        EblockType firstElement = blockTypesIAmAllowedToUse.First();
        blockTypesIAmAllowedToUse.Remove(firstElement);
        blockTypesIAmAllowedToUse.Add(firstElement);
        SetTypeForNewBlocks(blockTypesIAmAllowedToUse.First());
      
    }

    private void SetTypeForNewBlocks(EblockType eblockType)
    {
        typeforNewBlocks = eblockType;
      //  previewBlock.GetComponent<Block>().InitializeBlock(eblockType);

    }

    private void DecrementTypeForNewBlocks()
    {
        EblockType lastElement = blockTypesIAmAllowedToUse.Last();
        blockTypesIAmAllowedToUse.Remove(lastElement);
        blockTypesIAmAllowedToUse.Insert(0,lastElement);
        SetTypeForNewBlocks(blockTypesIAmAllowedToUse.First());

    }

    public void QuitGame()
    {
        Application.Quit();
    }




    public void ActivateMachineGun()
    {
        InvokeRepeating(nameof(PlaceABlockWithRaycast), 0, 0.2f);
    }

    public void DeactivateMachineGun()
    {
        CancelInvoke(nameof(PlaceABlockWithRaycast));
    }
}
