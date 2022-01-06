using UnityEngine;

//This script behaves as a singleton.
//We can use the MonoSingleton script from the internet to do this.
//Or we can do a public static instance.  
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }

    [SerializeField] private Animator menuAnimator; 

    private void Awake()
    {
        Instance = this;
    }

    //Main Menu Buttons 
    public void OnLocalGameButton()
    {
        menuAnimator.SetTrigger("InGameMenu");
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    //Online Menu Buttons
    public void OnOnlineHostButton()
    {
        menuAnimator.SetTrigger("HostMenu");
    }
    public void OnOnlineConnectButton()
    {
        print("OnLocalGameButton"); //Needs additional logic to make it delay slightly.  In the video he put "// $$" I don't know what that means -- https://youtu.be/6_o7_fmk2os?t=1497
    }
    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }
    
    //Waitng For Connection Button
    public void OnHostBackButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }
}
