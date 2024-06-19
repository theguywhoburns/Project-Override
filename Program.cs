
using System.Formats.Asn1;
using System.Text;
using System.Text.Unicode;

class Program {
	public static void Main(string[] args) {
		Args.Parse(args);
		if(!File.Exists(Args.File)) {
			Console.Error.WriteLine("Error, file not found: " + Args.File);
			Environment.Exit(1);
		}
		String source = File.ReadAllText(Args.File);
		List<Token> tokens = Lexer.Lex(source);
		if(Args.Debug) {
			FileStream f = File.Create("tokens.txt");
			foreach (Token token in tokens) {
				string tokstr = $"[{token.type}, {token.line}:{token.col}]: {token.ToString()}\n";
				byte[] tokenBytes = Encoding.Unicode.GetBytes(tokstr);
    		f.Write(tokenBytes);
			}
			f.Flush();
			f.Close();
		}

		List<AstNode> asts = AST.ParseTokens(tokens);
		if(Args.Debug) {
			FileStream f = File.Create("asts.txt");
			foreach (AstNode ast in asts) {
				string aststr = $"{DebugRecursiveAST(ast)}\n";
				byte[] astBytes = Encoding.Unicode.GetBytes(aststr);
				f.Write(astBytes);
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

	static String DebugRecursiveAST(AstNode node) {
		String ret = "[";
		ret += node.type;
		ret += ":";
		ret += node.value??"null"; 
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