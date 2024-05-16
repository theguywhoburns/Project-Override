# Project -> Override
### An extensive project configuration tool
Welcome to the RainWorld `Project -> Override` tool! This tool is designed specifically to simplify the project setup and configuration process. Define project directives using the guidelines below.

###WARNING: For now this tool only supports RainWorld projects

## Directives Available
- `CONFIG_VERSION(version)`: Set the configuration version.
- `RW_DIR(path)`: Set the RainWorld game directory.
- `INPUT(msg_to_display)`: Create an input box for project creation.
- `DEFAULT(directive_name)`: Set a default value for a directive.
- `BOOL_INPUT(msg_to_display)`: Create a boolean input for directives.
- `MESSAGE(message)`: Display a message in the project console.
- `DIRECTORY(create, path)`: Create a directory within the project.
- `FILE_COPY(copy, path_to_copy_from, path_to_paste)`: Copy a file to a specified location within the project.
- `FILE_CREATE(create, path)`: Create a file within the project.
- `DESCRIPTION(description)`: Provide a description for the project configuration.
- `DEPENDENCYID[id1, id2, ...]`: Set RainWorld project dependency IDs.
- `DEPENDENCYNAME[name1, name2, ...]`: Set RainWorld project dependency names.
- `PRIORITIES[priority1, priority2, ...]`: Set priorities for the RainWorld project.
- `TAGS[tag1, tag2, ...]`: Set tags for the RainWorld project.
- `CHECKSUMOVERRIDE(value)`: Override checksum setting for the RainWorld project.

## Usage
1. Define the configuration version with `CONFIG_VERSION(version)`.
2. Set the RainWorld project directory with `RW_DIR(path)`.
3. Customize directives as needed using `INPUT(msg_to_display)`, `DEFAULT(directive_name)`, and `BOOL_INPUT(msg_to_display)`.
4. Display messages in the RainWorld project console with `MESSAGE(message)`.
5. Create directories with `DIRECTORY(create, path)` and `DIRECTORY(true, "path/subpath")`.
6. Copy files with `FILE_COPY(BOOL_INPUT("Copy file?"), "path_to_copy_from", "path_to_paste")`.
7. Create files using `FILE_CREATE(true, "data")`.
8. Provide a description of the configuration with `DESCRIPTION(description)`.
9. Manage dependencies, priorities, tags, and checksum settings as needed.

For detailed error handling and additional command options, refer to the documentation or use the `--help-ex` command to display error codes and explanations.

Enjoy configuring your projects effortlessly with the `Project -> Override` Project Configurator!