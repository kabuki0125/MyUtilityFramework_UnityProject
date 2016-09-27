
typedef enum {
    LobiHTTPMethodGET = 0,
    LobiHTTPMethodPOST
} LobiHTTPMethod;

typedef enum {
    LobiPermissionTypeAnyUser = 0,
    LobiPermissionTypeLeader,
    LobiPermissionTypeSuperUser,
} LobiPermissionType;

typedef NS_ENUM (NSInteger, LobiPopOverArrowDirection) {
    LobiPopOverArrowDirectionUp,
    LobiPopOverArrowDirectionLeft,
    LobiPopOverArrowDirectionRight,
};
