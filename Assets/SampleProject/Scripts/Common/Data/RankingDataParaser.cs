using UnityEngine;


namespace JsonDataParser 
{

    /// <summary>
    /// ランキングデータがJsonで帰ってくるのでパースする用のクラス.
    /// パラメータは固定なので自動出力にもせず手動で定義する.
    /// </summary>
    public partial class RankingDataParaser
    {
        public string status_code;
        public Result result;
        public string id;
        
        public partial class Result 
        {
            public Ranking ranking;
            public string cursor;
            public string total_results;
            public Orders[] orders;
            public string limit;
            public SelfOrder self_order;
        }
        
        public partial class Ranking 
        {
            public string icon;
            public string name;
            public string join_count;
            public string id;
        }
        
        public partial class Orders 
        {
            public string icon;
            public string uid;
            public string name;
            public string is_self;
            public string score;
            public string display_score;
            public string rank;
        }
        
        public partial class SelfOrder
        {
            public string icon;
            public string uid;
            public string name;
            public string score;
            public string display_score;
            public string rank;
        }
    }
}