using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MyLibrary.Unity;

/// <summary>
/// クラス：ローカルデータ管理.
/// </summary>
public class LocalDataManager
{
    /// <summary>データ.</summary>
    public static LocalData Data { get; private set; }
    
    /// <summary>
    /// データを保存.
    /// </summary>
    public static void Save()
    {
        Data.BundleVersion = DefinePlayerSettings.BUNDLE_VERSION;   // 毎回バージョンを登録.
        
        var fileName = "LocalData";
        
        FileUtility.Delete(GameSystem.CachesDirectoryPath+"/0002/"+fileName); // すでにある場合は都度上書きする.
        FileUtility.CreateDirectory(GameSystem.CachesDirectoryPath+"/0002/");
        
        var strJson = JsonFx.Json.JsonWriter.Serialize(Data);
        var byteJson = Encoding.UTF8.GetBytes(strJson);
        FileUtility.WriteToFileWith3DES(byteJson, GameSystem.CachesDirectoryPath+"/0002/"+fileName);
    }
    /// <summary>
    /// データを読み込み.
    /// </summary>
    public static LocalData Load()
    {
        var fileName = "LocalData";
        var byteJson = FileUtility.ReadFromFileWith3DES(GameSystem.CachesDirectoryPath+"/0002/"+fileName);
        if(byteJson != null){
            var strJson = Encoding.UTF8.GetString(byteJson);
            Data = JsonFx.Json.JsonReader.Deserialize<LocalData>(strJson);
        }else{
            Debug.Log("[LocalDataManager] Load : not found file. create new data.");
            Data = new LocalData();
        }
        return Data;
    }
}

/// <summary>
/// ローカルデータ
/// </summary>
public class LocalData
{
    /// <summary>初回起動済み？.</summary>
    public bool IsAlreadyBoot { get; set; }
    
    /// <summary>このローカルデータを保存した際のバンドルバージョン(PlayerSettingsのbundleVersion).</summary>
    public string BundleVersion { get; set; }
    
    /// <summary>BGM音量.</summary>
    public float Volume_BGM { get; set; }
    /// <summary>SE音量.</summary>
    public float Volume_SE { get; set; }
    
    /// <summary>通知設定.</summary>
    public bool IsNotificate { get; set; }
    
    
    /// <summary>
    /// データのクリア、リセット.
    /// </summary>
    public void Clear()
    {
        Volume_BGM = 0.5f;
        Volume_SE = 0.5f;
        IsNotificate = true;
    }
    
    public LocalData()
    {
        this.Clear();
    }
}
