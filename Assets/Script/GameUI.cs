using System;
using TMPro;
using UnityEngine;

public enum CameraAngle
{
    menu = 0,
    whiteTeam = 1, 
    blackTeam = 2
}

//This script behaves as a singleton.
//We can use the MonoSingleton script from the internet to do this.
//Or we can do a public static instance.  
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }

    public Server server; 
    public Client client;

    [SerializeField] private Animator menuAnimator; 
    [SerializeField] private TMP_InputField addressInput; 
    [SerializeField] private GameObject[] cameraAngles; 

    public Action<bool> SetLocalGame; 

    private void Awake()
    {
        Instance = this;
        RegisterEvents();
        //UnregisterEvents?
    }

    //CAMERAS
    public void ChangeCamera(CameraAngle index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
            cameraAngles[i].SetActive(false); 

        cameraAngles[(int)index].SetActive(true);
    }

    //MAIN MENU BUTTONS
    public void OnLocalGameButton()
    {
        SetLocalGame?.Invoke(true);
        server.Init(8007); //Change this later.  This is arbitrary.
        client.Init("127.0.0.1", 8007); 
        menuAnimator.SetTrigger("InGameMenu");
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    //ONLINE MENU BUTTONS
    public void OnOnlineHostButton()
    {
        SetLocalGame?.Invoke(false);
        server.Init(8007); //Change this later.  This is arbitrary.
        client.Init("127.0.0.1", 8007); 
        menuAnimator.SetTrigger("HostMenu");
    }
    public void OnOnlineConnectButton()
    {
        SetLocalGame?.Invoke(false);
        client.Init(addressInput.text, 8007);
        //print("OnLocalGameButton"); //Needs additional logic to make it delay slightly.  In the video he put "// $$" I don't know what that means -- https://youtu.be/6_o7_fmk2os?t=1497
    }
    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }
    
    //WAITING FOR CONNECTION BUTTON 
    public void OnHostBackButton()
    {
        server.ShutDown();
        client.ShutDown();
        menuAnimator.SetTrigger("OnlineMenu");
    }

    //LISTEN FOR NETWORK EVENTS
    //Used to turn off the UI 
    #region 
    //REGISTER EVENTS
    private void RegisterEvents()
    {        
        NetUtility.C_START_GAME += OnStartClient; 
    }
    private void UnregisterEvents()
    {
        NetUtility.C_START_GAME -= OnStartClient; 
    }

    //TRIGGER THE ANIMATOR EVENT
    private void OnStartClient(NetMessage obj)
    {
        menuAnimator.SetTrigger("InGameMenu");
    }
    #endregion
}
