//*******************************************************//
// Copyright © 2014-2016 ZXTeam http://www.zxteam.org    //
// License: http://opensource.org/licenses/MIT           //
//*******************************************************//
// Author: Maxim Anurin <maxim.anurin@zxteam.net>        //
// Web: http://www.anurin.name                           //
// SkypeId: maxim.anurin                                 //
//*******************************************************//

#include "org/zxteam/lib/reusable/Object.hpp"
#include "org/zxteam/lib/reusable/app/DesktopApplication.hpp"

#include "../contract/FeatureHost.hpp"
#include "ZXAssistApplication.hpp"

#include <iostream>
#include "boost/intrusive_ptr.hpp"
#include "org/zxteam/lib/reusable/String.hpp"
#include "org/zxteam/lib/reusable/runtime/DynamicLibrary.hpp"
#include "org/zxteam/lib/reusable/fs/FileSystem.hpp"
#include "Windows.h"

using namespace org::zxteam::lib::reusable;
using namespace org::zxteam::lib::reusable::app;
using namespace org::zxteam::lib::reusable::runtime;

using namespace org::zxteam::zxassist::host;

int main(int argc, wchar_t** argv)
{
	ZXAssistApplicationPtr app = new ZXAssistApplication(GetModuleHandle(NULL));
	return app->Run();
}