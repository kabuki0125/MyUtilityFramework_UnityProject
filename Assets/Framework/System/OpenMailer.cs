using UnityEngine;
using System.Collections;

/// <summary>
/// メーラーを開く用クラス.
/// </summary>
public class OpenMailer
{
    /// <summary>
    /// メーラーを起動する
    /// </summary>
    public static void Exec(string mailAdress, string title = null)
    {
        if(title == null){
			title = "test";//DefinePlayerSettings.PRODUCT_NAME;
        }
        
        //本文は端末名、OS、アプリバージョン、言語
        var deviceName = SystemInfo.deviceModel;
#if UNITY_IOS && !UNITY_EDITOR
        deviceName = UnityEngine.iOS.Device.generation.ToString();
#endif
        
        var body = "\n\n---------以下の内容はそのままで---------\n\n";
        body += "Device   : " + deviceName                             +"\n";
        body += "OS       : " + SystemInfo.operatingSystem             +"\n";
        body += "Ver      : " + "1.0"/*DefinePlayerSettings.BUNDLE_VERSION*/    +"\n";
        body += "Language : " + Application.systemLanguage.ToString()  +"\n";
        
        //エスケープ処理
        body    = System.Uri.EscapeDataString(body);
        title   = System.Uri.EscapeDataString(title);
        
        Application.OpenURL("mailto:" + mailAdress + "?subject=" + title + "&body=" + body);
    }
    
}
