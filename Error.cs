class Error {
	public string message;
	public int line;
	public int col;
	public Error(string message, int line, int col) {
		this.message = message;
		this.line = line;
		this.col = col;
	}
}