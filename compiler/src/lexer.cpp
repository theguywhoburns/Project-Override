#include "lexer.hpp"

TokenStream::TokenStream(const std::string& path) {
  if(!std::filesystem::exists(path)) throw new std::runtime_error("Failed to open file at path: " + path);
#if _DEBUG
  debug_lexer_file = std::ofstream(path + ".lexer_out");
#endif
  std::ifstream file(path, std::ios::in | std::ios::binary);
  if (!file.is_open()) throw new std::runtime_error("Failed to open file at path: " + path);
  file_source = file_source = std::string(std::istreambuf_iterator<char>(file), std::istreambuf_iterator<char>());
  this->last = lex_new();
}

TokenStream::~TokenStream() {
#if _DEBUG
  debug_lexer_file.close(); // Just in case
#endif
}

Token TokenStream::next() {
  #if _DEBUG
    std::string ret_str = "[" + token_type_to_string(this->last.type) + ":" + std::to_string(this->last.line) + ":" + std::to_string(this->last.column) + "]: " + std::string(this->last.value) + "\n";
    debug_lexer_file.write(ret_str.c_str(), ret_str.size()).flush();
  #endif
  return (this->last = (this->last.type == Token::Type::TOKEN_EOF ? this->last : lex_new() ));
}

Token TokenStream::peek() {
  return this->last;
}

#if _DEBUG
std::string TokenStream::token_type_to_string(Token::Type type) {
  switch (type) {
  case Token::Type::TOKEN_EOF:          return "EOF";
  case Token::Type::TOKEN_OPAREN:       return "OPEN_PAREN";
  case Token::Type::TOKEN_OSQUARE:      return "OPEN_SQUARE";
  case Token::Type::TOKEN_OCURLY:       return "OPEN_CURLY";
  case Token::Type::TOKEN_OANGLE:       return "OPEN_ANGLE";
  case Token::Type::TOKEN_CPAREN:       return "CLOSE_PAREN";
  case Token::Type::TOKEN_CSQUARE:      return "CLOSE_SQUARE";
  case Token::Type::TOKEN_CCURLY:       return "CLOSE_CURLY";
  case Token::Type::TOKEN_CANGLE:       return "CLOSE_ANGLE";
  case Token::Type::TOKEN_IDENTIFIER:   return "IDENTIFIER";
  case Token::Type::TOKEN_NUMBER:       return "NUMBER";
  case Token::Type::TOKEN_STRING:       return "STRING";
  default: return "UNKNOWN";
  }
}
#endif

Token TokenStream::lex_new() {
  while (file_index < file_source.size() && ( 
    file_source[file_index] == ' '  || 
    file_source[file_index] == '\t' || 
    file_source[file_index] == '\n' || 
    file_source[file_index] == '\r' || 
    file_source[file_index] == '/'
  )) {
  switch (file_source[file_index]) {
  case ' ':
  case '\t': column++;  break;
  case '\r': column = 1;break;
  case '\n': line++;    break;
  case '/':
    if (file_index >= file_source.size() - 1) throw new std::runtime_error("[Lexer error " + std::to_string(line) + ":" + std::to_string(column) + "]: Unknown / at the end of the file");
    if (file_source[file_index + 1] == '/') {
      while (file_index < file_source.size() && file_source[file_index] != '\n') file_index++;
      line++; column = 1;
    } else if (file_source[file_index + 1] == '*') {
      bool found_end = false;
      while(file_index < file_source.size() - 2 && !found_end) {
        if (file_source[file_index] == '\r') column = 1;
      	else if (file_source[file_index] == '\n') line++;
        else if((file_source[file_index] != '*' && file_source[file_index + 1] != '/')) found_end = true;
        column++; file_index++;
      }
      if(!found_end) throw new std::runtime_error("[Lexer error " + std::to_string(line) + ":" + std::to_string(column) + "]: Unterminated multiline comment");
      file_index += 2;column += 2;
    } else throw new std::runtime_error("[Lexer error " + std::to_string(line) + ":" + std::to_string(column) + "]: Unknown or misplaced /");
    break; // Useless break but i'll keep it lol
    default: 
    break;
  }
  file_index++;
  }
  if (file_index >= file_source.size() || file_source[file_index] == 0) return Token(Token::Type::TOKEN_EOF, "", line, column);
  switch (file_source[file_index]) {
  case '(': return Token(Token::Type::TOKEN_OPAREN, std::string_view(&file_source[file_index++], 1), line, column++); 
  case ')': return Token(Token::Type::TOKEN_CPAREN, std::string_view(&file_source[file_index++], 1), line, column++);
  case '{': return Token(Token::Type::TOKEN_OCURLY, std::string_view(&file_source[file_index++], 1), line, column++);
  case '}': return Token(Token::Type::TOKEN_CCURLY, std::string_view(&file_source[file_index++], 1), line, column++);
  case '[': return Token(Token::Type::TOKEN_OSQUARE,std::string_view(&file_source[file_index++], 1), line, column++);
  case ']': return Token(Token::Type::TOKEN_CSQUARE,std::string_view(&file_source[file_index++], 1), line, column++);
  case '<': return Token(Token::Type::TOKEN_OANGLE, std::string_view(&file_source[file_index++], 1), line, column++);
  case '>': return Token(Token::Type::TOKEN_CANGLE, std::string_view(&file_source[file_index++], 1), line, column++);
  case ',': return Token(Token::Type::TOKEN_COMA,   std::string_view(&file_source[file_index++], 1), line, column++);
  case '.': return Token(Token::Type::TOKEN_DOT,    std::string_view(&file_source[file_index++], 1), line, column++);
  case ':': return Token(Token::Type::TOKEN_DOUBLE_DOT, std::string_view(&file_source[file_index++], 1), line, column++);
  case '0':
  case '1':
  case '2':
  case '3':
  case '4':
  case '5':
  case '6':
  case '7':
  case '8':
  case '9':  return lex_number();
  case '"':
  case '\'': return lex_string();
  default:   
  if(!Utils::IsAlphaNumeric(file_source[file_index]) && file_source[file_index] != '_') throw new std::runtime_error("[Lexer error " + std::to_string(line) + ":" + std::to_string(column) + "]: Illegal character " + std::string(1, file_source[file_index]));
  return lex_identifier();
  }
}

Token TokenStream::lex_number() {
  size_t start = file_index, start_column = column;
  while (file_index < file_source.size() && Utils::IsNumber(file_source[file_index])) { file_index++; column++; }
  return Token(Token::Type::TOKEN_NUMBER, std::string_view(&file_source[start], file_index - start), line, start_column);
}

Token TokenStream::lex_string() {
  size_t start = file_index, start_column = column;
  char string_type = file_source[file_index++]; column++;
  while (file_index < file_source.size()) {
    char current_char = file_source[file_index++]; column++;
    if (current_char == '\n') { throw new std::runtime_error("Lexer error, line " + std::to_string(line) + ", column " + std::to_string(column) + ": Unterminated string"); }
    else if (current_char == string_type) {
      if (string_type == '\'' && file_index - start > 3) throw new std::runtime_error("Lexer error, line " + std::to_string(line) + ", column " + std::to_string(column) + ": single quoted string cannot be longer than 1 character");
      break;
    } 
    else if (current_char == '\\' && file_index < file_source.size()) { file_index++; column++; }
  }
  return Token(Token::Type::TOKEN_STRING, std::string_view(&file_source[start], file_index - start), line, start_column);
}

Token TokenStream::lex_identifier() {
  size_t start_index = file_index, start_column = column;
  while (file_index < file_source.size() && (Utils::IsAlphaNumeric(file_source[file_index]) || file_source[file_index] == '_')) { file_index++; column++; }
  return Token(Token::Type::TOKEN_IDENTIFIER, std::string_view(&file_source[start_index], file_index - start_index), line, start_column);
}