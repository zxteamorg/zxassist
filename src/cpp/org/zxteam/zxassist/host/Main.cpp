//*******************************************************//
// Copyright © 2014-2016 ZXTeam http://www.zxteam.org    //
// License: http://opensource.org/licenses/MIT           //
//*******************************************************//
// Author: Maxim Anurin <maxim.anurin@zxteam.net>        //
// Web: http://www.anurin.name                           //
// SkypeId: maxim.anurin                                 //
//*******************************************************//

#include "org/zxteam/lib/reusable/Object.hpp"
#include "org/zxteam/lib/reusable/Action.hpp"
#include "org/zxteam/lib/reusable/windows/Application.hpp"
#include "org/zxteam/lib/reusable/windows/Menu.hpp"
#include "org/zxteam/lib/reusable/windows/TrayIcon.hpp"

#include "Windows.h"


//#include "../contract/FeatureHost.hpp"
//#include "ZXAssistApplication.hpp"
//#include <iostream>
//#include "org/zxteam/lib/reusable/runtime/DynamicLibrary.hpp"
//#include "org/zxteam/lib/reusable/fs/FileSystem.hpp"

using namespace org::zxteam::lib::reusable;
using namespace org::zxteam::lib::reusable::windows;

//using namespace org::zxteam::zxassist::host;

class MyApplication : public Application
{
public:
	virtual void Run()
	{
		typedef Action0Instance<MyApplication> MenuAction;
		MenuAction onScreenshotAction(this, &MyApplication::OnScreenshot);
		MenuAction onPreferencesAction(this, &MyApplication::OnPreferences);
		MenuAction onExitAction(this, &MyApplication::OnExit);

		TrayIcon ti;
		ti.SetIcon("zxassist_48x48.png");
		ti.SetToolTip("ZXAssist C++");
		Menu mainMenu;
		mainMenu.AddAction("Make Screenshot",  onScreenshotAction);
		mainMenu.AddAction("Preferences",  onPreferencesAction);
		mainMenu.AddAction("Exit", onExitAction);
		ti.SetMenu(mainMenu);
		
		Application::Run();
	}
private:
	void OnExit(void) { this->Shutdown(); }
	void OnScreenshot(void) { MessageBoxA(0, "TODO Make Screenshot", "ZXAssist C++", MB_OK); }
	void OnPreferences(void) { MessageBoxA(0, "TODO Open Preferences Window", "ZXAssist C++", MB_OK); }
}/* MyApplication */;

int main(int argc, char** argv)
{
	if(TrayIcon::IsTraySubsystemAvailable())
	{
		MyApplication app;
		app.Run();
	}
	return 0;
} 