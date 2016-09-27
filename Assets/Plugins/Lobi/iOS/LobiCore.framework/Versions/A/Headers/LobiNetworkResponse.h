//
//  LobiNetworkResponse.h
//  LobiCore
//
//  Created by takahashi-kohei on 2014/03/24.
//  Copyright (c) 2014年 面白法人カヤック. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LobiNetworkResponse;
typedef void (^LobiNetworkHandler)(LobiNetworkResponse *res);

/**
 LobiAPIのResponseをラップしたクラスです。
 */
@interface LobiNetworkResponse : NSObject

/**
 *  LobiAPIリクエストのレスポンス
 */
@property (nonatomic, retain)   NSURLResponse *response;

/**
 *  LobiAPIリクエスト時のエラー
 */
@property (nonatomic, retain)   NSError       *error;

/**
 *  LobiAPIリクエストのレスポンスBody
 */
@property (nonatomic, retain)   id      body;

/**
 *  LobiAPIリクエストのレスポンスdictionary
 */
@property (nonatomic, readonly) NSDictionary  *dictionary;

/**
 *  LobiAPIリクエストのレスポンスarray
 */
@property (nonatomic, readonly) NSArray       *array;

/**
 *  LobiAPIリクエストのレスポンスdata
 */
@property (nonatomic, readonly) NSData        *data;

@end

