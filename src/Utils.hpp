#pragma once
#include <iostream>
#include <fstream>

// Currently unused
#include <sstream>
#include <chrono>
#include <filesystem>
#include <thread> 
//

#include <string>
#include <vector>
#include <any>

#include <limits>
#include <unordered_map>
#include <functional>
#include <typeindex>
#include <type_traits>
#include <concepts>
#include <numeric>
// This one is for reduced memory usage
#include <string_view>

// basically useless but i'll keep it just in case
#include <cassert>
#include <cstdint>
// most of the starting code is the same as in PROJECT_OVERRIDE
namespace Utils { 
	// Custom character functions, fuck off if you don't like them
  inline bool IsSpace (char c) { return c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\f'; }
  inline bool IsNumber(char c) { return c >= '0' && c <= '9'; }
  inline bool IsLetter(char c) { return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'); }
  inline bool IsAlphaNumeric(char c){ return IsLetter(c) || IsNumber(c); }
  inline bool IsNewLine(char c)     { return c == '\n'; }
  inline bool IsSpecialChar(char c) { return std::strchr("!\"$%&'*+,-./:;=?@\\^|~", c) != nullptr; } 
  template<typename T>
  concept NumericBooleanOrString = std::integral<T> || std::floating_point<T> || std::same_as<T, std::string> || std::same_as<T, bool>;
  #define MACRO_STR_(x) #x
  #define MACRO_UNWRAP(x) x
  #define MACRO_STR(x) MACRO_UNWRAP(MACRO_STR_(x))
  #define PROJECT_OVERRIDE_VERSION_STR MACRO_STR(PROJECT_OVERRIDE_VERSION_MAJOR) "." MACRO_STR(PROJECT_OVERRIDE_VERSION_MINOR) "." MACRO_STR(PROJECT_OVERRIDE_VERSION_PATCH)
  namespace Ascii {
    ///@author https://gist.github.com/RabaDabaDoba/145049536f815903c79944599c6f952a
    
    //Regular text
    const inline std::string BLACK   = "\e[0;30m";
    const inline std::string RED     = "\e[0;31m";
    const inline std::string GREEN   = "\e[0;32m";
    const inline std::string YELLOW  = "\e[0;33m";
    const inline std::string BLUE    = "\e[0;34m";
    const inline std::string MAGENTA = "\e[0;35m";
    const inline std::string CYAN    = "\e[0;36m";
    const inline std::string WHITE   = "\e[0;37m";
  
    //Regular bold text
    const inline std::string BOLD_BLACK   = "\e[1;30m";
    const inline std::string BOLD_RED     = "\e[1;31m";
    const inline std::string BOLD_GREEN   = "\e[1;32m";
    const inline std::string BOLD_YELLOW  = "\e[1;33m";
    const inline std::string BOLD_BLUE    = "\e[1;34m";
    const inline std::string BOLD_MAGENTA = "\e[1;35m";
    const inline std::string BOLD_CYAN    = "\e[1;36m";
    const inline std::string BOLD_WHITE   = "\e[1;37m";
  
    //Regular underline text
    const inline std::string UNDERLINE_BLACK   = "\e[4;30m";
    const inline std::string UNDERLINE_RED     = "\e[4;31m";
    const inline std::string UNDERLINE_GREEN   = "\e[4;32m";
    const inline std::string UNDERLINE_YELLOW  = "\e[4;33m";
    const inline std::string UNDERLINE_BLUE    = "\e[4;34m";
    const inline std::string UNDERLINE_MAGENTA = "\e[4;35m";
    const inline std::string UNDERLINE_CYAN    = "\e[4;36m";
    const inline std::string UNDERLINE_WHITE   = "\e[4;37m";
  
    //Regular background
    const inline std::string BLACK_BACK   = "\e[40m";
    const inline std::string RED_BACK     = "\e[41m";
    const inline std::string GREEN_BACK   = "\e[42m";
    const inline std::string YELLOW_BACK  = "\e[43m";
    const inline std::string BLUE_BACK    = "\e[44m";
    const inline std::string MAGENTA_BACK = "\e[45m";
    const inline std::string CYAN_BACK    = "\e[46m";
    const inline std::string WHITE_BACK   = "\e[47m";
  
    //High intensty background 
    const inline std::string HIGH_INTENSITY_BLACK_BACK   = "\e[0;100m";
    const inline std::string HIGH_INTENSITY_RED_BACK     = "\e[0;101m";
    const inline std::string HIGH_INTENSITY_GREEN_BACK   = "\e[0;102m";
    const inline std::string HIGH_INTENSITY_YELLOW_BACK  = "\e[0;103m";
    const inline std::string HIGH_INTENSITY_BLUE_BACK    = "\e[0;104m";
    const inline std::string HIGH_INTENSITY_MAGENTA_BACK = "\e[0;105m";
    const inline std::string HIGH_INTENSITY_CYAN_BACK    = "\e[0;106m";
    const inline std::string HIGH_INTENSITY_WHITE_BACK   = "\e[0;107m";
  
    //High intensty text
    const inline std::string HIGH_INTENSITY_BLACK   = "\e[0;90m";
    const inline std::string HIGH_INTENSITY_RED     = "\e[0;91m";
    const inline std::string HIGH_INTENSITY_GREEN   = "\e[0;92m";
    const inline std::string HIGH_INTENSITY_YELLOW  = "\e[0;93m";
    const inline std::string HIGH_INTENSITY_BLUE    = "\e[0;94m";
    const inline std::string HIGH_INTENSITY_MAGENTA = "\e[0;95m";
    const inline std::string HIGH_INTENSITY_CYAN    = "\e[0;96m";
    const inline std::string HIGH_INTENSITY_WHITE   = "\e[0;97m";
  
    //Bold high intensity text
    const inline std::string BOLD_HIGH_INTENSITY_BLACK   = "\e[1;90m";
    const inline std::string BOLD_HIGH_INTENSITY_RED     = "\e[1;91m";
    const inline std::string BOLD_HIGH_INTENSITY_GREEN   = "\e[1;92m";
    const inline std::string BOLD_HIGH_INTENSITY_YELLOW  = "\e[1;93m";
    const inline std::string BOLD_HIGH_INTENSITY_BLUE    = "\e[1;94m";
    const inline std::string BOLD_HIGH_INTENSITY_MAGENTA = "\e[1;95m";
    const inline std::string BOLD_HIGH_INTENSITY_CYAN    = "\e[1;96m";
    const inline std::string BOLD_HIGH_INTENSITY_WHITE   = "\e[1;97m";
  
    //Reset
    const inline std::string RESET = "\e[0m";
  };
};

#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
  #define OS_WINDOWS 1
#elif defined(unix) || defined(__unix__) || defined(__unix)
  #define OS_UNIX 1
#endif