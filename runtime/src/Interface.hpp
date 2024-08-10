#pragma once

#include <Utils.hpp>

std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>> PROV_API GetAllExports();