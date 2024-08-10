#include <Args.hpp>
#include <lexer.hpp>
#include <Ast.hpp>
int main(int argc, char** argv) {
	try {
		Utils::DynamicLibrary lib("bin/ProjectOverrideRuntime.dll");
		auto fun = lib.get<std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>>>("GetAllExports");
		auto ret = fun();
		for(auto& [k, v] : ret) {
			std::cout << k << " " << v({""}) << std::endl;
		}
		return 0;
		Args::Parse(argc, argv);
		if(Args::help) {
			std::cout << "Help" << std::endl;
			return 0;
		} else if(Args::version) {
			std::cout << 
			Utils::Ascii::BOLD_HIGH_INTENSITY_WHITE + "PROJECT_OVERRIDE: " + 
			Utils::Ascii::BOLD_HIGH_INTENSITY_BLUE  + (PROJECT_OVERRIDE_VERSION_STR) + 
			Utils::Ascii::RESET 
			<< std::endl;
			return 0;
		} 
		TokenStream ts(Args::config_file);
		while (ts.next().type != Token::Type::TOKEN_EOF);
	} catch (std::exception* e) {
		std::cout << 
		Utils::Ascii::BOLD_HIGH_INTENSITY_WHITE + "PROJECT_OVERRIDE: " + 
		Utils::Ascii::BOLD_HIGH_INTENSITY_RED + "error: " + 
		Utils::Ascii::RESET + e->what() << std::endl;
	}
	
	return 0;
}