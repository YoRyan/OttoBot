# Otto Bot

This is a personal Discord bot of mine written using F# and Discord.Net. Although it is not a public bot, I have open-sourced it so that it might help other programmers write their own F# bots.

The bot is structured similarly to a normal Discord.Net bot, just with F# instead of C#. The most difficult part was translating between C# Task and F# Async. You can peruse all of the gory details in the source code...

![](https://upload.wikimedia.org/wikipedia/en/d/da/Airplane_screenshot_Haggerty_Nielsen.jpg)

To run Otto, supply the bot's API token in the `DISCORD_TOKEN` environment variable.

### License

Copyright 2021 Ryan Young

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
