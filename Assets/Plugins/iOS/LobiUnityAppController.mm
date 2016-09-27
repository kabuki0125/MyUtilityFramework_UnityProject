#if UNITY_VERSION < 500
#import "iPhone_target_Prefix.pch"
#else
#import "Prefix.pch"
#endif

#import "UnityAppController.h"
#import "UnityAppController+Rendering.h"

// Lobi用にヘッダをインポートする
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#define LOBI_CHAT
#define LOBI_REC

#if UNITY_VERSION < 500

#import <LobiCore/LobiCore.h>
#include "../Libraries/LobiCoreBridge.h"

#ifdef LOBI_CHAT
#import <LobiChat/LobiChat.h>
#include "../Libraries/LobiChatBridge.h"
#endif

#ifdef LOBI_REC
#import <LobiRec/LobiRec.h>
#include "../Libraries/LobiRecBridge.h"
#endif

void UnityPause(bool pause);

#else

#import <LobiCore/LobiCore.h>
#include "../Libraries/Plugins/iOS/LobiCoreBridge.h"

#ifdef LOBI_CHAT
#import <LobiChat/LobiChat.h>
#include "../Libraries/Plugins/iOS/LobiChatBridge.h"
#endif

#ifdef LOBI_REC
#import <LobiRec/LobiRec.h>
#include "../Libraries/Plugins/iOS/LobiRecBridge.h"
#endif

void UnityPause(int pause);

#endif

#import "LobiCoreBridge.h"

#ifdef LOBI_CHAT
@interface LobiUnityAppController : UnityAppController < LobiChatAppLinkDelegate, LobiChatGameProfileDelegate >
#else
@interface LobiUnityAppController : UnityAppController
#endif

+(void)load;

@end

@implementation LobiUnityAppController

+(void)load
{
    extern const char* AppControllerClassName;
    AppControllerClassName = "LobiUnityAppController";
}

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    BOOL ret = [super application:application didFinishLaunchingWithOptions:launchOptions];
    
    // UnityのViewControllerを取得する
    LobiCore_set_root_view_controller_func(UnityGetGLViewController);
    
    // UnityPause関数を取得する
    LobiCore_set_unity_pause_func(UnityPause);
    
#ifdef LOBI_REC
    // 利用するレンダリングAPIを決定する
    UnityRenderingAPI api = self.renderingAPI;
    // Metalに対応していないUnityの場合は`apiMetal`がないはずなのでOpenGLES「ではないか」で判別
    BOOL isMetal = (api != apiOpenGLES2 && api != apiOpenGLES3);
    if (isMetal) {
        [LobiRec useMetal];
    }
    else {
        [LobiRec useOpenGLES];
    }

    // GLViewを設定する
    LobiRec_set_unity_gl_view(UnityGetGLView);
#endif
    
#ifdef LOBI_CHAT
    // AppLink からデータの取得を行う
    [LobiChat setAppLinkDelegate:self];
    
    // ゲームプロフィールページに遷移する
    [LobiChat setGameProfileDelegate:self];
#endif
    
    // 初期化を行う
    [LobiCore setupClientId:@""
            accountBaseName:@""];
    
#ifdef LOBI_CHAT
    // app link 起動時に - (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation が呼ばれるように
    return YES;
#else
    return ret;
#endif
}

- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
    BOOL ret = [super application:application
                          openURL:url
                sourceApplication:sourceApplication
                       annotation:annotation];
    
    if ([LobiCore handleOpenURL:url]) {
        return YES;
    }
    
    return ret;
}

#ifdef LOBI_CHAT
#pragma mark - LobiChatAppLinkDelegate

- (void)onAppLink:(NSString *)value
{
    LobiChat_set_app_link_listener_message(value);
}

#pragma mark - LobiChatGameProfileDelegate

- (void)onGameProfile:(NSString *)value
{
    LobiChat_call_game_profile_listener(value);
}

#endif

@end
