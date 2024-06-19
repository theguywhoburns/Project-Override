using System.Text;

class Token {
	public enum Type {
		Power,		//^ 
		Plus,		//+ 
		Equals,     //= 
		Minus,		//- 
		Not,		// ! 
		At,			//@ 
		Dollar, 	//$ 
		Percent,	//% 
		And,		//& 
		Or,			//| 
		Probably,   //~ 
		Hashtag,	//# 
		Oparen,		//( 
		Cparen,		//) 
		Ocurly,		//{ 
		Ccurly,		//} 
		Osquare,	//[ 
		Csquare,	//] 
		Quote,		//' 
		DoubleQuote,//" 
		Dot,		//. 
		Comma,		//, 
		Colon,		//: 
		Semicolon, //; 
		Star, 		//* 
		Whitespace, //  
		Newline,	// \n 
		Tab,		// \t 
		Comment,	// //test 
		Slash,		// / 
		BackSlash,	//   
		StartMultiLineComment,// /* 
		EndMultiLineComment, // */ 
		EndOfFile,	//EOF 
		Identifier, //Everything else 
		Number, 	//Number(i64), 
		Boolean, 	//Boolean(bool),
		MaxTokens   //NOTE: This is for enum size, it's not a limit to tokens or a token itself 
	}

	public Type type;
	public String value = "";
	public int length;
	public int line;
	public int col;
	public Token(Type type, int line, int col, String value = "", int length = 1) {
		this.length = length;
		this.type = type;
		this.value = value;
		this.line = line;
		this.col = col;
	}
	public static Dictionary<char, Type> char_type = new Dictionary<char, Type> {
		{ '^', Type.Power },     //^
		{ '+', Type.Plus },      //+
		{ '=', Type.Equals },    //=
		{ '-', Type.Minus },     //-
		{ '!', Type.Not },       //!
		{ '@', Type.At },        //@
		{ '$', Type.Dollar },    //$
		{ '%', Type.Percent },   //%
		{ '&', Type.And },       //&
		{ '|', Type.Or },        //|
		{ '~', Type.Probably },  //~
		{ '#', Type.Hashtag },   //#
		{ '(', Type.Oparen },    //(
		{ ')', Type.Cparen },    //)
		{ '{', Type.Ocurly },    //{
		{ '}', Type.Ccurly },    //}
		{ '[', Type.Osquare },   //[
		{ ']', Type.Csquare },   //]
		{ '\'', Type.Quote },    //'
		{ '\"', Type.DoubleQuote }, //"
		{ '.', Type.Dot },       //.
		{ ',', Type.Comma },     //,
		{ ':', Type.Colon },     //:
		{ ';', Type.Semicolon }, //;
		{ ' ', Type.Whitespace }, // 
		{ '\n', Type.Newline },  // \n
		{ '\t', Type.Tab },      // \t
		{ '\\', Type.BackSlash } // \
	};

	public override string ToString() {
		switch (type) {
			case Type.Power:
				return "^";
			case Type.Plus:
				return "+";
			case Type.Equals:
				return "=";
			case Type.Minus:
				return "-";
			case Type.Not:
				return "!";
			case Type.At:
				return "@";
			case Type.Dollar:
				return "$";
			case Type.Percent:
				return "%";
			case Type.And:
				return "&";
			case Type.Or:
				return "|";
			case Type.Probably:
				return "~";
			case Type.Hashtag:
				return "#";
			case Type.Oparen:
				return "(";
			case Type.Cparen:
				return ")";
			case Type.Ocurly:
				return "{";
			case Type.Ccurly:
				return "}";
			case Type.Osquare:
				return "[";
			case Type.Csquare:
				return "]";
			case Type.Quote:
				return "'";
			case Type.DoubleQuote:
				return "\"";
			case Type.Dot:
				return ".";
			case Type.Comma:
				return ",";
			case Type.Colon:
				return ":";
			case Type.Semicolon:
				return ";";
			case Type.Star:
				return "*";
			case Type.Whitespace:
				return " ";
			case Type.Newline:
				return "\\n";
			case Type.Tab:
				return "\\t";
			case Type.Comment:
				return "//" + value != null ? value.ToString() : "";
			case Type.StartMultiLineComment:
				return "/*";
			case Type.EndMultiLineComment:
				return "*/";
			case Type.EndOfFile:
				return "EOF";
			case Type.Identifier:
				return value != null ? value.ToString() : "";
			case Type.Number:
				return value != null ? value.ToString() : "";
		}
		return "";
	}
}

static class Lexer {
	public static List<Token> Lex(String source) {
		const string DELIMITERS = "^+-=!@$%&|~@(){}[]'\".,:;* \n\t\r /\\";
		List<Token> ret = new List<Token>();
		int line = 1;
		int col = 1;
		for (int i = 0; i < source.Length; i++) {
			char c = source.ToCharArray()[i];
			if(Token.char_type.ContainsKey(c)) {
				ret.Add(new Token(Token.char_type[c], line, col, c.ToString(), 1));
				col += 1;
				if(Token.char_type[c] == Token.Type.Newline) {
					line++;
					col = 1;
				}
				continue;
			}
			switch (source.ToCharArray()[i]) {
				case '*':
					if (i+1 < source.Length && source[i+1] == '/') {
						ret.Add(new Token(Token.Type.EndMultiLineComment, line, col, "*/", 2));
						col += 2;
						i += 1;
					} else {
						ret.Add(new Token(Token.Type.Star, line, col, "*", 1));
						col += 1;
					}
					break;
				case '\r':
					if (i+1 < source.Length && source[i+1] == '\n') {
						ret.Add(new Token(Token.Type.Newline, line, col, "", 1));
						i += 1;
					} else {
						ret.Add(new Token(Token.Type.Newline, line, col, "", 1));
					}
					line++;
					col = 1;
					break;
				case '/':
					if (i+1 < source.Length && source[i+1] == '*') {
						ret.Add(new Token(Token.Type.StartMultiLineComment, line, col, "/*", 2));
						col += 2;
						i += 1;
					} else if (i+1 < source.Length && source[i+1] == '/') {
						int j = i+2;
						while (j < source.Length && (source[j] != '\n' && source[j] != '\r' && source[j+1] != '*' && source[j+1] != '/')) {
							j += 1;
						}
						String sub = source.Substring(i + 2, j - i - 2);
            ret.Add(new Token(Token.Type.Comment, line, col, sub, sub.Length + 2));
						col += sub.Length + 2;
						i = j;
					} else {
						ret.Add(new Token(Token.Type.Slash, line, col, "/", 1));
						col++;
					}
					break;
				default:
					int end = i + 1;
					if (char.IsDigit(source[i])) {
						while (end < source.Length && char.IsDigit(source[end])) {
							end += 1;
						}
						if (end < source.Length && !DELIMITERS.Contains((char)source[end])) {
							while (end < source.Length && !DELIMITERS.Contains((char)source[end])) {
								end += 1;
							}
							String sub = source.Substring(i, end - i);
							ret.Add(new Token(Token.Type.Identifier, line, col, sub, sub.Length));
							col += sub.Length;
						}	else {
							String sub = source.Substring(i, end - i);
							ret.Add(new Token(Token.Type.Number, line, col, sub, sub.Length));
							col += sub.Length;
						}
						i = end - 1;
					}	else {
						while (end < source.Length && !DELIMITERS.Contains((char)source[end]) && source[end] != ' ') {
							end += 1;
						}
						String sub = source.Substring(i, end - i);
						if(sub == "true" || sub == "false") {
							ret.Add(new Token(Token.Type.Boolean, line, col, sub == "true" ? "true" : "false", sub.Length));
							col += sub.Length;
							i = end - 1;
						} else {
							ret.Add(new Token(Token.Type.Identifier, line, col, sub, sub.Length));
							col += sub.Length;
							i = end - 1;
						}
					}
				break;
			}
			if(i == source.Length - 1) {
				ret.Add(new Token(Token.Type.EndOfFile, line, col, "", 1));
				col += 1;
			}
		}
		return ret;
	}
}