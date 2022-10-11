using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Stove.PCSDK.NET;

public struct StovePCError
{
    // 호출된 함수를 나타내는 enum 값
    public StovePCFunctionType FunctionType;

    // 발생한 에러 유형을 나타내는 enum 값
    public StovePCResult Result;

    // 발생한 에러 메시지
    public string Message;

    // 외부 에러(http 에러, 외부 모듈 에러)가 발생한 경우, 해당하는 에러 코드
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

            // 게임지원서비스
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
            // 초기화 오류가 없어  RunCallback 주기적  호출
        }
        else
        {
            // 초기화 실패로 게임 종료
        }
    }
    private void OnRank(StovePCRank[] ranks, uint rankTotalCount)
    {
        // 순위 정보 출력
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
        // 모든 업적 정보 출력
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
        // 단일 업적 정보 출력
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
        // 스탯 업데이트 결과 정보 출력
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnSetStat");
        sb.AppendFormat(" - statValue.CurrentValue : {0}" + Environment.NewLine, statValue.CurrentValue.ToString());
        sb.AppendFormat(" - statValue.Updated : {0}" + Environment.NewLine, statValue.Updated.ToString());
        sb.AppendFormat(" - statValue.ErrorMessage : {0}", statValue.ErrorMessage);

        Debug.Log(sb.ToString());
    }
    private void OnStat(StovePCStat stat)
    {
        // 스탯 정보 출력
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
            // [LOGIN_USER_MEMBER_NO] StovePCUser 구조체의 MemberNo
            // [OwnershipCode] 1: 소유권 획득, 2: 소유권 해제(구매 취소한 경우)
            if (ownership.MemberNo != LOGIN_USER_MEMBER_NO ||
                ownership.OwnershipCode != 1)
            {
                continue;
            }

            // [GameCode] 3: BASIC 게임, 5: DLC
            if (ownership.GameId == "YOUR_GAME_ID" &&
                ownership.GameCode == 3)
            {
                owned = true; // 소유권 확인 변수 true로 설정
            }

            // DLC를 판매하는 게임일 때만 필요
            if (ownership.GameId == "YOUR_DLC_ID" &&
                ownership.GameCode == 5)
            {
                // YOUR_DLC_ID(DLC) 소유권이 있기에 DLC 플레이 허용
            }
        }

        if (owned)
        {
            // 소유권 검증이 정상적으로 완료 된 이후 게임진입 로직 작성
        }
        else
        {
            // 소유권 검증실패 후 게임을 종료하고 에러메세지 표출 로직 작성
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

        // 어쩌면 당신은 즉시 앱을 중단하기보다는 사용자에게 앱 중단에 대한 메시지를 보여준 후
        // 사용자 액션(e.g. 종료 버튼 클릭)에 따라 앱을 중단하고 싶어 할지도 모릅니다.
        // 그렇다면 여기에 QuitApplication을 지우고 당신만의 로직을 구현하십시오.
        // 권장하는 필수 사전 작업 오류에 대한 메시지는 아래와 같습니다.
        // 한국어 : 필수 사전 작업이 실패하여 게임을 종료합니다.
        // 그 외 언어 : The required pre-task fails and exits the game.
        QuitApplication();
    }
    private void QuitApplication()
    {
        Debug.Log("게임 종료 메소드");
    }
}
public class StovePCCallback
{
    // StovePCSDK 에서 에러발생시 호출되는 콜백
    public StovePCErrorDelegate OnError;

    // PC SDK 초기화가 완료됐을 때 호출되는 콜백
    public StovePCInitializationCompleteDelegate OnInitializationComplete;

    // GetToken 처리가 완료됐을 때 호출되는 콜백
    public StovePCTokenDelegate OnToken;

    // GetUser 처리가 완료됐을 때 호출되는 콜백
    public StovePCUserDelegate OnUser;

    // GetOwnership 처리가 완료됐을 때 호출되는 콜백
    public StovePCOwnershipDelegate OnOwnership;

    // GetStat 처리가 완료됐을 때 호출되는 콜백
    public StovePCStatDelegate OnStat;

    // SetStat 처리가 완료됐을 때 호출되는 콜백
    public StovePCSetStatDelegate OnSetStat;

    // GetAchievement 처리가 완료됐을 때 호출되는 콜백
    public StovePCAchievementDelegate OnAchievement;

    // GetAllAchievement 처리가 완료됐을 때 호출되는 콜백
    public StovePCAllAchievementDelegate OnAllAchievement;

    // GetRank 처리가 완료됐을 때 호출되는 콜백
    public StovePCRankDelegate OnRank;
}
