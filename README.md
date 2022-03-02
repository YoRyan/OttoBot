# Otto the F# Bot

This is a personal Discord bot of mine written using .NET 6, F#, C#, and [Discord.Net](https://github.com/discord-net/Discord.Net). Although it is not a public bot, I have open-sourced it so that it might help other programmers write their own F# bots.

C# sets up Discord.Net and the bot's modules, while F# performs the heavy lifting of executing commands. It's possible to write all of the classes in pure F#, but there is so much friction between the C# OOP/Task domain and the F# functional/Async domain that it's just not a practical way to program.

![](https://upload.wikimedia.org/wikipedia/en/d/da/Airplane_screenshot_Haggerty_Nielsen.jpg)

To run Otto, supply the following environment variables:

- `DC_token`: the Discord API token (mandatory)
- `DC_testguild`: if running with a debug executable, the bot will register its commands with this guild instead of globally
- `DC_clearcommands`: will clear the registered commands either globally or, with a debug executable, from the guild passed to `DC_testguild`

### License

Copyright 2021 [Ryan Young](https://youngryan.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
