using System.Text;

static class Args {
	public static bool Debug = true;
	public static string File = "ProjectOverride.txt";
	public static string ProjectFolder = "./";
	public const string VERSION = "0.0.1";
	public static void Parse(string[] args) {
		if (args.Length > 0) {
			args = args[1..];
		}
		if(args.Length == 0) {
			return;
		}
	for (int i = 0; i < args.Length; i++) {
		if(args[i] == "-h" || args[i] == "--help") {
			Help();
		} else if(args[i] == "-v" || args[i] == "--version") {
			Console.WriteLine("ProjectOverrde: V" + VERSION);
		} else if(args[i] == "-f" || args[i] == "--file") {
			if (i + 1 >= args.Length)
			{
				Console.Error.WriteLine($"Error, has {args[i]} but no file specified");
				Environment.Exit(1);
			}
			if (args[i + 1].StartsWith("\"") && !args[i + 1].EndsWith("\"")) {
				StringBuilder tmp = new StringBuilder(args[i + 1]);
				int j = i + 2;
				while (j < args.Length && !args[j].Contains("\""))
				{
					tmp.Append(' ').Append(args[j]);
					j++;
				}
				if (j == args.Length)
				{
					Console.Error.WriteLine("Error, no closing \" found");
					Environment.Exit(1);
				}
				File = tmp.ToString(1, tmp.Length - 2);
			} else {
				File = args[i + 1];
			}

			i++;
		} else if(args[i] == "--debug-mode") {
			Debug = true;
		} else if(args[i] == "--project-folder" || args[i] == "-p") {
			if(i + 1 < args.Length) {
				Console.Error.WriteLine("Error, has" + args[i] + " but no file specified");
				Environment.Exit(1);
			} 
			ProjectFolder = args[i + 1];
			if(args[i + 1].StartsWith("\"") && !args[i + 1].EndsWith("\"")) {
				string tmp = args[i + 1];
				int j = i + 2;
				while (j < args.Length && !args[j].Contains("\"")) {
					tmp += " " + args[j];
					j++;
				}
				if(j == args.Length) {
					Console.Error.WriteLine("Error, no closing \" found");
					Environment.Exit(1);
				}
				ProjectFolder = tmp.Substring(1, tmp.Length - 2);
			} else {
				ProjectFolder = args[i + 1];
			}
			i++;
		} else {
			Console.Error.WriteLine($"Error, unknown argument {args[i]}");
			Environment.Exit(1);
		}
	}
	}

	public static void Help() {
		Console.WriteLine($"ProjectOverrde: V{VERSION}");
		Console.WriteLine("Usage: ProjectOverrde [Args]");
		Console.WriteLine("Args:");
		Console.WriteLine("-h, --help: Show this help");
		Console.WriteLine("-v, --version: Show version");
		Console.WriteLine("-f, --file [file path]: File to use");
		Console.WriteLine("-p, --project-folder [folder path]: Folder where the project is/will be");
		Console.WriteLine("--debug-mode: Enable debug mode");
	}
}