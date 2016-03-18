//*******************************************************//
// Copyright © 2014-2016 ZXTeam http://www.zxteam.org    //
// License: http://opensource.org/licenses/MIT           //
//*******************************************************//
// Author: Maxim Anurin <maxim.anurin@zxteam.net>        //
// Web: http://www.anurin.name                           //
// SkypeId: maxim.anurin                                 //
//*******************************************************//

#ifndef __ORG_ZXTEAM_ZXASSIST_HOST__APPLICATION__
#define __ORG_ZXTEAM_ZXASSIST_HOST__APPLICATION__

#include "Macros.hpp"
#include "../contract/FeatureHost.hpp"
#include "org/zxteam/lib/reusable/app/DesktopApplication.hpp"
#include "org/zxteam/lib/reusable/runtime/DynamicLibrary.hpp"
#include <vector>

NAMESPACE__ORG_ZXTEAM_ZXASSIST_HOST__BEGIN

class ZXAssistApplication : public virtual org::zxteam::lib::reusable::app::DesktopApplication, public virtual org::zxteam::zxassist::FeatureHost
{
public:
	ZXAssistApplication(HMODULE moduleHandle) : org::zxteam::lib::reusable::app::DesktopApplication(moduleHandle) { }
	virtual int Run(void);

	// FeatureHost
	virtual void SayHello(const char* message) { MessageBoxA(NULL, message,0,MB_OK); }

private:
	void InitFeatures(void);
	void ReleaseFeatures(void);

	typedef std::vector<org::zxteam::lib::reusable::runtime::DynamicLibraryPtr> FeaturesVector;
	FeaturesVector _features;
};

DECLARE_PTR__BEGIN(ZXAssistApplication)
DECLARE_PTR__END

NAMESPACE__ORG_ZXTEAM_ZXASSIST_HOST__END

#endif // __ORG_ZXTEAM_ZXASSIST_HOST__APPLICATION__
