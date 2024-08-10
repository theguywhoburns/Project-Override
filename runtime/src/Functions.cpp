#include <Functions.hpp>

PROV_API std::string Test(std::vector<std::string> args) {
	std::cout << "Test: " << std::endl;
	for (auto arg : args) {
		std::cout << arg << std::endl;
	}
	std::cout << "Test finished " << std::endl;
	return "Test";
}