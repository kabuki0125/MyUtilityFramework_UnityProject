using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// クラス：動的メッシュ生成クラス
/// </summary>
public static class MeshCreater
{
    /// <summary>
    /// 頂点座標から多角形メッシュ生成.
    /// </summary>
    /// <param name="vetices">多角形頂点リスト.</param>
    public static Mesh Create(params Vector3[] vertices)
    {
        if(vertices.Length < 3){
            Debug.LogError("[MeshCreater] Create Error!! : vetrices length then 3 lower.");
            return null;
        }
        m_points = vertices;
        
        var mesh = new Mesh();
        
        mesh.Clear();
        mesh.name = "from_MeshCreater_mesh";
        mesh.vertices = m_points;
        mesh.triangles = GetTriangulesFromVetices();
        
        return mesh;
    }
    
    /// <summary>
    /// 連続する頂点(時計回り・反時計回りは設定済み)から三角形の構成を取得する.
    /// </summary>
    /// <returns>三角形の構成を並べた各頂点座標Vector3のリスト.</returns>
    /// <param name="vetices">時計回り・反時計回りは設定済みの多角形頂点リスト.</param>
    private static int[] GetTriangulesFromVetices()
    {
        List<int> indices = new List<int> ();
        var ptNum = m_points.Length;
        
        // 頂点の並びが時計回り・反時計回りのどちらか.
        var vertIndexList = new int[ptNum];
        if(Area() > 0){
            for(var i = 0; i < ptNum; i++){
                vertIndexList[i] = i;
            }
        }else{
            for(var i = 0; i < ptNum; i++)
                vertIndexList[i] = (ptNum - 1) - i;
        }
        
        // 頂点を結び頂点番号リストを作成.
        var remain = ptNum;
        var count = 2 * remain;
        for(var i2 = remain - 1; remain > 2;){
            if((count--) <= 0){
                break;
            }
            
            var i1 = i2;
            if(i1 >= remain){
                i1 = 0;
            }
            i2 = i1 + 1;
            if(i2 >= remain){
                i2 = 0;
            }
            var i3 = i2 + 1;
            if(i3 >= remain){
                i3 = 0;
            }
            
            if(Snip(i1, i2, i3, remain, vertIndexList)){
                var a = vertIndexList[i1];
                var b = vertIndexList[i2];
                var c = vertIndexList[i3];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for(int s = i2, t = i2 + 1; t < remain; s++, t++){
                    vertIndexList[s] = vertIndexList[t];
                }
                remain--;
                count = 2 * remain;
            }
        }
        
        var pos0 = m_points[indices[0]];
        var pos1 = m_points[indices[1]];
        var pos2 = m_points[indices[2]];
        var vec1 = pos1 - pos0;
        var vec2 = pos2 - pos1;
        var crossVec = Vector3.Cross (vec1, vec2);
        var camPos = CameraHelper.SharedInstance.MainCamera.transform.position;
        if(Vector3.Dot(camPos, crossVec) <= 0){
            indices.Reverse();
        }
        
        return indices.ToArray ();
    }
    private static float Area()
    {
        var n = m_points.Length;
        var A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            var pval = m_points[p];
            var qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }
    private static bool Snip(int i1, int i2, int i3, int ptNum, int[] idxList)
    {
        var ptA = m_points[idxList[i1]];
        var ptB = m_points[idxList[i2]];
        var ptC = m_points[idxList[i3]];
        if(Mathf.Epsilon > (((ptB.x - ptA.x) * (ptC.y - ptA.y)) - ((ptB.y - ptA.y) * (ptC.x - ptA.x)))){
            return false;
        }
        for(var p = 0; p < ptNum; p++){
            if ((p == i1) || (p == i2) || (p == i3)){
                continue;
            }
            var P = m_points[idxList[p]];
            if(InsideTriangle(ptA, ptB, ptC, P)){
                return false;
            }
        }
        return true;
    }
    // 点が三角形の内側にいるかの判定.内側にいる場合はtrueが返る.
    private static bool InsideTriangle (Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;        
        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;        
        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;        
        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
    
    private static Vector3[] m_points;
}
