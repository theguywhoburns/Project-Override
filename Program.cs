using System.Text;

class Program {
	public static void Main(string[] args) {
		Args.Parse(args);
		if(!File.Exists(Args.File)) {
			Console.Error.WriteLine("Error, file not found: " + Args.File);
			Environment.Exit(1);
		}
		string source = File.ReadAllText(Args.File);
		List<Token> tokens = Lexer.Lex(source);
		if(Args.Debug) {
			FileStream f = File.Create("tokens.txt");
			foreach (Token token in tokens) {
    		f.Write(Encoding.Unicode.GetBytes($"[{token.type}, {token.line}:{token.col}]: {token}\n"));
			}
			f.Flush();
			f.Close();
		}

		List<AstNode> asts = AST.ParseTokens(tokens);
		if(Args.Debug) {
			FileStream f = File.Create("asts.txt");
			foreach (AstNode ast in asts) {
				f.Write(Encoding.Unicode.GetBytes($"{DebugRecursiveAST(ast)}\n"));
			}
			f.Flush();
			f.Close();
		}
		if(ProjectOverride.errors.Any()) {
			foreach(Error error in ProjectOverride.errors) {
				Console.Error.WriteLine(error.message + " at " + error.line + ":" + error.col);
			}
			Environment.Exit(1);
		}

		AST.Execute(asts);
	}

	static string DebugRecursiveAST(AstNode node) {
		string ret = "[";
		ret += node.type;
		if(node.type != AstNode.Type.STATEMENT) {
			ret += ":";
			ret += node.value??"null"; 
		}
		ret += "]";
		if(node.children.Any()) {
			ret += "{";
		}
		foreach(AstNode child in node.children) {
			ret += DebugRecursiveAST(child);
		}
		if(node.children.Any()) {
			ret += "}";
		}
		return ret;
	}
}
