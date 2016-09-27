#import <UIKit/UIKit.h>

#import <LobiCore/LobiCore.h>
#import <LobiCore/LobiAPI.h>
#import <LobiCore/LobiNetworkResponse.h>

#import "LobiCoreCommon.h"

#if UNITY_VERSION < 500
static void(*sUnityPause)(bool pause);
#else
static void(*sUnityPause)(int pause);
#endif

#if UNITY_VERSION < 500
void LobiCore_set_unity_pause_func(void(*unityPause)(bool))
{
    sUnityPause = unityPause;
}
#else
void LobiCore_set_unity_pause_func(void(*unityPause)(int))
{
    sUnityPause = unityPause;
}
#endif

extern int lobi_unityVersion();
int lobi_unityVersion()
{
    return UNITY_VERSION;
}

void LobiCore_prepare_handler()
{
    if(sUnityPause != NULL){
        sUnityPause(1);
    }
}

void LobiCore_after_handler()
{
    if(sUnityPause != NULL){
        sUnityPause(0);
    }
    UnitySendMessage("LobiEventReceiver",
                     "Dissmissed",
                     "");
}

static UIViewController*(*sGetUnityViewController)();

void LobiCore_set_root_view_controller_func(UIViewController*(*getViewController)())
{
    sGetUnityViewController = getViewController;
}

UIViewController* LobiCore_get_root_view_controller()
{
    return sGetUnityViewController();
}

int LobiCore_is_signed_in_()
{
    return [LobiCore isReady] ?  1 : 0;
}

void LobiCore_set_account_base_name_(const char *base_name, int base_name_len)
{
    NSString *baseName = make_autorelease_string(base_name);
    [LobiCore sharedInstance].accountBaseName = baseName;
}

void LobiCore_present_profile_()
{
    [LobiCore setRootViewController:sGetUnityViewController()];
    [LobiCore presentProfile:^{
        LobiCore_prepare_handler();
    } afterHandler:^{
        LobiCore_after_handler();
    }];
}

void LobiCore_present_ad_wall_()
{
    [LobiCore setRootViewController:sGetUnityViewController()];
    [LobiCore presentAdWall];
}

void LobiCore_setup_pop_over_controller_(int x, int y, LobiPopOverArrowDirection direction)
{
    [LobiCore setupPopOverController:CGPointMake(x, y) direction:direction];
}

void LobiCore_set_navigation_bar_custom_color_(float r, float g, float b)
{
    UIColor *color = [UIColor colorWithRed:r green:g blue:b alpha:1.0];
    [LobiCore setNavigationBarCustomColor:color];
}

/*
 + (void)prepareExternalId:(NSString*)externalId
 initializeVector:(NSString*)iv
 accountBaseName:(NSString*)accountBaseName
 strictExId:(BOOL)strictExId;
 */
void LobiCore_prepare_external_id_initialize_vector_account_base_name_(
                                                                       const char *encrypted_external_id, int encrypted_external_id_len,
                                                                       const char *encrypt_iv, int encrypt_iv_len,
                                                                       const char *base_name, int base_name_len)
{
    NSString *encryptedExternalId = make_autorelease_string(encrypted_external_id);
    NSString *encryptIv           = make_autorelease_string(encrypt_iv);
    NSString *baseName            = make_autorelease_string(base_name);
    
    [LobiCore prepareExternalId:encryptedExternalId
               initializeVector:encryptIv
                accountBaseName:baseName];
}

void LobiCore_set_use_strict_ex_id_(int enable)
{
    [LobiCore sharedInstance].useStrictExId = (enable == 1);
}

int LobiCore_get_use_strict_ex_id_()
{
    return [LobiCore sharedInstance].useStrictExId ? 1 : 0;
}

int LobiCore_has_exid_user_()
{
    return [LobiCore hasExidUser] ?  1 : 0;
}

void LobiCore_bind_to_lobi_account_()
{
    [LobiCore setRootViewController:sGetUnityViewController()];
    [LobiCore bindToLobiAccount];
}
