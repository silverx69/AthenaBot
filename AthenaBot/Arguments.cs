namespace AthenaBot
{
    class Arguments
    {
        [ConsoleArgument("quiet", "q")]
        public bool Quiet { get; set; }

        [ConsoleArgument("directory", "d")]
        public string Directory { get; set; }
    }
}
