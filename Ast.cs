class AstNode {
	public List<AstNode> children = new List<AstNode>();
	public int line=0;
	public int col=0;
	public enum Type {
		STATEMENT = 0b0000000000000000,
		STRING 		= 0b0000000000000001, 
		NUMBER 		= 0b0000000000000010,
		BOOLEAN		= 0b0000000000000100,
		NULL 			= 0b0000000000001000,
		VERSION 	= 0b0000000000010000,
		ANY_VAR 	= 0b0000000000011111, // Used for statement defs that accept any type of argument
		ADD 			= 0b0000000000100000, // +
		MULTIPLY 	= 0b0000000001000000, // *
		DIVIDE 		= 0b0000000010000000,	// /
		SUBSTRACT = 0b0000000100000000, // =
		MODULO 		= 0b0000001000000000, // %
		EXPRESSION= 0b0000010000000000, // expression, any thing like add sub or mul is combined in the expression ast
	}

	public dynamic? value;
	public Type type;
	
	public AstNode(Type type, int line = 0, int col = 0, dynamic? value = null) {
		this.type = type;
		this.value = value;
		this.line = line;
		this.col = col;
	}
}

static class AST {
	public static dynamic ExecuteStatement(AstNode ast) {
		dynamic[] args = new dynamic[ast.children.Count];
		for(int i = 0; i < ast.children.Count; i++) {
			switch (ast.children[i].type)
			{
				case AstNode.Type.STRING:
					args[i] = ast.children[i].value ?? "";
					break;
				case AstNode.Type.NUMBER:
					args[i] = ast.children[i].value ?? 0;
					break;
				case AstNode.Type.BOOLEAN:
					args[i] = ast.children[i].value ?? false;
					break;
				case AstNode.Type.NULL:
					ProjectOverride.errors.Add(new Error("RUNTIME COMPILER ERROR: Unexpected node type " + ast.type.ToString(), ast.line, ast.col));
					break;
				case AstNode.Type.STATEMENT:
					args[i] = ExecuteStatement(ast.children[i]);
					break;
				default:
					args[i] = ast.children[i].value ?? "";
					break;
			}
		}
		AstNode _ret = ast.value(args);
		dynamic ret = "";
		switch(_ret.type) {
		case AstNode.Type.STRING:
			ret = _ret.value??"";
			break;
		case AstNode.Type.NUMBER:
			ret = _ret.value??0;
			break;
		case AstNode.Type.BOOLEAN:
			ret = _ret.value??false;
			break;	
		case AstNode.Type.NULL:
			ProjectOverride.errors.Add(new Error("RUNTIME COMPILER ERROR: Unexpected node type " + _ret.type.ToString(), _ret.line, _ret.col));
			ProjectOverride.PrintErrors();
			Environment.Exit(1);
			break;
		case AstNode.Type.STATEMENT:
			ret = ExecuteStatement(_ret);
			break;
		default:
			ret = _ret.value??"";
			break;
		}
		return ret;
	}
	public static void Execute(List<AstNode> asts) {
		foreach(AstNode ast in asts) {
			if(ast.type != AstNode.Type.STATEMENT) {
				// TODO: Line and column
				ProjectOverride.errors.Add(new Error("RUNTIME COMPILER ERROR: Unexpected node type " + ast.type.ToString(), ast.line, ast.col));
			}
			
			dynamic[] args = new dynamic[ast.children.Count];
			for(int i = 0; i < ast.children.Count; i++) {
				switch(ast.children[i].type) {
					case AstNode.Type.STRING:
						args[i] = ast.children[i].value??"";
						break;
					case AstNode.Type.NUMBER:
						args[i] = ast.children[i].value??0;
						break;
					case AstNode.Type.BOOLEAN:
						args[i] = ast.children[i].value??false;
						break;
					case AstNode.Type.NULL:
						ProjectOverride.errors.Add(new Error("RUNTIME COMPILER ERROR: Unexpected node type " + ast.type.ToString(), ast.line, ast.col));
						break;
					case AstNode.Type.STATEMENT:
						args[i] = ExecuteStatement(ast.children[i]);
						break;
					default:
						args[i] = ast.children[i].value??"";
						break;
				}
			}
			if(ast.value == null) {
				ProjectOverride.errors.Add(new Error("RUNTIME COMPILER ERROR: NULL statement", ast.line, ast.col));
			}	
			ast.value(args);
		}
	}

	public static List<AstNode> ParseTokens(List<Token> tokens) {
		List<AstNode> ret = new List<AstNode>();
		bool found_config_version = false;
		for(int i = 0; i < tokens.Count; i++) {
			switch(tokens[i].type) {
			case Token.Type.Comment:
			case Token.Type.Newline:
			case Token.Type.Tab:
			case Token.Type.Whitespace:
				break;
			case Token.Type.StartMultiLineComment:
				i += 1;
				GetRidOfMultiLineComment(tokens, ref i);
				break;

			case Token.Type.Identifier:
				if(!found_config_version) {
					if(tokens[i].value != "CONFIG_VERSION") {
						ProjectOverride.errors.Add(new Error("CRITICAL ERROR: CONFIG_VERSION statement not found!!!", tokens[i].line, tokens[i].col));
						return ret;
					} else {
						found_config_version = true;
					}
				}
				AstNode? ident = ParseIdentifier(ref tokens, ref i, ref ret);
				if(ident != null) ret.Add(ident);
				break;
			case Token.Type.EndOfFile:
				break;
			default: 
				ProjectOverride.errors.Add(new Error("Unexpected token " + tokens[i].ToString(), tokens[i].line, tokens[i].col));
				break;
			}
		}
		return ret;
	}

	private static AstNode? ParseIdentifier(ref List<Token> tokens, ref int i, ref List<AstNode> ret) {
		int cont = -1; 
		// -1 isn't in statement defs
		// 0 in normal statement defs
		// 1 in array  statement defs
		// 2 in both statement defs
		if(StatementDef.statements.ContainsKey(tokens[i].value)) 
			cont = 0;
		if(StatementDef.array_statements.ContainsKey(tokens[i].value)) 
			cont = cont == 0 ? 2 : 1;
		if(cont == -1) {
			ProjectOverride.errors.Add(new Error("Unexpected identifier " + tokens[i].ToString(), tokens[i].line, tokens[i].col));
			while(tokens[i].type != Token.Type.Newline) {
				if(i >= tokens.Count) {
					return null;
				}
				i++;
			}
			return null;
		}
		int stat_i = i;
		if(i >= tokens.Count) {
			ProjectOverride.errors.Add(new Error("Unexpected EOF", tokens[i-1].line, tokens[i-1].col));
			return null;
		}
		i++;
		while(i < tokens.Count && tokens[i].type == Token.Type.Whitespace || tokens[i].type == Token.Type.Tab || tokens[i].type == Token.Type.Newline) {
			i++;
		}
		if(i < tokens.Count && tokens[i].type == Token.Type.Oparen && cont != 1) {
			cont = 0;
			i++;
		} else if(i < tokens.Count && tokens[i].type == Token.Type.Osquare && cont != 0) {
			cont = 1;
			i++;
		} else {
			string expected;
			if(cont == 2) {
				expected = "( or [";
			} else if(cont == 1) {
				expected = "[";
			} else {
				expected = "(";
			}
			ProjectOverride.errors.Add(new Error("Unexpected token, expected " + expected + " got: " + tokens[i].ToString(), tokens[i].line, tokens[i].col));
			while(i < tokens.Count && tokens[i].type != Token.Type.Newline) {
				i++;
			}
			return null;
		}
		SkipWhitespaceAndTab(tokens, ref i);
		StatementDef statement_def = cont == 0 ? StatementDef.statements[tokens[stat_i].value] : StatementDef.array_statements[tokens[stat_i].value];
		int statement_i = i;
		AstNode statement = new AstNode(AstNode.Type.STATEMENT, tokens[i].line, tokens[i].col, null);
		List<AstNode>? args = GetArgs(tokens, ref i);
		if(tokens[i].type != Token.Type.Cparen && cont == 0) {
			ProjectOverride.errors.Add(new Error("Unexpected token, expected )", tokens[i - 1].line, tokens[i - 1].col));
			while(i < tokens.Count && tokens[i].type != Token.Type.Newline) {
				i++;
			}
			return null;
		} else if(tokens[i].type != Token.Type.Csquare && cont == 1) {
			ProjectOverride.errors.Add(new Error("Unexpected token, expected ]", tokens[i - 1].line, tokens[i - 1].col));
			while(i < tokens.Count && tokens[i].type != Token.Type.Newline) {
				i++;
			}
			return null;
		} else {
			i++;
		}
		if(args == null) {
			return null;
		}
		if(args.Count != statement_def.var_types.Count && cont == 0) {
			ProjectOverride.errors.Add(new Error("Expected " + statement_def.var_types.Count + " arguments, got " + args.Count, tokens[i - 1].line, tokens[i - 1].col));
			while(i < tokens.Count && tokens[i].type != Token.Type.Newline) {
				i++;
			}
			return null;
		}
		if(args.Count < statement_def.var_types.Count && cont == 1) {
			ProjectOverride.errors.Add(new Error("Expected at least " + statement_def.var_types.Count + " arguments, got " + args.Count, tokens[i - 1].line, tokens[i - 1].col));
		}
		int j = 0;
		for(; j < statement_def.var_types.Count; j++) {
			if((statement_def.var_types[j] & args[j].type) != args[j].type) {
				ProjectOverride.errors.Add(new Error("Expected type " + statement_def.var_types[j] + ", got " + args[j].type, args[j].line, args[j].col));
				while(i < tokens.Count && tokens[i].type != Token.Type.Newline) {
					i++;
				}
				return null;
			}
		}
		if(j < args.Count) {
			int last_j = j-1;
			for(; j < args.Count; j++) {
				AstNode.Type t = statement_def.var_types[last_j];
				AstNode.Type t2 = args[j].type;
				if((t & t2) != t2) {
					ProjectOverride.errors.Add(new Error("Expected type " + statement_def.var_types[last_j] + ", got " + args[j].type, args[j].line, args[j].col));
					while(i < tokens.Count && tokens[i].type != Token.Type.Newline) {
						i++;
					}
					return null;
				}
			}
		}
				
		statement.children = args;
		statement.value = statement_def.Func;
		return statement;
	}

	private static List<AstNode> ?GetArgs(List<Token> tokens, ref int i) {
		List<AstNode> ret = new List<AstNode>();
		List<AstNode> args = new List<AstNode>();
		SkipWhitespaceAndTab(tokens, ref i);
		while (tokens[i].type != Token.Type.Cparen && tokens[i].type != Token.Type.Csquare && i < tokens.Count) {
			SkipWhitespaceAndTab(tokens, ref i);
			while(tokens[i].type != Token.Type.Comma && tokens[i].type != Token.Type.Cparen && tokens[i].type != Token.Type.Csquare) {
				SkipWhitespaceAndTab(tokens, ref i);
				if(tokens[i].type == Token.Type.String) {
					AstNode str = new AstNode(AstNode.Type.STRING, tokens[i].line, tokens[i].col, tokens[i].value);
					args.Add(str);
					i++;
				} else if(tokens[i].type == Token.Type.Newline) {
					if(tokens[i-1].type != Token.Type.Comma) {
						
						while(tokens[i].type == Token.Type.Newline || tokens[i].type == Token.Type.Whitespace || tokens[i].type == Token.Type.Tab) {
							i++;
						}
						if(tokens[i].type != Token.Type.Identifier) {
							ProjectOverride.errors.Add(new Error("Unexpected newline", tokens[i].line, tokens[i].col));
							return null;
						}
					} else {
						i++;
					}
					SkipWhitespaceAndTab(tokens, ref i);
					continue;
				} else if(tokens[i].type == Token.Type.Identifier) {
					AstNode? ident = ParseIdentifier(ref tokens, ref i, ref ret);
					if(ident == null) {
						return null;
					}
					args.Add(ident);
				} else if (tokens[i].type == Token.Type.Number) {
					AstNode num = new AstNode(AstNode.Type.NUMBER, tokens[i].line, tokens[i].col, int.Parse(tokens[i].value));
					args.Add(num);
				} else if(tokens[i].type == Token.Type.Boolean) {
					AstNode boolean = new AstNode(AstNode.Type.BOOLEAN, tokens[i].line, tokens[i].col, tokens[i].value == "true");
					args.Add(boolean);
				} else {
					if(tokens[i].type == Token.Type.Cparen || tokens[i].type == Token.Type.Csquare || tokens[i].type == Token.Type.Comma) {
						break;
					}
					ProjectOverride.errors.Add(new Error("Unexpected token " + tokens[i].ToString(), tokens[i].line, tokens[i].col));
					while(i < tokens.Count && tokens[i].type != Token.Type.Cparen && tokens[i].type != Token.Type.Csquare) {
						i++;
					}
					return null;
				}
				if(tokens[i].type == Token.Type.Cparen || tokens[i].type == Token.Type.Csquare || tokens[i].type == Token.Type.Comma) {
					break;
				}
				i++;
			}
			AstNode? combined = CombineArgs(args);
			args.Clear();
			if(combined == null) {
				return null;
			}
			if(tokens[i].type == Token.Type.Comma) {
				i++;
			}
			
			ret.Add(combined);
		}
		return ret;
	}

	// !!!
	// IMPORTANT: TODO: Implement proper CombineArgs and add Error handling
	private static AstNode? CombineArgs(List<AstNode> args) {
		AstNode ret;
		if(args.Count > 1) {
		ret = new AstNode(AstNode.Type.EXPRESSION, args[0].line, args[0].col, null);
		foreach(AstNode arg in args) {
			ret.children.Add(arg);
		}
		} else {
			ret = args[0];
		}

		return ret;
	}

	private static void SkipWhitespaceAndTab(List<Token> tokens, ref int i) {
		while(tokens[i].type == Token.Type.Whitespace || tokens[i].type == Token.Type.Tab) {
			i++;
		}
	}

	private static void GetRidOfMultiLineComment(List<Token> tokens, ref int i) {
		while(tokens[i].type != Token.Type.EndMultiLineComment) {
			i++;
		}
	}	
}