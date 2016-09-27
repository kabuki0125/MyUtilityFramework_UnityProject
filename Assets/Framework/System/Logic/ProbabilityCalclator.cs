using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// クラス：確率計算.
/// </summary>
public class ProbabilityCalclator
{
    /// <summary>
    /// 指定のID,確率ペアリストの中からIDの抽選を行う.リストの中から必ず一つ抽選される.
    /// </summary>
    /// <returns>抽選の結果選ばれたID.</returns>
    /// <param name="probList">IDと確率のペアリスト.このリストの確率を見て各IDの抽選を行う.</param>
    public static int LotteryID(List<KeyValuePair<int/*id*/,int/*probalility*/>> probList)
    {
        // トータルの値(分母)の抽出.
        var totalProb = 0;
        foreach(var info in probList){
            totalProb += info.Value;
        }
        // 値の抽選.
        var val = Random.Range(0, totalProb);
        // IDの抽選
        var prev = 0;
        foreach(var info in probList){
            if(val < prev+info.Value){
                return info.Key;
            }
            prev += info.Value;
        }
        Debug.LogError("[ProbabilityCalclator] LotteryID Error!! : Lotto logic is faulty.");
        return 0;
    }
    
    /// <summary>
    /// 指定された確率で当たるかどうかの単発抽選.
    /// </summary>
    /// <returns>抽選されたID.外れた場合は−1.</returns>
    /// <param name="probability">確率の値.100を上限とするint値.</param>
    public static bool Lottery(int probability)
    {
        var val = Random.Range(0, 100);
        return val < probability;
    }
}
