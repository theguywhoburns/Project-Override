#pragma once

#include <Utils.hpp>

PROV_API std::unordered_map<std::string, std::function<std::string(std::vector<std::string>)>> GetAllExports();