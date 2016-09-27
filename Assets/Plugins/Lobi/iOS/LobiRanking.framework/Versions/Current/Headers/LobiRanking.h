//
//  LobiRanking.h
//  LobiSDK
//
//  Created by takahashi-kohei on 2014/03/16.
//  Copyright (c) 2014年 KAMEDAkyosuke. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "LobiAPI+Ranking.h"

@interface LobiRanking : NSObject

+ (instancetype)sharedInstance;
+ (NSString*)SDKVersion;

/**
 *  ランキング一覧を表示します。
 */
+ (void)presentRanking __attribute__ ((deprecated));

/**
 *  ランキング一覧を表示します。
 *
 *  @param prepareHandler 表示完了後に行う処理を指定します。バックグラウンドからの復帰時にも呼び出されます
 *  @param afterHandler   dissmiss完了後に行う処理を指定します
 */
+ (void)presentRanking:(void(^)(void))prepareHandler
          afterHandler:(void(^)(void))afterHandler;
@end
