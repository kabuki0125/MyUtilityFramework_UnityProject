#import <UIKit/UIKit.h>

#import <LobiCore/LobiCore.h>
#import <LobiRanking/LobiAPI+Ranking.h>
#import <LobiCore/LobiNetworkResponse.h>

#import "LobiCoreCommon.h"
#import "LobiCoreAPICommon.h"

/*
+ (void)sendRanking:(NSString *)rankingID
              score:(int64_t)score
            handler:(LobiNetworkHandler)handler;
 */
void LobiRanking_send_ranking_(const char *game_object_name, int game_object_name_len,
                               const char *callback_method_name, int callback_method_name_len,
                               const char *ranking_id, int ranking_id_len,
                               int64_t score)
{
    NSString *gameObjectName     = make_autorelease_string(game_object_name);
    NSString *callbackMethodName = make_autorelease_string(callback_method_name);
    NSString *rankingId          = make_autorelease_string(ranking_id);
    [LobiAPI sendRanking:rankingId
                   score:score
                 handler:^(LobiNetworkResponse *res)
    {
        LobiAPICommon_callback_to_unity(gameObjectName, callbackMethodName, res);
    }];
}

/*
+ (void)getRanking:(NSString *)rankingID
              type:(KLRRankingRange)type
            origin:(KLRRankingCursorOrigin)origin
            cursor:(NSInteger)cursor
             limit:(NSInteger)limit
           handler:(LobiNetworkHandler)handler;
 */

void LobiRanking_get_ranking_(const char *game_object_name, int game_object_name_len,
                              const char *callback_method_name, int callback_method_name_len,
                              const char *ranking_id, int ranking_id_len,
                              int type,
                              int origin,
                              int cursor,
                              int limit)
{
    NSString *gameObjectName     = make_autorelease_string(game_object_name);
    NSString *callbackMethodName = make_autorelease_string(callback_method_name);
    NSString *rankingId          = make_autorelease_string(ranking_id);
    [LobiAPI getRanking:rankingId
                   type:type
                 origin:origin
                 cursor:cursor
                  limit:limit
                handler:^(LobiNetworkResponse *res)
    {
        LobiAPICommon_callback_to_unity(gameObjectName, callbackMethodName, res);
    }];
}

/*
+ (void)getRankingList:(KLRRankingRange)type
               handler:(LobiNetworkHandler)handler;
 */
void LobiRanking_get_ranking_list_(const char *game_object_name, int game_object_name_len,
                                   const char *callback_method_name, int callback_method_name_len,
                                   int type)
{
    NSString *gameObjectName     = make_autorelease_string(game_object_name);
    NSString *callbackMethodName = make_autorelease_string(callback_method_name);
    [LobiAPI getRankingList:type
                    handler:^(LobiNetworkResponse *res)
    {
        LobiAPICommon_callback_to_unity(gameObjectName, callbackMethodName, res);
    }];
}

/*
 + (void)getRankingList:(KLRRankingRange)type
                   user:(NSString*)uid
                handler:(LobiNetworkHandler)handler;
 */
void LobiRanking_get_user_ranking_list_(const char *game_object_name, int game_object_name_len,
                                        const char *callback_method_name, int callback_method_name_len,
                                        int type,
                                        const char *uid, int uid_len)
{
    NSString *gameObjectName     = make_autorelease_string(game_object_name);
    NSString *callbackMethodName = make_autorelease_string(callback_method_name);
    NSString *u                  = make_autorelease_string(uid);
    [LobiAPI getRankingList:type
                       user:u
                    handler:^(LobiNetworkResponse *res)
    {
        LobiAPICommon_callback_to_unity(gameObjectName, callbackMethodName, res);
    }];
}
