# REPLPlugin

![REPL preview](https://raw.githubusercontent.com/denikson/REPLPlugin/master/img/example.png)

A basic REPL (read-evaluate-print loop) for Unity games. Uses BepInEx to sideload itself into the game.

## Features

* C# 7 support via a custom Mono Compiler Service
* Code completion suggestions
* Movable, resizable window

## Installation

First, install [BepInEx](https://github.com/BepInEx/BepInEx). Then download the latest version of this plug-in and extract the contents of the archive into the `BepInEx` folder located your game folder.

To use the plugin, open the game and press <kbd>Insert</kbd> when the game has finished loading. You can change the key in BepInEx' configuration file.

## Key commands

* While the game is open
    * <kbd>Insert</kbd> to display or hide the REPL
* When focused on the command input
    * <kbd>Enter</kbd> execute the command
    * <kbd>Up</kbd> or <kbd>Down</kbd> Paste previously executed commands