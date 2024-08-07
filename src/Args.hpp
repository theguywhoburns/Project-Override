#pragma once
#include <Utils.hpp>

class Args final{
public:
	static void Parse(int argc, char** argv);
	static bool version;
	static bool verbose;
	static bool help;
	static std::string config_file;
	Args() = delete;
};