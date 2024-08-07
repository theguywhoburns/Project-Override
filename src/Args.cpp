#include <Args.hpp>
bool Args::help = false;
bool Args::version = false;
bool Args::verbose = false;
std::string Args::config_file = "ProjectOverride.txt";
/**
 * Args syntax:
 * -# single char
 * --## multiple chars
 * -#= assign value
 * -# ' ' assign value
 * --##= assign value
 * --## ' ' assign value
 * example: -h --help -o output.PROJECT_OVERRIDE || -o=output.PROJECT_OVERRIDE 
*/
// TODO: implement this and fix the problem with no '-' args
void Args::Parse(int argc, char** argv) {
	for (int i = 1; i < argc; i++) {
		std::string_view arg = argv[i];
		if(arg.length() == 0) { throw std::invalid_argument("Empty argument"); }
		if(arg == "-h" || arg == "--help") { help = true; }
		else if(arg == "-v" || arg == "--version") { version = true; }	
		else if(arg == "-V" || arg == "--verbose") { verbose = true; }
		else if(arg.find('-') != std::string::npos) {
			throw std::invalid_argument("Invalid argument: " + std::string(arg));
		} else {
			if(config_file != "ProjectOverride.txt") throw std::invalid_argument("Config file is already set or unknown argument: " + std::string(arg)); 
			config_file = std::string(arg);
		}
	}
}