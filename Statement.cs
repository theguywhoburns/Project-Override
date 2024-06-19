class StatementDef {
	public List<AstNode.Type> var_types = new List<AstNode.Type>();
	public AstNode.Type ret_type;
	public Func<dynamic[], AstNode> Func;
	public StatementDef(Func<dynamic[], AstNode> Func, AstNode.Type ret_type, params AstNode.Type[] var_types) {
		this.Func = Func;
		this.var_types = var_types.ToList();
		this.ret_type = ret_type;
	}	

	public static Dictionary<string, StatementDef> statements = new Dictionary<string, StatementDef> {
		{"CONFIG_VERSION", new StatementDef(Statements.CONFIG_VERSION, AstNode.Type.NULL, AstNode.Type.VERSION | AstNode.Type.STRING)},
		{"MESSAGE", new StatementDef(Statements.MESSAGE, AstNode.Type.NULL, AstNode.Type.ANY_VAR)}
	};

	public static Dictionary<string, StatementDef> array_statements = new Dictionary<string, StatementDef> {
		{"MESSAGE", new StatementDef(Statements.MESSAGE, AstNode.Type.NULL,AstNode.Type.ANY_VAR)}
	};
}

static class Statements {
	public static AstNode MESSAGE(dynamic[] args) {
		foreach(dynamic arg in args) {
			Console.Write(arg.ToString());
		}
		Console.WriteLine();
		return new AstNode(AstNode.Type.NULL);
	}

	public static AstNode CONFIG_VERSION(dynamic[] args) {
		return new AstNode(AstNode.Type.NULL);
	}
}
