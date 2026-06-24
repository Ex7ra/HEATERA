using UnityEngine;
using UnityEngine.Video;


public class TV_Manager : MonoBehaviour
{
    public VideoPlayer TV;//for TVplayer object
    public VideoClip[] videos; // an array of video clips
    private int currentIndex = 0;// 0 is index, so by defualt video with index 0 will play 
    void Start()
    {
       if(videos.Length > 0)//only plays if there is at least one video in the array
        {
            TV.clip = videos[currentIndex];//sets the clip/video currently will play
            TV.Play();//tells the VideoPlayer to start playback
        }
    }
    public void next_forw()
    {
        if (videos.Length == 0) return;//tells: if there are no video, stop this method immediatly
        currentIndex++;//increases index by 1

        if(currentIndex >= videos.Length)//this means if we moved past the last video, go back to the first one
            currentIndex = 0;//
        
        TV.clip = videos[currentIndex];//sets the current clip 
        TV.Play();//plays it 
    }    
    public void next_back()
    {
        if (videos.Length == 0) return;
        currentIndex--;

        if(currentIndex < 0)
            currentIndex = videos.Length - 1;//this line means: jump to the last video in the list
        
        TV.clip = videos[currentIndex];
        TV.Play();
    }   
}
