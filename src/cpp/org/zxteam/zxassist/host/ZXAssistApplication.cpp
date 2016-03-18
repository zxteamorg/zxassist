#include "ZXAssistApplication.hpp"
#include "org/zxteam/lib/reusable/runtime/DynamicLibrary.hpp"
//#include "org/zxteam/lib/reusable/Finalizer.hpp"

using namespace org::zxteam::lib::reusable;
using namespace org::zxteam::lib::reusable::runtime;
using namespace org::zxteam::zxassist::host;

int ZXAssistApplication::Run(void)
{
	InitFeatures();
	int retcode = DesktopApplication::Run();
	ReleaseFeatures();
	return retcode;
}

typedef void (*PLUGIN_FACTORYFUNC)(org::zxteam::zxassist::FeatureHost&);

void ZXAssistApplication::InitFeatures(void)
{
	DynamicLibraryPtr lib = new DynamicLibrary(L"ZXAssist.Feature.Screenshot.dll");
	this->_features.push_back(lib);

	PLUGIN_FACTORYFUNC InitFeature = lib->FindFunction<PLUGIN_FACTORYFUNC>("InitFeature");
	InitFeature(*this);
}

void ZXAssistApplication::ReleaseFeatures(void)
{
	for(FeaturesVector::iterator it = this->_features.begin(); it != this->_features.end(); ++it) {
		DynamicLibraryPtr lib = *it;
		/* std::cout << *it; ... */
		PLUGIN_FACTORYFUNC ReleaseFeature = lib->FindFunction<PLUGIN_FACTORYFUNC>("ReleaseFeature");
		ReleaseFeature(*this);
	}
}
