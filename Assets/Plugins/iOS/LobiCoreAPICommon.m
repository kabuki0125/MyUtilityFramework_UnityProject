#import <UIKit/UIKit.h>

#import "LobiCoreAPICommon.h"
#import <LobiCore/LobiNetworkResponse.h>

LobiAPIStatusCode LobiAPICommon_getStatusCode(NSError *error){
    if(error == nil){
        return LobiAPIStatusCodeSuccess;
    }
    else if(400 <= error.code && error.code < 500){
        return LobiAPIStatusCodeFatalError;
    }
    else if(500 <= error.code && error.code < 600){
        return LobiAPIStatusCodeResponseError;
    }
    else{
        return LobiAPIStatusCodeNetworkError;
    }
}

NSString* LobiAPICommon_error_message(LobiAPIStatusCode status_code)
{
    NSDictionary *params = @{
                             @"status_code" : [NSString stringWithFormat:@"%d", status_code],
                             @"error" : @"1",
                             };
    if([NSJSONSerialization isValidJSONObject:params]){
        NSError *error = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:params
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
        if(error == nil){
            return [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
        }
    }
    return @"{\"status_code\" : \"102\", \"error\" : \"1\"}";
}

NSString* LobiAPICommon_error_message_with_ex(LobiAPIStatusCode status_code, id response)
{
    NSString *errorMessage = @"";
    if(response != nil && [response isKindOfClass:[NSDictionary class]]){
        NSArray* ary = [response objectForKey:@"error"];
        if(ary != nil){
            errorMessage = [ary componentsJoinedByString:@"\n"];
        }
    }
    
    NSDictionary *params = @{
                             @"status_code" : [NSString stringWithFormat:@"%d", status_code],
                             @"error"       : errorMessage,
                             };
    if([NSJSONSerialization isValidJSONObject:params]){
        NSError *error = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:params
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
        if(error == nil){
            return [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
        }
    }
    return LobiAPICommon_error_message(status_code);
}

NSString* LobiAPICommon_default_success_message(LobiAPIStatusCode status_code)
{
    NSDictionary *params = @{
                             @"status_code" : [NSString stringWithFormat:@"%d", status_code],
                             @"result"      : @{@"success" : @"1"},
                             };
    if([NSJSONSerialization isValidJSONObject:params]){
        NSError *error = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:params
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
        if(error == nil){
            return [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
        }
    }
    return LobiAPICommon_error_message(status_code);
}

NSString* LobiAPICommon_success_message(LobiAPIStatusCode status_code, id result)
{
    NSDictionary *params = @{
                             @"status_code" : [NSString stringWithFormat:@"%d", status_code],
                             @"result"      : result,
                             };
    if([NSJSONSerialization isValidJSONObject:params]){
        NSError *error = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:params
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
        if(error == nil){
            return [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
        }
    }
    return LobiAPICommon_error_message(status_code);
}

static void callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName, NSString *message)
{
    UnitySendMessage([gameObjectName cStringUsingEncoding:NSUTF8StringEncoding],
                     [callbackMethodName cStringUsingEncoding:NSUTF8StringEncoding],
                     [message cStringUsingEncoding:NSUTF8StringEncoding]);
}

void LobiAPICommon_success_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName)
{
    callback_to_unity(gameObjectName, callbackMethodName, LobiAPICommon_default_success_message(LobiAPIStatusCodeSuccess));
}

void LobiAPICommon_error_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName)
{
    callback_to_unity(gameObjectName, callbackMethodName, LobiAPICommon_error_message(LobiAPIStatusCodeFatalError));
}

void LobiAPICommon_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName, LobiNetworkResponse *res)
{
    NSString *message;
    if(res == nil){
        message = LobiAPICommon_error_message(LobiAPIStatusCodeFatalError);
    }
    else if(res.error){
        LobiAPIStatusCode statusCode = LobiAPICommon_getStatusCode(res.error);
        message = LobiAPICommon_error_message_with_ex(statusCode, res.error.userInfo[@"responsedata"]);
    }
    else{
        message = LobiAPICommon_success_message(LobiAPIStatusCodeSuccess,
                                                res.dictionary != nil ? res.dictionary : res.array);
    }
    callback_to_unity(gameObjectName, callbackMethodName, message);
}

void LobiAPICommon_message_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName, NSString *message)
{
    callback_to_unity(gameObjectName, callbackMethodName, message);
}
