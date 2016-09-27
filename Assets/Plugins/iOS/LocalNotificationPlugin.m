#import <Foundation/Foundation.h>


void LocalNotificationPlugin_ClearBadge()
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0 ;
}

