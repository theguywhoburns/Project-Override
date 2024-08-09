#include <Utils.hpp>
#include <iostream>
extern "C" __declspec(dllexport) int __cdecl Test(void) { std::cout << "Hello, World!" << std::endl; return 141068; }
