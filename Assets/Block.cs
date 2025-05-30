using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public List<SerializableBlock> serializableBlocks;
}

[Serializable]
public class SerializableBlock
{
    public Vector3 position;
    public EblockType blockType;
    public bool indestructible;
    public int hp;
}

public enum EblockType
{
    Erba,
    Terra,
    Pietra,
    Ferro,
    Diamante,
    Legno,
    Oro
}
[Serializable]
public class BlockConfiguration
{
    public EblockType type;
    public int hpMax;
    public Texture texture;
    public AudioClip hitClip;

}

public class Block : MonoBehaviour
{
    public List<BlockConfiguration> blockConfigurationDatabase;
    public int hpMax;
    public int hp;
    public EblockType type;
    public bool indestructible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SerializableBlock GetSerializableBlock()
    {
        SerializableBlock serializableBlock = new SerializableBlock();
        serializableBlock.hp = hp;
        serializableBlock.indestructible = indestructible;
        serializableBlock.blockType = type;
        serializableBlock.position = transform.position;
        return serializableBlock;
    }

    public void InitializeBlock(EblockType blockType)
    {
        type = blockType;
        BlockConfiguration blockConfigurationSelected = blockConfigurationDatabase.Find(x => x.type == blockType);
        hpMax = blockConfigurationSelected.hpMax;
        SetHp(hpMax);
        GetComponent<MeshRenderer>().material.mainTexture = blockConfigurationSelected.texture;
    }
    public void SetHp(int hpToSet)
    {
        hp = hpToSet;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.white * ((float)hp / hpMax);
        }
       
    }
}
