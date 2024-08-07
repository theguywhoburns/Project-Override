#pragma once
#include <Utils.hpp>
#include <Ast.hpp>
// using the structure from the c# version of this project, will need to rewrite the whole system
class IAst;
class StatementDefinition {
public:
	Constant::ConstantType return_type;
	std::vector<std::vector<Constant::ConstantType>> arg_types;
	std::function<void(std::vector<IAst>, IAst&)> function;
	std::unordered_map<std::string, StatementDefinition> statements;
};