//*******************************************************//
// Copyright © 2014-2016 ZXTeam http://www.zxteam.org    //
// License: http://opensource.org/licenses/MIT           //
//*******************************************************//
// Author: Maxim Anurin <maxim.anurin@zxteam.net>        //
// Web: http://www.anurin.name                           //
// SkypeId: maxim.anurin                                 //
//*******************************************************//

#ifndef __ORG_ZXTEAM_ZXASSIST_CONTRACT__MACROS__
#define __ORG_ZXTEAM_ZXASSIST_CONTRACT__MACROS__

#ifndef __linux__
#	define ZXASSISTAPI __declspec(dllexport)
#else
#	define ZXASSISTAPI
#endif

/* Namespace helper*/
#define NAMESPACE__ORG_ZXTEAM_ZXASSIST__BEGIN \
namespace org \
{ \
namespace zxteam \
{ \
namespace zxassist \
{ \

#define NAMESPACE__ORG_ZXTEAM_ZXASSIST__END \
} /* namespace zxasssist */ \
} /* namespace zxteam */ \
} /* namespace org */ \

#define NAMESPACE__ORG_ZXTEAM_ZXASSIST_FEATURES__BEGIN NAMESPACE__ORG_ZXTEAM_ZXASSIST__BEGIN \
namespace features \
{ \

#define NAMESPACE__ORG_ZXTEAM_ZXASSIST_FEATURES__END \
} /* namespace features */ \
NAMESPACE__ORG_ZXTEAM_ZXASSIST__END \

#endif // __ORG_ZXTEAM_ZXASSIST_CONTRACT__MACROS__
