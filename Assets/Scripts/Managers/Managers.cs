using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    static Managers instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return instance; } } // 유일한 매니저를 갖고온다

    //#region Core
    //DataManager _data = new DataManager();
    //GameManager _game = new GameManager();
    //InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    //SoundManager _sound = new SoundManager();

    //public static DataManager Data { get { return Instance._data; } }
    //public static GameManager Game { get { return Instance._game; } }
    //public static InputManager Input { get { return Instance._input; } }
    //public static ResourceManager Resource { get { return Instance._resource; } }
    //public static SoundManager Sound { get { return Instance._sound; } }
    //#endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        //_input.OnUpdate();
        //_game.OnUpdate();
    }

    static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();

            //instance._data.Init();
            //instance._game.Init();
            //instance._sound.Init();
        }
    }

    public static void Clear()
    {

    }
}
