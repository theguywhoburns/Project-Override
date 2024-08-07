using System;
using System.Collections.Generic;

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
		Semicolon,  //; 
		Star, 		//* 
		Whitespace, //  
		Newline,	// \n 
		Tab,		// \t 
		Comment,	// //test 
		Slash,		// / 
		BackSlash,	// \   
		StartMultiLineComment,// /* 
		EndMultiLineComment, // */ 
		EndOfFile,	//EOF 
		Identifier, //Everything else 
		Number, 	//Number(i64), 
		Boolean, 	//Boolean(bool),
		String,		//String(string)
		MaxTokens   //NOTE: This is for enum size, it's not a limit to tokens or a token itself 
	}

	public Type type;
	public string value = "";
	public int length;
	public int line;
	public int col;

	public Token(Type type, int line, int col, string value = "", int length = 1) {
		this.type = type;
		this.value = value;
		this.length = length;
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
		return type switch {
			Type.Power => "^",
			Type.Plus => "+",
			Type.Equals => "=",
			Type.Minus => "-",
			Type.Not => "!",
			Type.At => "@",
			Type.Dollar => "$",
			Type.Percent => "%",
			Type.And => "&",
			Type.Or => "|",
			Type.Probably => "~",
			Type.Hashtag => "#",
			Type.Oparen => "(",
			Type.Cparen => ")",
			Type.Ocurly => "{",
			Type.Ccurly => "}",
			Type.Osquare => "[",
			Type.Csquare => "]",
			Type.Quote => "'",
			Type.DoubleQuote => "\"",
			Type.Dot => ".",
			Type.Comma => ",",
			Type.Colon => ":",
			Type.Semicolon => ";",
			Type.Star => "*",
			Type.Whitespace => " ",
			Type.Newline => "\\n",
			Type.Tab => "\\t",
			Type.Comment => "//" + value,
			Type.StartMultiLineComment => "/*",
			Type.EndMultiLineComment => "*/",
			Type.EndOfFile => "EOF",
			Type.Identifier or Type.Number => value,
			Type.String => "\"" + value + "\"",
			_ => "",
		};
	}
}

static class Lexer {
	public static List<Token> Lex(string source) {
		const string DELIMITERS = "^+-=!@$%&|~@(){}[]'\".,:;* \n\t\r/\\#";
		List<Token> ret = new List<Token>();
		int line = 1;
		int col = 1;

		for (int i = 0; i < source.Length; i++) {
			char c = source[i];
			if (Token.char_type.ContainsKey(c)) {
				ret.Add(new Token(Token.char_type[c], line, col, c.ToString(), 1));
				col += 1;
				if (Token.char_type[c] == Token.Type.Newline) {
					line++;
					col = 1;
				}
				continue;
			}
			switch (c) {
				case '"': {
					string retString = "";
					int startCol = col;
					i++;
					while (i < source.Length && source[i] != '"') {
					  if (source[i] == '\\' && i + 1 < source.Length) {
					    switch (source[i + 1]) {
					      case 'n': retString += "\n"; break;
					      case 't': retString += "\t"; break;
					      case 'r': retString += "\r"; break;
					      case 'b': retString += "\b"; break;
					      case 'f': retString += "\f"; break;
					      default: retString += source[i + 1]; break;
					    }
					    i++;
					    col++;
					  } else {
					    retString += source[i];
					  }
					  i++;
					  col++;
					}
					if (i < source.Length && source[i] == '"') {
					    ret.Add(new Token(Token.Type.String, line, startCol, retString, retString.Length + 2));
					    col++;
					} else {
					    throw new Exception("Unterminated string literal");
					}
        }break;
        case '\'': {
					string retChar = "";
					int startCol = col;
					i++;
					while (i < source.Length && source[i] != '\'') {
					  if (source[i] == '\\' && i + 1 < source.Length) {
							switch (source[i + 1]) {
								case 'n': retChar += "\n"; break;
								case 't': retChar += "\t"; break;
								case 'r': retChar += "\r"; break;
								case 'b': retChar += "\b"; break;
								case 'f': retChar += "\f"; break;
								default: retChar += source[i + 1]; break;
							}
							i++;
							col++;
					  } else {
					    retChar += source[i];
					  }
					  i++;
					  col++;
					}
					if (i < source.Length && source[i] == '\'') {
					  ret.Add(new Token(Token.Type.String, line, startCol, retChar, retChar.Length + 2));
					  col++;
					} else {
					  throw new Exception("Unterminated character literal");
					}
        }break;
				case '*':
					if (i + 1 < source.Length && source[i + 1] == '/') {
						ret.Add(new Token(Token.Type.EndMultiLineComment, line, col, "*/", 2));
						col += 2;
						i++;
					} else {
						ret.Add(new Token(Token.Type.Star, line, col, "*", 1));
						col += 1;
					}
					break;
				case '\r':
					if (i + 1 < source.Length && source[i + 1] == '\n') {
						ret.Add(new Token(Token.Type.Newline, line, col, "", 1));
						i++;
					} else {
						ret.Add(new Token(Token.Type.Newline, line, col, "", 1));
					}
					line++;
					col = 1;
					break;
				case '/':
					if (i + 1 < source.Length && source[i + 1] == '*') {
						ret.Add(new Token(Token.Type.StartMultiLineComment, line, col, "/*", 2));
						col += 2;
						i++;
					} else if (i + 1 < source.Length && source[i + 1] == '/') {
						int j = i + 2;
						while (j < source.Length && source[j] != '\n' && source[j] != '\r') {
							j++;
						}
						string sub = source.Substring(i + 2, j - i - 2);
						ret.Add(new Token(Token.Type.Comment, line, col, sub, sub.Length + 2));
						col += sub.Length + 2;
						i = j - 1;
					} else {
						ret.Add(new Token(Token.Type.Slash, line, col, "/", 1));
						col++;
					}
					break;
				default:
					int end = i + 1;
					if (char.IsDigit(c)) {
						while (end < source.Length && char.IsDigit(source[end])) {
							end++;
						}
						string sub = source.Substring(i, end - i);
						ret.Add(new Token(Token.Type.Number, line, col, sub, sub.Length));
						col += sub.Length;
						i = end - 1;
					} else {
						while (end < source.Length && !DELIMITERS.Contains(source[end])) {
							end++;
						}
						string sub = source.Substring(i, end - i);
						if (sub == "true" || sub == "false") {
							ret.Add(new Token(Token.Type.Boolean, line, col, sub, sub.Length));
						} else {
							ret.Add(new Token(Token.Type.Identifier, line, col, sub, sub.Length));
						}
						col += sub.Length;
						i = end - 1;
					}
					break;
			}
			if (i == source.Length - 1) {
				ret.Add(new Token(Token.Type.EndOfFile, line, col, "", 1));
				col += 1;
			}
		}
		return ret;
	}
}