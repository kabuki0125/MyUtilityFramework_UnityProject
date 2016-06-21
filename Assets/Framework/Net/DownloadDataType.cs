using UnityEngine;
using System.Collections;

/// <summary>
/// ダウンロードデータ種類
/// </summary>
public enum DownloadDataType
{
	Binary,         // バイナリデータ
	AssetBundle,    // AssetBundle ファイル
	NetCommand,     // 通信コマンド
}

public static class DownloadDataTypeExtension
{
	/// <summary>
	/// 通信優先度を取得する
	/// </summary>
	/// <returns><c>static void</c></returns>
	public static int GetNetPriority(this DownloadDataType type)
	{
		return NET_PRIORITY_LIST[(int)type];
	}
	private static readonly int[] NET_PRIORITY_LIST = {
		30,
		10,
		20,
	};
}
