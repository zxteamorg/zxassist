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
#include "org/zxteam/lib/reusable/windows/utils.Screenshot.hpp"
#include "org/zxteam/lib/reusable/web/HttpClient.hpp"

#include "Windows.h"






//#include "../contract/FeatureHost.hpp"
//#include "ZXAssistApplication.hpp"
//#include <iostream>
//#include "org/zxteam/lib/reusable/runtime/DynamicLibrary.hpp"
//#include "org/zxteam/lib/reusable/fs/FileSystem.hpp"

using namespace org::zxteam::lib::reusable;
using namespace org::zxteam::lib::reusable::windows;
using namespace org::zxteam::lib::reusable::windows::utils;
using namespace org::zxteam::lib::reusable::web;

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
	void OnScreenshot(void) 
	{
		//String
		Screenshot screenshot = Screenshot::FromPrimaryScreen();
		//screenshot.Save("screenshot.png");
		//ShellExecute(NULL, L"open", L"screenshot.png", NULL, NULL, SW_SHOWDEFAULT);

		String data = screenshot.ToBase64String();
		const char* p = data.c_str();

		Url url;
		url.SetScheme("https");
		url.SetHost("api.imgur.com");
		url.SetPath("3/upload.xml");

		HttpClient httpClient;
		httpClient.SetUrl(url);
		httpClient.GetHeader().Add("Authorization", "Client-ID 5f2c70d3a6a89ad");
		httpClient.GetHeader().Add("Expect");
		httpClient.GetHeader().Add("Content-Type", "application/x-www-form-urlencoded");
		String xmlResult = httpClient.PerformSimpePost(data);
		const char* str = xmlResult.c_str();
		/*
<?xml version="1.0" encoding="utf-8"?>
<data type="array" success="1" status="200">
	<id>ppDhBrD</id><title/>
	<description/>
	<datetime>1463694294</datetime>
	<type>image/png</type>
	<animated>false</animated>
	<width>128</width>
	<height>128</height>
	<size>13952</size>
	<views>0</views>
	<bandwidth>0</bandwidth>
	<vote/>
	<favorite>false</favorite>
	<nsfw/><section/><account_url/>
	<account_id>0</account_id>
	<in_gallery>false</in_gallery>
	<deletehash>oOAycVjc8lIkFho</deletehash>
	<name/>
	<link>http://i.imgur.com/ppDhBrD.png</link></data>
		*/

		while(*str)
		{
			size_t len = std::strlen(str);
			if(len >=6)
			{
				if(0 == strnicmp(str, "<link>", 6))
				{
					str+=6;
					int count = 0;
					while(*(str + count) && *(str + count) != '<') count++;
					char* link = new char[count + 1];
					std::strncpy(link, str, count);
					link[count] = 0;
					ShellExecuteA(NULL, "open", link, NULL, NULL, SW_SHOWDEFAULT);
					delete link;
					break;
				}
			}
			++str;
		}
	}
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