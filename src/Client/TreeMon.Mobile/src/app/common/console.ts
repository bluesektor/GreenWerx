export class ConsoleColors {

    public static Reset = '\x1b[0m';
    public static Bright = '\x1b[1m';
    public static Dim = '\x1b[2m';
    public static Underscore = '\x1b[4m';
    public static Blink = '\x1b[5m';
    public static Reverse = '\x1b[7m';
    public static Hidden = '\x1b[8m';

    public static FgBlack = '\x1b[30m';
    public static FgRed = '\x1b[31m';
    public static FgGreen = '\x1b[32m';
    public static FgYellow = '\x1b[33m';
    public static FgBlue = '\x1b[34m';
    public static FgMagenta = '\x1b[35m';
    public static FgCyan = '\x1b[36m';
    public static FgWhite = '\x1b[37m';

    public static BgBlack = '\x1b[40m';
    public static BgRed = '\x1b[41m';
    public static BgGreen = '\x1b[42m';
    public static BgYellow = '\x1b[43m';
    public static BgMagenta = '\x1b[45m';
    public static BgCyan = '\x1b[46m';
    public static BgWhite = '\x1b[47m';
    public static BgBlue() { return '\x1b[44m'; }
}
