#pragma once

#include <Utils.hpp>

class Token {
public:
  enum class Type {
    TOKEN_EOF = 0,
    TOKEN_OPAREN,
    TOKEN_CPAREN,
    TOKEN_OSQUARE,
    TOKEN_CSQUARE,
    TOKEN_OCURLY,
    TOKEN_CCURLY,
    TOKEN_OANGLE,
    TOKEN_CANGLE,
    TOKEN_COMA,
    TOKEN_DOT,
    TOKEN_DOUBLE_DOT,
    TOKEN_IDENTIFIER,
    TOKEN_NUMBER,
    TOKEN_STRING,
    TOKEN_UNKNOWN,
  };
  Token(Type type, std::string_view value, size_t line, size_t column)
    : type(type), value(value), line(line), column(column) {}
  Type type;
  size_t line;
  size_t column;
  std::string_view value;
};

class TokenStream {
public:
  TokenStream(const std::string& path);
  ~TokenStream();
  Token next();
  Token peek();

private:
  std::string file_source;
  size_t file_index = 0;
  size_t line  = 1;
  size_t column= 1;
  Token last = Token(Token::Type::TOKEN_EOF, "", 0, 0);
 	Token lex_new();
  Token lex_number();
  Token lex_string();
  Token lex_identifier();
  

#if _DEBUG
  std::ofstream debug_lexer_file;
  std::string token_type_to_string(Token::Type type);
#endif
};