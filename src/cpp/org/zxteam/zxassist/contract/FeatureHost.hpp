//*******************************************************//
// Copyright © 2014-2016 ZXTeam http://www.zxteam.org    //
// License: http://opensource.org/licenses/MIT           //
//*******************************************************//
// Author: Maxim Anurin <maxim.anurin@zxteam.net>        //
// Web: http://www.anurin.name                           //
// SkypeId: maxim.anurin                                 //
//*******************************************************//

#ifndef __ORG_ZXTEAM_ZXASSIST_CONTRACT__FEATURE_HOST__
#define __ORG_ZXTEAM_ZXASSIST_CONTRACT__FEATURE_HOST__

#include "Macros.hpp"

NAMESPACE__ORG_ZXTEAM_ZXASSIST__BEGIN

class FeatureHost
{
public:
	virtual void SayHello(const char*) = 0;
};

NAMESPACE__ORG_ZXTEAM_ZXASSIST__END

#endif // __ORG_ZXTEAM_ZXASSIST_CONTRACT__FEATURE_HOST__
