#include <Utils.hpp>
#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
#include <Windows.h>
#elif defined(unix) || defined(__unix__) || defined(__unix)
#include <dlfcn.h>
#else
#error "Unsupported platform"
#endif
namespace Utils {
	DynamicLibrary::DynamicLibrary(const std::string& path) {
	#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
		_handle = reinterpret_cast<void*>(LoadLibraryA(path.c_str()));
		#elif defined(unix) || defined(__unix__) || defined(__unix)
		_handle = dlopen(path.c_str(), RTLD_NOW);
	#endif

		if (_handle == nullptr) {
			throw new std::runtime_error("Failed to load library: " + path);
		}
	}

  
  void* DynamicLibrary::_get(const std::string& function_name) {
		#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
			auto function = reinterpret_cast<void*>(GetProcAddress(HMODULE(_handle), function_name.c_str()));
		#elif defined(unix) || defined(__unix__) || defined(__unix)
			auto function = reinterpret_cast<void*>(dlsym(_handle, function_name.c_str()));
		#endif
		if (function == nullptr) {
			#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
				std::string err; 
				err.resize(1024);
				FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, GetLastError(), MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), err.data(), err.size(), NULL);
				throw new std::runtime_error("Failed to get function: \"" + function_name + "\" " + err);
			#elif defined(unix) || defined(__unix__) || defined(__unix)
				throw new std::runtime_error("Failed to get function: " + function_name);
			#endif
		}
		return function;
	}
};