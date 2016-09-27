#import <UIKit/UIKit.h>
#import <LobiCore/LobiAPI.h>
#import <LobiCore/LobiNetworkResponse.h>

typedef NS_ENUM (NSUInteger, KLRRankingRange)
{
    KLRRankingRangeToday = 0,
    KLRRankingRangeWeek,
    KLRRankingRangeAll,
    KLRRankingRangeLastWeek,
};

typedef NS_ENUM (NSUInteger, KLRRankingCursorOrigin)
{
    KLRRankingCursorOriginTop = 0,
    KLRRankingCursorOriginSelf,
};

@interface LobiAPI (Ranking)

/// ---------------------------------
/// @name Ranking API
/// ---------------------------------

/**
 * ランキングのスコアを送信します。
 *
 * @param rankingID 開発者向けページで作成したアプリのランキングIDを指定します。
 * @param score 整数形式で得点を指定します。
 */
+ (void)sendRanking:(NSString *)rankingID
              score:(int64_t)score
            handler:(LobiNetworkHandler)handler;

/**
 * ランキング詳細を取得します。
 * responseのself_orderには自分のランキングデータが設定されます。
 * もしユーザがランキングにスコアが投稿してない場合、self_orderの値は空になります。
 *
 * @param rankingID 開発者向けページで作成したアプリのランキングIDを指定します。
 * @param type ランキングの範囲をKLRRankingRangeToday（本日）,
 * KLRRankingRangeWeek（今週）,
 * KLRRankingRangeAll（全体）,
 * KLRRankingRangeLastWeek（先週）から指定します。
 * @param origin 順位の取得の基準をKLRRankingCursorOriginTop（先頭）,
 * KLRRankingCursorOriginSelf（自分中心）から指定します。
 * @param cursor KLRRankingCursorOriginTopを選択時に何位から取得するか指定します。
 * default は 1。self の場合は無視します。
 * @param limit KLRRankingCursorOriginTopを選択時、何人取得するか指定します。
 * KLRRankingCursorOriginSelfを選択時、前後何人取得するか指定します。
 */
+ (void)getRanking:(NSString *)rankingID
              type:(KLRRankingRange)type
            origin:(KLRRankingCursorOrigin)origin
            cursor:(NSInteger)cursor
             limit:(NSInteger)limit
           handler:(LobiNetworkHandler)handler;

/**
 * 順位表(LeaderBoard)毎のランキングを取得します。
 *
 * @param type ランキングの範囲をKLRRankingRangeToday（本日）,
 * KLRRankingRangeWeek（今週）,
 * KLRRankingRangeAll（全体）,
 * KLRRankingRangeLastWeek（先週）から指定します。
 */
+ (void)getRankingList:(KLRRankingRange)type
               handler:(LobiNetworkHandler)handler;


/**
 * ユーザの順位表(LeaderBoard)毎のランキングを取得します。
 * responseのorderには自分のランキングデータが設定されます。
 * もしユーザがランキングにスコアが投稿してない場合、orderの値は空になります。
 *
 * @param type ランキングの範囲をKLRRankingRangeToday（本日）,
 * KLRRankingRangeWeek（今週）,
 * KLRRankingRangeAll（全体）,
 * KLRRankingRangeLastWeek（先週）から指定します。
 * @param uid 取得するユーザのIDを指定します。
 */
+ (void)getRankingList:(KLRRankingRange)type
                  user:(NSString*)uid
               handler:(LobiNetworkHandler)handler;


+ (void)sendAppData:(NSString *)data
            handler:(LobiNetworkHandler)handler;

+ (void)getAppData:(NSArray *)fields
           handler:(LobiNetworkHandler)handler;

+ (void)removeAppData:(NSArray *)fields
              handler:(LobiNetworkHandler)handler;

+ (void)sendAppDataGlobal:(NSString *)data
                  handler:(LobiNetworkHandler)handler;

+ (void)getAppDataGlobal:(NSArray *)fields
                 handler:(LobiNetworkHandler)handler;

+ (void)removeAppDataGlobal:(NSArray *)fields
                    handler:(LobiNetworkHandler)handler;

@end
