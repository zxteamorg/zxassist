//*******************************************************//
// Copyright © 2014-2016 ZXTeam http://www.zxteam.org    //
// License: http://opensource.org/licenses/MIT           //
//*******************************************************//
// Author: Maxim Anurin <maxim.anurin@zxteam.net>        //
// Web: http://www.anurin.name                           //
// SkypeId: maxim.anurin                                 //
//*******************************************************//

#ifndef __ORG_ZXTEAM_ZXASSIST_CONTRACT__FEATURE__
#define __ORG_ZXTEAM_ZXASSIST_CONTRACT__FEATURE__

#include "FeatureHost.hpp"

extern "C"
{
	__declspec(dllexport) void InitFeature(org::zxteam::zxassist::FeatureHost&);
	__declspec(dllexport) void ReleaseFeature(org::zxteam::zxassist::FeatureHost&);
}

#endif // __ORG_ZXTEAM_ZXASSIST_CONTRACT__FEATURE__
