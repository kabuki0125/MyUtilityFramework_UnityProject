#import <UIKit/UIKit.h>

typedef enum {
    LobiAPIStatusCodeSuccess       = 0,
    LobiAPIStatusCodeNetworkError  = 100,
    LobiAPIStatusCodeResponseError = 101,
    LobiAPIStatusCodeFatalError    = 102
} LobiAPIStatusCode;

@class LobiNetworkResponse;

LobiAPIStatusCode LobiAPICommon_getStatusCode(NSError *error);
NSString* LobiAPICommon_error_message(LobiAPIStatusCode status_code);
NSString* LobiAPICommon_error_message_with_ex(LobiAPIStatusCode status_code, id response);
NSString* LobiAPICommon_default_success_message(LobiAPIStatusCode status_code);
NSString* LobiAPICommon_success_message(LobiAPIStatusCode status_code, id result);

void LobiAPICommon_success_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName);
void LobiAPICommon_error_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName);
void LobiAPICommon_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName, LobiNetworkResponse *res);
void LobiAPICommon_message_callback_to_unity(NSString *gameObjectName, NSString *callbackMethodName, NSString *message);
