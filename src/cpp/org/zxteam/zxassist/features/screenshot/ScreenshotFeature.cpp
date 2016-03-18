#include "ScreenshotFeature.hpp"
#include "../../contract/Feature.hpp"
#include "org/zxteam/lib/reusable/String.hpp"
#include "org/zxteam/lib/reusable/system/devices/VirtualScreen.hpp"
#include <iostream>

using namespace org::zxteam::zxassist::features::screenshot;
using namespace org::zxteam::lib::reusable;
using namespace org::zxteam::lib::reusable::system::devices;

static ScreenshotFeature* feature;

void InitFeature(org::zxteam::zxassist::FeatureHost& host)
{
	if(feature == 0)
	{
		feature = new ScreenshotFeature();
	}
}

void ReleaseFeature(org::zxteam::zxassist::FeatureHost& host)
{
	if(feature != 0)
	{
		delete feature, feature = 0;
	}
}

ScreenshotFeature::ScreenshotFeature()
{
	VirtualScreenPtr virtualScreen = VirtualScreen::GetVirtualScreen();
	std::cout << "ScreenshotFeature::ScreenshotFeature()" << std::endl;
}

ScreenshotFeature::~ScreenshotFeature()
{
	VirtualScreenPtr virtualScreen = VirtualScreen::GetVirtualScreen();
	std::cout << "ScreenshotFeature::~ScreenshotFeature()" << std::endl;
}
