using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    //correspond au type de son (si c'est juste un sound effect ou de la musique)
    public enum AudioTypes { soundEffect, music }
    public AudioTypes audioType;

    //le nom du son
    public string name;

    //Clip sonore qui va être joué
    public AudioClip clip;

    //sa source
    [HideInInspector]
    public AudioSource source;
    /*correspond à toutes les valeurs qui sont contenues dans l'audio clip : 
    le volume, le pitch, si le son est joué en loop, s'il est en train d'être joué, et si on le joue dès le départ*/
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public bool isLoop;
    public bool isPlaying;
    public bool playOnAwake;
}
