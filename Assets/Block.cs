using Fusion;
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
    public int indestructible;
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

public class Block : NetworkBehaviour
{
    public List<BlockConfiguration> blockConfigurationDatabase;

    [Networked, OnChangedRender(nameof(RefreshBlockBrightness))]
    public int HpMax { get; set; }

    [Networked, OnChangedRender(nameof(RefreshBlockBrightness))]
    public int Hp { get; set; }

    [Networked, OnChangedRender(nameof(RefreshBlockAesthetic))]
    public int Type { get; set; }

    [Networked]
    public int Indestructible { get; set; }

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
        serializableBlock.hp = Hp;
        serializableBlock.indestructible = Indestructible;
        serializableBlock.blockType = (EblockType) Type;
        serializableBlock.position = transform.position;
        return serializableBlock;
    }

    public void InitializeBlock(EblockType blockType)
    {
        Type = (int)blockType;
        BlockConfiguration blockConfigurationSelected = blockConfigurationDatabase.Find(x => x.type == blockType);

        RefreshBlockAesthetic();
        HpMax = blockConfigurationSelected.hpMax;
        RPC_SetHp(HpMax);
    }

    public void RefreshBlockAesthetic()
    {
        BlockConfiguration blockConfigurationSelected = blockConfigurationDatabase.Find(x => x.type == (EblockType)Type);
        GetComponent<MeshRenderer>().material.mainTexture = blockConfigurationSelected.texture;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_SetHp(int hpToSet)
    {
        Hp = hpToSet;
        if (Hp <= 0)
        {
            ReplicatedDestroy();
        }
        else
        {
            RefreshBlockBrightness();
        }
    }

    public void LocalSetHp(int predictedHp)
    {
        if (predictedHp <= 0)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            RefreshBlockBrightnessWithParameter(predictedHp);
        }

    }

    public void ReplicatedDestroy()
    {
        Runner.Despawn(GetComponent<NetworkObject>());
    }

    public void RefreshBlockBrightness()
    {
        RefreshBlockBrightnessWithParameter(Hp);
    }

    public void RefreshBlockBrightnessWithParameter(int hpToShow)
    {
        GetComponent<MeshRenderer>().material.color = Color.white * ((float)hpToShow / HpMax);
    }

    public override void Spawned()
    {
        base.Spawned();
        RefreshBlockAesthetic();
        RefreshBlockBrightness();
    }
}
