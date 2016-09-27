//
//  LobiCore.h
//  LobiCore
//
//  Created by takahashi-kohei on 2014/03/11.
//  Copyright (c) 2014年 面白法人カヤック. All rights reserved.

#ifndef _LobiCore_h
#define _LobiCore_h

#import <Foundation/Foundation.h>
#import "LobiConst.h"

/**
 Lobiのコア機能（アカウント関連の操作と基本画面）を扱うクラスです。
 */
@interface LobiCore : NSObject

/**
 *  Lobiのコア機能（アカウント関連の操作と基本画面）を扱うLobiCoreクラスのシングルトンインスタンスを提供します。
 *
 *  @return LobiCoreクラスのシングルトンインスタンスを返します。
 */
+ (instancetype)sharedInstance;

/**
 *  LobiCoreSDKのバージョンを返します。
 *
 *  @return バージョン番号
 */
+ (NSString*)lobiCoreVersion;

/**
 *  LobiCoreSDKを導入しているアプリのバージョンを返します。
 *
 *  @return バージョン番号
 */
+ (NSString*)applicationVersion;

/**
 *  LobiCoreSDKのbundleを返します。
 *
 *  @return LobiCoreのBundle
 */
+ (NSBundle*)bundle;

/**
 *  Lobiのアカウントが既に存在しているかを返します。
 *  @deprecated `+ (BOOL)isSignedIn` を使用してください。
 *  @return Lobiのアカウントの有無
 */
+ (BOOL)isReady  __attribute__ ((deprecated("use isSignedIn")));

/**
 *  Lobiのアカウントが既に存在しているかを返します。
 *
 *  @return Lobiのアカウントの有無
 */
+ (BOOL)isSignedIn;

/**
 *  Lobiの機能を使い始めるにあたり、必要となる情報を設定します。
 *
 *  @param clientId        セットアップ時に設定するアプリ毎に固有のID（developerページにて取得）
 *  @param accountBaseName　セットアップ時に設定するLobiアカウントのユーザー名
 */
+ (void)setupClientId:(NSString*)clientId accountBaseName:(NSString*)accountBaseName;

/**
 *  広告に関する設定を行います。
 *
 *  @param adPlatform
 *  @param adPlatformId
 */
+ (void)setupAdPlatform:(NSString*)adPlatform adPlatformId:(NSString*)adPlatformId __attribute__ ((deprecated));

/**
 *  LobiSDKの基本画面を表示するViewのViewControllerを設定します。
 *
 *  @param rootViewController LobiSDKの基本画面を表示するViewのViewController
 */
+ (void)setRootViewController:(UIViewController *)rootViewController;


+ (BOOL)handleOpenURL:(NSURL *)url;

/**
 *  自身のLobiアカウントのプロフィール画面を表示します。
 */
+ (void)presentProfile __attribute__ ((deprecated));


/**
 *  自身のLobiアカウントのプロフィール画面を表示します。
 *
 *  @param prepareHandler 表示完了後に行う処理を指定します。バックグラウンドからの復帰時にも呼び出されます
 *  @param afterHandler   dissmiss完了後に行う処理を指定します
 */
+ (void)presentProfile:(void(^)(void))prepareHandler
          afterHandler:(void(^)(void))afterHandler;

+ (void)prepareExternalId:(NSString*)externalId
         initializeVector:(NSString*)iv
          accountBaseName:(NSString*)accountBaseName;

+ (BOOL)hasExidUser;
+ (void)bindToLobiAccount;

/**
 *  iPadで表示する際に表示の開始位置と吹き出しの出る方向を設定する
 *
 *  @param origin    表示の開始位置
 *  @param direction 吹き出しの出る方向
 */
+ (void)setupPopOverController:(CGPoint)origin direction:(LobiPopOverArrowDirection)direction;

/**
 *  ナビゲーションバーの色を変更します。
 *
 *  @param color 指定する色
 */
+ (void)setNavigationBarCustomColor:(UIColor *)color;

/**
 *  Lobiで表示されているViewControllerを全て閉じる
 *
 *  @param animated   アニメーションするかどうか
 */
+ (void)dismissViewControllerAnimated:(BOOL)animated;

@property (nonatomic, readonly) NSString *clientId;
@property (nonatomic, copy)     NSString *accountBaseName;
@property (nonatomic, assign)   BOOL useStrictExId;
@property (nonatomic, copy)     void(^afterHandler)(void) __attribute__ ((deprecated));

@property (nonatomic, readonly) NSString *adPlatform   __attribute__ ((deprecated));
@property (nonatomic, readonly) NSString *adPlatformId __attribute__ ((deprecated));

@end

#endif
