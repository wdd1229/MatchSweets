using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager : Singleton<SDKManager>
{

    //private readonly static string TAG = "SDKManager";

    //private TTSDK.TTGameRecorder m_TTGameRecorder;

    ///// <summary>
    ///// 是否录制声音
    ///// </summary>
    //private bool m_IsRecordAudio=true;

    ///// <summary>
    ///// 录屏最大时长
    ///// </summary>
    //private int m_MaxRecordTime;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    TT.InitSDK((code, env) =>
    //    {
    //        Debug.Log("Unity message init sdk callback");
    //        Debug.Log("Unity message code: " + code);
    //        Debug.Log("Unity message HostEnum: " + env.m_HostEnum);
    //        Debug.Log("Unity message AppId: " + env.GameAppId);
    //    });

    //    Debug.LogError("是否在TT Container真机环境下：" + TT.InContainerEnv);

    //    //TT.CheckScene(TTSideBar.SceneEnum.SideBar, b =>
    //    //{
    //    //    Debug.Log("check scene success，" + b);

    //    //    var data = new JsonData
    //    //    {
    //    //        ["scene"] = "sidebar",
    //    //    };
    //    //    TT.NavigateToScene(data, () =>
    //    //    {
    //    //        Debug.Log("navigate to scene success");
    //    //    }, () =>
    //    //    {
    //    //        Debug.Log("navigate to scene complete");
    //    //    }, (errCode, errMsg) =>
    //    //    {
    //    //        Debug.Log($"navigate to scene error, errCode:{errCode}, errMsg:{errMsg}");
    //    //    });


    //    //}, () =>
    //    //{
    //    //    Debug.Log("check scene complete");
    //    //}, (errCode, errMsg) =>
    //    //{
    //    //    Debug.Log($"check scene error, errCode:{errCode}, errMsg:{errMsg}");
    //    //});



    //    //m_TTGameRecorder=TT.GetGameRecorder();

    //    //Debug.LogError($"当前录屏状态：{m_TTGameRecorder.GetEnabled()}");

    //    //m_TTGameRecorder.Start(m_IsRecordAudio,
    //    //    m_MaxRecordTime,
    //    //    OnRecordStart,
    //    //    OnRecordError,
    //    //    OnRecordTimeout);

      
    //}

    //public void StopRecord()
    //{
    //    m_TTGameRecorder.Stop(OnRecordComplete, OnRecordError);

    //    ///录屏时长 int
    //    m_TTGameRecorder.GetRecordDuration();


    //    Debug.LogError($"video record state: {m_TTGameRecorder.GetVideoRecordState()}");
    //}

    //private void OnRecordStart()
    //{
    //    Debug.LogError($"{TAG}  OnRecordStart");
    //}

    //private void OnRecordError(int errCode, string errMsg)
    //{
    //    Debug.LogError($"{TAG}  OnRecordError - errCode: {errCode}, errMsg: {errMsg}");
    //    StarkUIManager.ShowToast($"OnRecordError - errCode: {errCode}, errMsg: {errMsg}");
    //}

    //private void OnRecordTimeout(string videoPath)
    //{
    //    Debug.LogError($"{TAG}  OnRecordTimeout - videoPath: {videoPath}, video duration: {m_TTGameRecorder.GetRecordDuration() / 1000.0f} s");
    //}

    //void OnRecordComplete(string videoPath)
    //{
    //    Debug.LogError($"{TAG}  OnRecordComplete - videoPath: {videoPath}, video duration: {m_TTGameRecorder.GetRecordDuration() / 1000.0f} s");
    //}
}
