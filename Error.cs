class Error {
	public String message;
	public int line;
	public int col;
	public Error(String message, int line, int col) {
		this.message = message;
		this.line = line;
		this.col = col;
	}
}