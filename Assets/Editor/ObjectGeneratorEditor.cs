using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// エディタ拡張：表示物が増えるとやってられないのでObjectGeneretorを使いやすくする.
/// </summary>
[CustomEditor(typeof(ObjectGenerator))]
[CanEditMultipleObjects]
public class ObjectGeneratorEditor : Editor
{
    

	public override void OnInspectorGUI()
    {
        var generator = target as ObjectGenerator;  
        
        // 検索用文字列入力.文字が入っていた場合表示が切り替わる.
        EditorGUILayout.LabelField("検索文字列");
        m_searchText = EditorGUILayout.TextArea(m_searchText);
        
        // 表示リストに隙間を作らない.
        if( GUILayout.Button("隙間削除") ){
            var list = new List<UnityEngine.Object>(generator.prefabs);
            list.RemoveAll(o => o == null);
            generator.prefabs = list.ToArray();
        }
        
        // アルファベット順に並び替え.
        if( GUILayout.Button("A-Z順並び替え") ){
            m_searchText = "";
            Array.Sort(generator.prefabs, (x, y) => x.name.CompareTo(y.name));
        }
        
        if(generator.prefabs.Length >= 20){
            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Height(420));
        }
        
        // 検索文字列に応じて表示リストをフィルタリング.
        if(string.IsNullOrEmpty(m_searchText)){
            DrawDefaultInspector();
        }else{
            for(var i = 0; i < generator.prefabs.Length ; i++){
                if(generator.prefabs[i].name.Contains(m_searchText)){
                    generator.prefabs[i] = EditorGUILayout.ObjectField(generator.prefabs[i], typeof(UnityEngine.Object), true);
                }
            }
        }
        
        if(generator.prefabs.Length >= 20){
            EditorGUILayout.EndScrollView();
        }
    }
    
    private Vector2 m_scrollPos;
    private string m_searchText; // 検索文字列
}
