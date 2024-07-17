using System.Formats.Asn1;
using System.Net;

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
		{"MESSAGE", new StatementDef(Statements.MESSAGE, AstNode.Type.NULL, AstNode.Type.ANY_VAR)},
		{"LINK", new StatementDef(Statements.LINK, AstNode.Type.STRING, AstNode.Type.STRING)},
		{"SET", new StatementDef(Statements.SET, AstNode.Type.NULL, AstNode.Type.STRING, AstNode.Type.ANY_VAR)},
		{"GET", new StatementDef(Statements.GET, AstNode.Type.ANY_VAR, AstNode.Type.STRING)},
		{"INPUT", new StatementDef(Statements.INPUT, AstNode.Type.STRING)},
		{"INT_CAST", new StatementDef(Statements.INT_CAST, AstNode.Type.NUMBER, AstNode.Type.ANY_VAR)},
		{"BOOL_CAST", new StatementDef(Statements.BOOL_CAST, AstNode.Type.NUMBER, AstNode.Type.ANY_VAR)},
		{"STRING_CAST", new StatementDef(Statements.STRING_CAST, AstNode.Type.STRING, AstNode.Type.ANY_VAR)}
	};

	public static Dictionary<string, StatementDef> array_statements = new Dictionary<string, StatementDef> {
		{"MESSAGE", new StatementDef(Statements.MESSAGE, AstNode.Type.NULL,AstNode.Type.ANY_VAR)}
	};
}

static class Statements {
	static HttpClient web = new();
	static Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
	public static AstNode MESSAGE(dynamic[] args) {
		foreach(dynamic? arg in args) {
			
			Console.Write(arg == null ? "(null)" : arg.ToString());
		}
		Console.WriteLine();
		return new AstNode(AstNode.Type.NULL);
	}

	public static AstNode CONFIG_VERSION(dynamic[] args) {
		return new AstNode(AstNode.Type.NULL);
	}

	public static AstNode LINK(dynamic[] args) {
		string url = args[0].ToString();
		var stream = web.GetStreamAsync(url).Result;
		string ret = new StreamReader(stream).ReadToEnd();
    return new AstNode(AstNode.Type.STRING, 0, 0, ret);
	}

	public static AstNode SET(dynamic[] args) {
		string name = args[0].ToString();
		if(!variables.ContainsKey(name.ToString())) {
			variables.Add(name, args[1]);
		} else {
			variables[name] = args[1];
		}
		return new AstNode(AstNode.Type.NULL);
	}

	public static AstNode GET(dynamic[] args) {
		string name = args[0].ToString();
		if(!variables.ContainsKey(name)) {
			return new AstNode(AstNode.Type.NULL);
		} else {
			return new AstNode(AstNode.Type.ANY_VAR, 0, 0, variables[name]);
		}
	}

	public static AstNode INPUT(dynamic[] args) {
		string ret = Console.ReadLine()??"";
		return new AstNode(AstNode.Type.STRING, 0, 0, ret);
	}

	public static AstNode INT_CAST(dynamic[] args) {
		return new AstNode(AstNode.Type.NUMBER, 0, 0, int.Parse(args[0].ToString()));
	}

	public static AstNode BOOL_CAST(dynamic[] args) {
		return new AstNode(AstNode.Type.NUMBER, 0, 0, bool.Parse(args[0].ToString()));
	}

	public static AstNode STRING_CAST(dynamic[] args) {
		return new AstNode(AstNode.Type.STRING, 0, 0, args[0].ToString());
	}
}
