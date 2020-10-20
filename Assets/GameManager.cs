using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ConnectionType
{
    CLIENT,
    SERVER
}

public class GameManager : MonoBehaviour
{

    public const int PORT = 60977;
    public const int PACKET_LENGTH = 512;
    public string GAME_SCENE = "Game";

    public string IP = "127.0.0.1";

    public static GameManager Instance;

    public ConnectionType connectionType;

    public Dropdown dropdown;

    public Toggle toggle;

    public Button clientButton;

    public InputField clientInputField;

    void Start()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            optionDatas.Add(new Dropdown.OptionData($"{resolution.width}x{resolution.height}"));
        }
        dropdown.AddOptions(optionDatas);

        Screen.fullScreen = toggle.isOn;

        clientButton.onClick.AddListener(() => CreateClientGameScene(clientInputField.text));
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = toggle.isOn;
    }

    public void ChangeResolution()
    {
        string[] values = dropdown.options[dropdown.value].text.Split('x');
        int width = int.Parse(values[0]);
        int height = int.Parse(values[1]);
        Screen.SetResolution(width, height, toggle.isOn); 
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }

    public void CreateClientGameScene(string ip)
    {
        SceneManager.LoadScene(GAME_SCENE);
        connectionType = ConnectionType.CLIENT;
        IP = ip;
    }

    public void CreateServerGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE);
        connectionType = ConnectionType.SERVER;
    }

    public static float NotNull(float a, float b)
    {
        return a != 0 ? a : b;
    }
}
