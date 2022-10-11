using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    public static GameManager Inst
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    public BattleManager battle;
    public Player player;
    public TextAsset CardJson, EffJson;
    public JArray cardJson, effJson;
    public Sprite CursorImg, AttackCursor;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 120;
        cardJson = JArray.Parse(CardJson.text);
        effJson = JArray.Parse(EffJson.text);
        Cursor.SetCursor(CursorImg.texture, Vector2.zero, CursorMode.ForceSoftware);
        player = FindObjectOfType<Player>();
        battle = FindObjectOfType<BattleManager>();
    }
}
