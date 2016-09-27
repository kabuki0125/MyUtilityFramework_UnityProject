#define make_autorelease_string(c_str) [[[NSString alloc] initWithBytes:(c_str) length:(c_str ## _len) encoding:NSUTF8StringEncoding] autorelease]
