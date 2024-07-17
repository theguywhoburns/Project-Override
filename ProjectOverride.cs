static class ProjectOverride {
	public static List<Error> errors = new List<Error>();
	public static void PrintErrors() {
		foreach(Error error in errors) {
			Console.Error.WriteLine(error.message + " at " + error.line + ":" + error.col);
		}
	}
}