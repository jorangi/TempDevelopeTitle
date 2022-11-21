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
    public Transform giftUI;
    public GameObject giftPrefab;
    public List<GiftBase> gifts;
    public TextAsset CardJson, EffJson, CharacterJson;
    public JArray cardJson, effJson, characterJson;
    public Texture2D CursorImg, AttackCursor;

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
        characterJson = JArray.Parse(CharacterJson.text);
        Cursor.SetCursor(CursorImg, Vector2.zero, CursorMode.ForceSoftware);
        player = FindObjectOfType<Player>();
        battle = FindObjectOfType<BattleManager>();
    }
    public void AcquireGift(string gift)
    {
        GameObject obj = Instantiate(giftPrefab, giftUI);
        obj.name = gift;
        obj.AddComponent(System.Type.GetType(gift));
    }
    void OnPreCull() => GL.Clear(true, true, Color.black);
}
