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
	
  Constant(const std::string& constant) : IAst(AstType::AST_CONSTANT), type(ConstantType::type_string), value(constant) {}
    
  template<Utils::NumericBooleanOrString T>
  T get() { return std::get<T>(value); }
  ConstantType get_type() {return type;}
private:
  ConstantType type;
  std::string value;
};

class Variable : public Constant {
	std::string name;
public:
	// We'll need more ctors later
	Variable(std::string name, std::string value) : Constant(value), name(name) {}
};

std::vector<IAst> ParseTokenStream(TokenStream& ts);

//can there be classes for operators?
//yes but for this well have to make visitors to get rid of bloat code
//https://en.wikipedia.org/wiki/Visitor_pattern
// but this is later, now we need to make statements, variables and constants