# CommandComplete

![Build Status](https://programmeral.visualstudio.com/_apis/public/build/definitions/06509de1-b684-4544-a7f4-5a0d1a1a4269/28/badge)

CommandComplete is a simple text parser for pulling out a command name, along with parameter names/values.

There is also built-in suppport for using the library within a console application, with the ability to autocomplete command and parameter names when hitting the Tab key.

### Features
- Parses command name with parameters
  - Parameters can be Valued or Flags
  - Valued Parameters are parameters that must be given a value (you know, the most common form of parameter)
  - Flag Parameters are parameters that don't need a value. The presence means something for your app.
- Command text is space-delimited, but wrapping parameter values with double quotes will allow a space to be used. Like normal.
  - Ex: Command1 -Param1 "Some Value"
    - Param1 will have the value of "Some Value" but without the double quotes

### Features in Console
- Hitting Tab key will predict command name or parameter name
- Hitting Escape key deletes current item being entered
  - Ex: With text, "Command1 -Param1 Value -Param2" when hitting Escape, the -Param2 part will be removed

