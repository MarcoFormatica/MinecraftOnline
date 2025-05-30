using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Registro
{
    public List<Persona> persone;
}

[Serializable]
public class Persona 
{
    public string nome;
    public string cognome;
    public int giorno;
    public int mese;
    public int anno;
    public string luogoDiNascita;

    public List<Animale> listaDiAnimali;
}
[Serializable]
public class Animale
{
    public string nomeAnimale;
    public int annoAnimale;
    public int meseAnimale;
    public int giornoAnimale;
    public Vector3 pos;
}

public class Test : MonoBehaviour
{
    public Registro registro;
    // Start is called before the first frame update
    void Start()
    {
        /* 
                string jsonRegistro = PlayerPrefs.GetString("Registro");
                registro = JsonUtility.FromJson<Registro>(jsonRegistro);
           
        string jsonRegistro = JsonUtility.ToJson(registro);
        Debug.Log(jsonRegistro);
        PlayerPrefs.SetString("Registro", jsonRegistro);
        PlayerPrefs.Save();     */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
