#include <Functions.hpp>

std::string PROV_API Test(std::vector<std::string> args) {
	std::cout << "Test: " << std::endl;
	for (auto arg : args) {
		std::cout << arg << std::endl;
	}
	std::cout << "Test finished " << std::endl;
	return "Test";
}//