#pragma once
#include <Utils.hpp>
#include <lexer.hpp>
// Here goes the hard part lol
class IAst {
public:
	enum AstType {
		AST_STATEMENT, // Built-in callable function 
		AST_CONSTANT,
		AST_VARIABLE // don't think we'll need it
	};
	AstType ast_type;
	IAst(AstType ast_type) : ast_type(ast_type) {}
};

// Needs to be rewritten 100%
class CallableAst : public IAst {
	std::function<void(std::vector<IAst>, IAst& ret)> function;
public:
	CallableAst(std::function<void(std::vector<IAst>, IAst&)> function) : IAst(AstType::AST_STATEMENT), function(function) {}
	void call(std::vector<IAst> args, IAst& ret) { function(args, ret); }
};

class Constant : public IAst{
public: 
	enum ConstantType{ 
		type_bool,
		type_string,
		type_float,
		type_int,
		type_max
	};
	template<Utils::NumericBooleanOrString T>
  Constant(const T& constant) : IAst(AstType::AST_CONSTANT), constant_type(typeid(T) == typeid(bool) ? ConstantType::type_bool : typeid(T) == typeid(std::string) ? ConstantType::type_string : typeid(T) == typeid(double) ? ConstantType::type_float : ConstantType::type_int), value(constant) {}
  Constant(const Constant& constant) : IAst(AstType::AST_CONSTANT), constant_type(constant.constant_type), value(constant.value) {}
  template<Utils::NumericBooleanOrString T>
  T get() const{ return std::get<T>(value); }
	template<Utils::NumericBooleanOrString T>
	void set(T value) { this->value = value; this->constant_type = typeid(T) == typeid(bool) ? ConstantType::type_bool : typeid(T) == typeid(std::string) ? ConstantType::type_string : typeid(T) == typeid(double) ? ConstantType::type_float : ConstantType::type_int; }
  ConstantType get_type() const {return constant_type;}
private:
  ConstantType constant_type;
  std::variant<bool, std::string, double, int> value;
};

class Variable : public Constant {
	std::string name;
public:
	template<Utils::NumericBooleanOrString T>
	Variable(std::string name, const T& value) : Constant(value), name(name) {}
	Variable(std::string name, Variable& value): Constant(value), name(name) {}
};

std::vector<IAst> ParseTokenStream(TokenStream& ts);

//can there be classes for operators?
//yes but for this well have to make visitors to get rid of bloat code
//https://en.wikipedia.org/wiki/Visitor_pattern
// but this is later, now we need to make statements, variables and constants