using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Start()
    {
        //i want to add background music but im too tired to figure ts out >:(
        //audioSource.clip = background_msc;
        //audioSource.loop = true;
        //audioSource.Play();

        //to calvin: i also added the "yayyy" to be played at the end of waves
        //and the "oooo" to be played when the player loses/dies lol
    }
}