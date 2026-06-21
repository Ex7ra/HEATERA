using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class TV_Manager : MonoBehaviour
{
    public VideoPlayer TV;
    public VideoClip[] videos;
    private int currentIndex = 0;
    void Start()
    {
       if(videos.Length > 0)
        {
            TV.clip = videos[currentIndex];
            TV.Play();
        }
    }
    public void next_forw()
    {
        if (videos.Length == 0) return;
        currentIndex++;

        if(currentIndex >= videos.Length)
            currentIndex = 0;
        
        TV.clip = videos[currentIndex];
        TV.Play();
    }    
    public void next_back()
    {
        if (videos.Length == 0) return;
        currentIndex--;

        if(currentIndex < 0)
            currentIndex = videos.Length - 1;
        
        TV.clip = videos[currentIndex];
        TV.Play();
    }   
}
