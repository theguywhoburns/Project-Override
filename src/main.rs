use regex::Regex as Regex;
use std::{env, fs::File, io::Read, io::Write};

#[derive(Debug)]
struct Args {
    pub help: bool,
    pub debug: bool,
    pub output_dir: Option<String>,
    pub config_file_path: Option<String>,
}

static mut _ARGS: Args = Args {
    help: false,
    debug: false,
    output_dir: None,
    config_file_path: None,
};

impl Args {
    #[allow(dead_code)]
	pub fn parse(args: &[String]) {
        let mut help = false;
        let mut debug = false;
        let mut output_dir = None;
        let mut config_file_path = None;

        let regex = Regex::new(r"^(-{1,2}\w+)(?:=(.*))?$").ok().expect("Regex failed to compile");
        for arg in args {
            if let Some(captures) = regex.captures(arg) {
                let name = &captures[1];
                let value = captures.get(2).map(|m| m.as_str());

                match name.as_ref() {
                    "--help" | "-h" => help = true,
                    "--debug" | "-d" => debug = true,
                    "--output-dir" | "-o" => output_dir = value.map(|s| s.to_string()),
                    "--config-file-path" | "-c" => config_file_path = value.map(|s| s.to_string()),
                    _ => panic!("Unknown argument: {}", name),
                }
            }
        }

        unsafe {
            _ARGS.help = help;
            _ARGS.debug = debug;
            _ARGS.output_dir = output_dir;
            _ARGS.config_file_path = config_file_path;
        }
    }

	#[allow(dead_code)]
	pub fn help() -> bool {
		unsafe { _ARGS.help }
	}

	#[allow(dead_code)]
	pub fn debug() -> bool {
		unsafe { _ARGS.debug }
	}

	#[allow(dead_code)]
	pub fn output_dir() -> Option<String> {
		unsafe { _ARGS.output_dir.clone() }
	}

	#[allow(dead_code)]
	pub fn config_file_path() -> Option<String> {
		unsafe { _ARGS.config_file_path.clone() }
	}
}

#[derive(Debug, PartialEq)]
#[repr(u8)] // this tells the compiler to treat the enum as a number
pub enum Token {
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
	Comment(String),// //test
	Slash,		// /
	BackSlash,	// 
	StartMultiLineComment,// /*
	EndMultiLineComment, // */
	EndOfFile,	//EOF
	Identifier(String), //Everything else
	Number(i64),
	FloatNumber(f64),
	MaxTokens   //NOTE: This is for enum size, it's not a limit to tokens or a token itself
}

pub fn lex(source: &Vec<u8>) -> Vec<Token> {
	const DELIMITERS : &str = "^+-=!@$%&|~@(){}[]'\".,:;* \n\t\r /\\";
	let mut ret: Vec<Token> = Vec::with_capacity(source.len());
	let mut i = 0;
	while i < source.len() {
		
		match source[i] {
			b'^' => ret.push(Token::Power),
			b'+' => ret.push(Token::Plus),
			b'=' => ret.push(Token::Equals),
			b'-' => ret.push(Token::Minus),
			b'!' => ret.push(Token::Not),
			b'@' => ret.push(Token::At),
			b'$' => ret.push(Token::Dollar),
			b'%' => ret.push(Token::Percent),
			b'&' => ret.push(Token::And),
			b'|' => ret.push(Token::Or),
			b'~' => ret.push(Token::Probably),
			b'#' => ret.push(Token::Hashtag),
			b'(' => ret.push(Token::Oparen),
			b')' => ret.push(Token::Cparen),
			b'{' => ret.push(Token::Ocurly),
			b'}' => ret.push(Token::Ccurly),
			b'[' => ret.push(Token::Osquare),
			b']' => ret.push(Token::Csquare),
			b'\'' => ret.push(Token::Quote),
			b'"' => ret.push(Token::DoubleQuote),
			b'.' => ret.push(Token::Dot),
			b',' => ret.push(Token::Comma),
			b':' => ret.push(Token::Colon),
			b';' => ret.push(Token::Semicolon),
			b'*' => {
				if i+1 < source.len() && source[i+1] == b'/' {
					ret.push(Token::EndMultiLineComment);
					i += 1;
				} else {
					ret.push(Token::Star);
				}
			}
			b' ' => ret.push(Token::Whitespace),
			b'\n' => ret.push(Token::Newline),
			b'\r' => {
				if i+1 < source.len() && source[i+1] == b'\n' {
					ret.push(Token::Newline);
					i += 1;
				} else {
					ret.push(Token::Newline);
				}
			}
			b'\t' => ret.push(Token::Tab),
			b'/' => {
				if i+1 < source.len() && source[i+1] == b'*' {
					ret.push(Token::StartMultiLineComment);
					i += 1;
				} else if i+1 < source.len() && source[i+1] == b'/' {
					let mut j = i+2;
					while j < source.len() && source[j] != b'\n' && source[j] != b'\r' {
						j += 1;
					}
					ret.push(Token::Comment(std::str::from_utf8(&source[i+2..j]).unwrap().to_string()));
					i = j;
				} else {
					ret.push(Token::Slash);
				}
			}
			b'\\' => ret.push(Token::BackSlash),
			_ => {
				let mut end = i;
				if source[i].is_ascii_digit() {
					let mut end = i+1;
					while end < source.len() && source[end].is_ascii_digit() {
						end += 1;
					}
					if end < source.len() && !DELIMITERS.contains(source[end] as char) {
						while end < source.len() && !DELIMITERS.contains(source[end] as char) {
							end += 1;
						}
						ret.push(Token::Identifier(std::str::from_utf8(&source[i..end]).unwrap().to_string()));
					} else {
						ret.push(Token::Number(std::str::from_utf8(&source[i..end]).unwrap().parse().unwrap()));
					}
					i = end - 1;
				} else {
					while end < source.len() && !DELIMITERS.contains(source[end] as char) && source[end] != b' ' {
						end += 1;
					}
					ret.push(Token::Identifier(std::str::from_utf8(&source[i..end]).unwrap().to_string()));
					i = end - 1;
				}
			}
		}
		i += 1;
	}
	ret.push(Token::EndOfFile);
	ret
}

fn main() {
    let args: Vec<String> = env::args().collect();
    Args::parse(&args);
	if Args::help() {
		println!("Help");
		return;
	}
	let config_file_path = Args::config_file_path().unwrap_or("ProjectOverride.txt".to_string());
	let mut config_file = match File::open(config_file_path) {
    	Ok(file) => file,
    	Err(error) => panic!("Failed to open config file: {}", error),
	};
	let mut config_content = String::new();
	
	config_file.read_to_string(&mut config_content).expect("Failed to read config file");

	let tokens = lex(&config_content.into_bytes());
	if Args::debug() {
	let mut file = File::create("tokens.txt").unwrap();
		for token in &tokens {
	    	writeln!(file, "{:?}", token).unwrap();
		}
	}
}
