using System;
using System.Collections.Generic;

namespace Extensions
{
	/// <summary>
	/// 拡張クラス：リスト
	/// </summary>
	public static class ListEx
	{
		/// <summary>
		/// 空？
		/// </summary>
		public static bool IsEmpty(this List<Type> list)
		{
			return list.Count <= 0;
		}

		/// <summary>
		/// 最後の要素.
		/// </summary>
		public static Type Last(this List<Type> list)
		{
			if(list.Count <= 0){
				return default(Type);
			}
			return list[list.Count -1];
		}

#region Queue Behaviour
		/// <summary>
		/// キュー：配列の最後に要素をひとつ入れる。
		/// </summary>
		public static void Enqueue<Type>(this List<Type> list,Type obj)
		{
			list.Add(obj);
		}		
		/// <summary>
		/// キュー：配列の先頭から要素を１つ取り出す.
		/// </summary>
		public static Type Dequeue<Type>(this List<Type> list)
		{
			if( list.Count <= 0 ){
				return default(Type);
			}
			
			var obj = list[0];
			list.RemoveAt(0);
			return obj;
        }
#endregion

#region Stack Behaviour
		/// <summary>
		/// スタック；配列の最後に要素を１つ入れる
		/// </summary>
		public static void Push<Type>(this List<Type> list,Type obj)
		{
			list.Add(obj);
		}
		/// <summary>
		/// スタック；配列の最後から要素を１つ取り出す
		/// </summary>
		public static Type Pop<Type>(this List<Type> list)
		{
			if( list.Count <= 0 ){
				return default(Type);
			}
			
			var obj = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return obj;
        }
#endregion
    }
}
