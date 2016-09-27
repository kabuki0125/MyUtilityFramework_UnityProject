using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// タグ、レイヤー、シーン、ソーティングレイヤー名を定数で管理するクラスを自動で作成するスクリプト
/// </summary>
public class SettingClassCreator : AssetPostprocessorEx
{
    
    //変更を監視するディレクトリ名
    private const string TARGET_DIRECTORY_NAME = "ProjectSettings";
    
    //ProjectSettings以下の設定が編集されたら自動で各スクリプトを作成
    private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        List<string[]> assetsList = new List<string[]> (){
            importedAssets
        };
        
        List<string> targetDirectoryNameList = new List<string> (){
            TARGET_DIRECTORY_NAME
        };
        
        if(ExistsDirectoryInAssets(assetsList, targetDirectoryNameList)){
            Create ();
        }
    }
    
    //スクリプトを作成します
    [MenuItem("Tools/Create/Create System Define Class")]
    private static void Create()
    {
        //タグ
        Dictionary<string, string> tagDic = InternalEditorUtility.tags.ToDictionary(value => value);
        ConstantsClassCreator.Create ("DefineTagName", "AutoOutputFromEditor : タグ名を定数で管理するクラス", tagDic);
        
        //レイヤーとレイヤーマスク
        Dictionary<string, int> layerNoDic     = InternalEditorUtility.layers.ToDictionary(layer => layer, layer => LayerMask.NameToLayer (layer));
        Dictionary<string, int> layerMaskNoDic = InternalEditorUtility.layers.ToDictionary(layer => layer, layer => 1 <<  LayerMask.NameToLayer (layer));
        
        ConstantsClassCreator.Create ("DefineLayerNo",     "AutoOutputFromEditor : レイヤー番号を定数で管理するクラス",      layerNoDic);
        ConstantsClassCreator.Create ("DefineLayerMaskNo", "AutoOutputFromEditor : レイヤーマスク番号を定数で管理するクラス", layerMaskNoDic);        
        
        //ソーティングレイヤー
        Dictionary<string, string> sortingLayerDic = GetSortingLayerNames ().ToDictionary(value => value);
        ConstantsClassCreator.Create ("DefineSortingLayerName", "AutoOutputFromEditor : ソーティングレイヤー名を定数で管理するクラス", sortingLayerDic);
        
        // PlayerSettingsを抽出してクラス化.
        Dictionary<string, string> playerSettingsDic = new Dictionary<string, string>(){
            {"ProductName"  , PlayerSettings.productName},
            {"BundleVersion", PlayerSettings.bundleVersion},
        };
        ConstantsClassCreator.Create ("DefinePlayerSettings", "AutoOutputFromEditor : PlayerSettingsの設定を定数で管理するクラス", playerSettingsDic);
    }
    
    //sortinglayerの名前一覧を取得
    private static string[] GetSortingLayerNames() {
        
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        
    }
    
    
}