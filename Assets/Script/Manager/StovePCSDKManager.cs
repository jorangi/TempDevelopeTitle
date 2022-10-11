using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Stove.PCSDK.NET;

public struct StovePCError
{
    // ȣ��� �Լ��� ��Ÿ���� enum ��
    public StovePCFunctionType FunctionType;

    // �߻��� ���� ������ ��Ÿ���� enum ��
    public StovePCResult Result;

    // �߻��� ���� �޽���
    public string Message;

    // �ܺ� ����(http ����, �ܺ� ��� ����)�� �߻��� ���, �ش��ϴ� ���� �ڵ�
    public int ExternalError;
}
public class StovePCSDKManager : MonoBehaviour
{
    private Stove.PCSDK.NET.StovePCCallback callback;
    private Coroutine runcallbackCoroutine;

    StovePCConfig config = new()
    {
        Env = "live",
        AppKey = "8dd8732680c2d2599538bb7bef322d0fbc4e2b664c7aa22f9ae382a443d8fd08",
        AppSecret = "5b7428563b26ca5961dfea6e2b3c2f35e1dacd4ae375b9f46d0c04e38c6aa694c21c86a1a0159dd7a9e1ec5a1a7e3527",
        GameId = "	AAAtelier_FirstGame",
        LogLevel = StovePCLogLevel.Debug,
        LogPath = ""
    };

    public ulong LOGIN_USER_MEMBER_NO { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Initialize();
    }
    private void Initialize()
    {
        this.callback = new Stove.PCSDK.NET.StovePCCallback
        {
            OnError = new StovePCErrorDelegate(this.OnError),
            OnInitializationComplete = new StovePCInitializationCompleteDelegate(this.OnInitializationComplete),
            OnToken = new StovePCTokenDelegate(this.OnToken),
            OnUser = new StovePCUserDelegate(this.OnUser),
            OnOwnership = new StovePCOwnershipDelegate(this.OnOwnership),

            // ������������
            OnStat = new StovePCStatDelegate(this.OnStat),
            OnSetStat = new StovePCSetStatDelegate(this.OnSetStat),
            OnAchievement = new StovePCAchievementDelegate(this.OnAchievement),
            OnAllAchievement = new StovePCAllAchievementDelegate(this.OnAllAchievement),
            OnRank = new StovePCRankDelegate(this.OnRank)
        };

        StovePCResult sdkResult = StovePC.Initialize(config, callback);

        if (StovePCResult.NoError == sdkResult)
        {
            runcallbackCoroutine = StartCoroutine(RunCallback(0.5f));
            // �ʱ�ȭ ������ ����  RunCallback �ֱ���  ȣ��
        }
        else
        {
            // �ʱ�ȭ ���з� ���� ����
        }
    }
    private void OnRank(StovePCRank[] ranks, uint rankTotalCount)
    {
        // ���� ���� ���
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnRank");
        sb.AppendFormat(" - ranks.Length : {0}" + Environment.NewLine, ranks.Length);

        for (int i = 0; i < ranks.Length; i++)
        {
            sb.AppendFormat(" - ranks[{0}].MemberNo : {1}" + Environment.NewLine, i, ranks[i].MemberNo.ToString());
            sb.AppendFormat(" - ranks[{0}].Score : {1}" + Environment.NewLine, i, ranks[i].Score.ToString());
            sb.AppendFormat(" - ranks[{0}].Rank : {1}" + Environment.NewLine, i, ranks[i].Rank.ToString());
            sb.AppendFormat(" - ranks[{0}].Nickname : {1}" + Environment.NewLine, i, ranks[i].Nickname);
            sb.AppendFormat(" - ranks[{0}].ProfileImage : {1}" + Environment.NewLine, i, ranks[i].ProfileImage);
        }

        sb.AppendFormat(" - rankTotalCount : {0}", rankTotalCount);

        Debug.Log(sb.ToString());
    }
    private void OnAllAchievement(StovePCAchievement[] achievements)
    {
        // ��� ���� ���� ���
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnAllAchievement");
        sb.AppendFormat(" - achievements.Length : {0}" + Environment.NewLine, achievements.Length);

        for (int i = 0; i < achievements.Length; i++)
        {
            sb.AppendFormat(" - achievements[{0}].AchievementId : {1}" + Environment.NewLine, i, achievements[i].AchievementId);
            sb.AppendFormat(" - achievements[{0}].Name : {1}" + Environment.NewLine, i, achievements[i].Name);
            sb.AppendFormat(" - achievements[{0}].Description : {1}" + Environment.NewLine, i, achievements[i].Description);
            sb.AppendFormat(" - achievements[{0}].DefaultImage : {1}" + Environment.NewLine, i, achievements[i].DefaultImage);
            sb.AppendFormat(" - achievements[{0}].AchievedImage : {1}" + Environment.NewLine, i, achievements[i].AchievedImage);
            sb.AppendFormat(" - achievements[{0}].Condition.GoalValue : {1}" + Environment.NewLine, i, achievements[i].Condition.GoalValue.ToString());
            sb.AppendFormat(" - achievements[{0}].Condition.ValueOperation : {1}" + Environment.NewLine, i, achievements[i].Condition.ValueOperation);
            sb.AppendFormat(" - achievements[{0}].Condition.Type : {1}" + Environment.NewLine, i, achievements[i].Condition.Type);
            sb.AppendFormat(" - achievements[{0}].Value : {1}" + Environment.NewLine, i, achievements[i].Value.ToString());
            sb.AppendFormat(" - achievements[{0}].Status : {1}", i, achievements[i].Status);

            if (i < achievements.Length - 1)
                sb.AppendFormat(Environment.NewLine);
        }

        Debug.Log(sb.ToString());
    }
    private void OnAchievement(StovePCAchievement achievement)
    {
        // ���� ���� ���� ���
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnAchievement");
        sb.AppendFormat(" - achievement.AchievementId : {0}" + Environment.NewLine, achievement.AchievementId);
        sb.AppendFormat(" - achievement.Name : {0}" + Environment.NewLine, achievement.Name);
        sb.AppendFormat(" - achievement.Description : {0}" + Environment.NewLine, achievement.Description);
        sb.AppendFormat(" - achievement.DefaultImage : {0}" + Environment.NewLine, achievement.DefaultImage);
        sb.AppendFormat(" - achievement.AchievedImage : {0}" + Environment.NewLine, achievement.AchievedImage);
        sb.AppendFormat(" - achievement.Condition.GoalValue : {0}" + Environment.NewLine, achievement.Condition.GoalValue.ToString());
        sb.AppendFormat(" - achievement.Condition.ValueOperation : {0}" + Environment.NewLine, achievement.Condition.ValueOperation);
        sb.AppendFormat(" - achievement.Condition.Type : {0}" + Environment.NewLine, achievement.Condition.Type);
        sb.AppendFormat(" - achievement.Value : {0}" + Environment.NewLine, achievement.Value.ToString());
        sb.AppendFormat(" - achievement.Status : {0}", achievement.Status);

        Debug.Log(sb.ToString());
    }
    private void OnSetStat(StovePCStatValue statValue)
    {
        // ���� ������Ʈ ��� ���� ���
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnSetStat");
        sb.AppendFormat(" - statValue.CurrentValue : {0}" + Environment.NewLine, statValue.CurrentValue.ToString());
        sb.AppendFormat(" - statValue.Updated : {0}" + Environment.NewLine, statValue.Updated.ToString());
        sb.AppendFormat(" - statValue.ErrorMessage : {0}", statValue.ErrorMessage);

        Debug.Log(sb.ToString());
    }
    private void OnStat(StovePCStat stat)
    {
        // ���� ���� ���
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnStat");
        sb.AppendFormat(" - stat.StatFullId.GameId : {0}" + Environment.NewLine, stat.StatFullId.GameId);
        sb.AppendFormat(" - stat.StatFullId.StatId : {0}" + Environment.NewLine, stat.StatFullId.StatId);
        sb.AppendFormat(" - stat.MemberNo : {0}" + Environment.NewLine, stat.MemberNo.ToString());
        sb.AppendFormat(" - stat.CurrentValue : {0}" + Environment.NewLine, stat.CurrentValue.ToString());
        sb.AppendFormat(" - stat.UpdatedAt : {0}", stat.UpdatedAt.ToString());

        Debug.Log(sb.ToString());
    }
    private IEnumerator RunCallback(float intervalSeconds)
    {
        WaitForSeconds wfs = new WaitForSeconds(intervalSeconds);
        while (true)
        {
            StovePC.RunCallback();
            yield return wfs;
        }
    }
    private void OnOwnership(StovePCOwnership[] ownerships)
    {
        bool owned = false;

        foreach (var ownership in ownerships)
        {
            // [LOGIN_USER_MEMBER_NO] StovePCUser ����ü�� MemberNo
            // [OwnershipCode] 1: ������ ȹ��, 2: ������ ����(���� ����� ���)
            if (ownership.MemberNo != LOGIN_USER_MEMBER_NO ||
                ownership.OwnershipCode != 1)
            {
                continue;
            }

            // [GameCode] 3: BASIC ����, 5: DLC
            if (ownership.GameId == "YOUR_GAME_ID" &&
                ownership.GameCode == 3)
            {
                owned = true; // ������ Ȯ�� ���� true�� ����
            }

            // DLC�� �Ǹ��ϴ� ������ ���� �ʿ�
            if (ownership.GameId == "YOUR_DLC_ID" &&
                ownership.GameCode == 5)
            {
                // YOUR_DLC_ID(DLC) �������� �ֱ⿡ DLC �÷��� ���
            }
        }

        if (owned)
        {
            // ������ ������ ���������� �Ϸ� �� ���� �������� ���� �ۼ�
        }
        else
        {
            // ������ �������� �� ������ �����ϰ� �����޼��� ǥ�� ���� �ۼ�
        }
    }
    private void OnUser(StovePCUser user)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnUser");
        sb.AppendFormat(" - user.MemberNo : {0}" + Environment.NewLine, user.MemberNo.ToString());
        sb.AppendFormat(" - user.Nickname : {0}" + Environment.NewLine, user.Nickname);
        sb.AppendFormat(" - user.GameUserId : {0}", user.GameUserId);
        LOGIN_USER_MEMBER_NO = user.MemberNo;
        Debug.Log(sb.ToString());
    }
    private void OnToken(StovePCToken token)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnToken");
        sb.AppendFormat(" - token.AccessToken : {0}", token.AccessToken);

        Debug.Log(sb.ToString());
    }
    private void OnInitializationComplete()
    {
        Debug.Log("PC SDK initialization success");
    }
    private void OnError(Stove.PCSDK.NET.StovePCError error)
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnError");
        sb.AppendFormat(" - error.FunctionType : {0}" + Environment.NewLine, error.FunctionType.ToString());
        sb.AppendFormat(" - error.Result : {0}" + Environment.NewLine, (int)error.Result);
        sb.AppendFormat(" - error.Message : {0}" + Environment.NewLine, error.Message);
        sb.AppendFormat(" - error.ExternalError : {0}", error.ExternalError.ToString());
        Debug.Log(sb.ToString());
        #endregion

        switch (error.FunctionType)
        {
            case StovePCFunctionType.Initialize:
            case StovePCFunctionType.GetUser:
            case StovePCFunctionType.GetOwnership:
                BeginQuitAppDueToError();
                break;
        }
    }
    private void BeginQuitAppDueToError()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("BeginQuitAppDueToError");
        sb.AppendFormat(" - nothing");
        Debug.Log(sb.ToString());
        #endregion

        // ��¼�� ����� ��� ���� �ߴ��ϱ⺸�ٴ� ����ڿ��� �� �ߴܿ� ���� �޽����� ������ ��
        // ����� �׼�(e.g. ���� ��ư Ŭ��)�� ���� ���� �ߴ��ϰ� �;� ������ �𸨴ϴ�.
        // �׷��ٸ� ���⿡ QuitApplication�� ����� ��Ÿ��� ������ �����Ͻʽÿ�.
        // �����ϴ� �ʼ� ���� �۾� ������ ���� �޽����� �Ʒ��� �����ϴ�.
        // �ѱ��� : �ʼ� ���� �۾��� �����Ͽ� ������ �����մϴ�.
        // �� �� ��� : The required pre-task fails and exits the game.
        QuitApplication();
    }
    private void QuitApplication()
    {
        Debug.Log("���� ���� �޼ҵ�");
    }
}
public class StovePCCallback
{
    // StovePCSDK ���� �����߻��� ȣ��Ǵ� �ݹ�
    public StovePCErrorDelegate OnError;

    // PC SDK �ʱ�ȭ�� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCInitializationCompleteDelegate OnInitializationComplete;

    // GetToken ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCTokenDelegate OnToken;

    // GetUser ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCUserDelegate OnUser;

    // GetOwnership ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCOwnershipDelegate OnOwnership;

    // GetStat ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCStatDelegate OnStat;

    // SetStat ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCSetStatDelegate OnSetStat;

    // GetAchievement ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCAchievementDelegate OnAchievement;

    // GetAllAchievement ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCAllAchievementDelegate OnAllAchievement;

    // GetRank ó���� �Ϸ���� �� ȣ��Ǵ� �ݹ�
    public StovePCRankDelegate OnRank;
}
