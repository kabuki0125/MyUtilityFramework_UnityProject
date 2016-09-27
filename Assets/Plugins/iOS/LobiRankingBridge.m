#import <UIKit/UIKit.h>

#import <LobiCore/LobiCore.h>
#import <LobiRanking/LobiRanking.h>

#import "LobiCoreBridge.h"

void LobiRanking_present_ranking_()
{
    [LobiCore setRootViewController:LobiCore_get_root_view_controller()];
    [LobiRanking presentRanking:^{ LobiCore_prepare_handler(); }
                   afterHandler:^{ LobiCore_after_handler(); }];
}