using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform head;
    public GameObject blockPrefab;
    public EblockType typeforNewBlocks;
    public List<EblockType> blockTypesIAmAllowedToUse;
    public GameObject previewBlock;
    public int numberOfCreatedBlocks;
    public GameObject menu;
    public GameObject saveGameText;

    // Start is called before the first frame update
    void Start()
    {
        SetTypeForNewBlocks(blockTypesIAmAllowedToUse.First());
        if (FindObjectOfType<MainMenuController>().shouldLoad == true)
        {
            LoadWorld();
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (menu.activeSelf) 
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenMenu();
        }

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
            ActivateMachineGun();
        }

        if(Input.GetMouseButtonUp(0))
        {
            DeactivateMachineGun();
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
                    if (blockHit.indestructible == false)
                    {
                        blockHit.SetHp(blockHit.hp - 1);
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
                GameObject spawnedBlockGO = Instantiate(blockPrefab);
                spawnedBlockGO.GetComponent<Block>().InitializeBlock(typeforNewBlocks);
                spawnedBlockGO.transform.position = blockHit.gameObject.transform.position + infoAboutTheRay.normal;
                numberOfCreatedBlocks = numberOfCreatedBlocks + 1;


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
            spawnedBlock.SetHp(serializableBlock.hp);
            spawnedBlock.indestructible = serializableBlock.indestructible;

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

        ActivateSaveText();

        Invoke(nameof(DeactivateSaveText), 3);

       
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
        previewBlock.GetComponent<Block>().InitializeBlock(eblockType);

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

    public void OpenMenu()
    {
        SetMenuVisible(true);
    }
    public void CloseMenu()
    {
        SetMenuVisible(false);
    }

    public void SetMenuVisible(bool isVisible) 
    {
        menu.SetActive(isVisible);
        gameObject.GetComponent<FirstPersonMovement>().enabled = !isVisible;
        gameObject.GetComponent<Jump>().enabled = !isVisible;
        gameObject.GetComponent<Crouch>().enabled = !isVisible;
        gameObject.GetComponentInChildren<FirstPersonLook>().enabled = !isVisible;
        gameObject.GetComponent<Rigidbody>().isKinematic = isVisible;

        Cursor.visible =  isVisible;

        Cursor.lockState = (isVisible) ? CursorLockMode.None : CursorLockMode.Locked;


    }

    public void ActivateSaveText()
    {
        saveGameText.SetActive(true);
    }
    public void DeactivateSaveText()
    {
        saveGameText.SetActive(false);
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
