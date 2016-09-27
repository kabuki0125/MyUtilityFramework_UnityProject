#import <UIKit/UIKit.h>

#ifdef __cplusplus
extern "C" {
#endif
    void LobiCore_set_root_view_controller_func(UIViewController*(*getViewController)());
    UIViewController* LobiCore_get_root_view_controller();
    
#if UNITY_VERSION < 500
    void LobiCore_set_unity_pause_func(void(*unityPause)(bool));
#else
    void LobiCore_set_unity_pause_func(void(*unityPause)(int));
#endif
    void LobiCore_prepare_handler();
    void LobiCore_after_handler();
#ifdef __cplusplus
}
#endif
