#include <Interface.hpp>
#include <Functions.hpp>

std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>> GetAllExports() {
	return std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>> {
		{"Test", Test}
	};
}