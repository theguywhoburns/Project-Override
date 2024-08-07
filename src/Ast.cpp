#include <Ast.hpp>
/*

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
*/
std::vector<IAst> ParseTokenStream(TokenStream& ts) {
	Token token = Token(Token::Type::TOKEN_UNKNOWN, "", 0, 0); // default stuff
	while ((token = ts.next()).type != Token::Type::TOKEN_EOF) {
		if(token.type != Token::Type::TOKEN_IDENTIFIER) throw new std::runtime_error("Unexpected token: " + std::string(token.value));
		// then we parse all the identifiers
		// for that we need a map of default statements
	}
}