#include <Interface.hpp>
#include <Functions.hpp>

std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>> PROV_API GetAllExports() {
	return std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>> {
		{"Test", Test}
	};
}