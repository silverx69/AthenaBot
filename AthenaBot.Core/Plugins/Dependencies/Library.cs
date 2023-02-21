namespace AthenaBot.Plugins.Dependencies
{
    public class Library
    {
        public string Type { get; set; }

        public bool Serviceable { get; set; }

        public string Sha512 { get; set; }

        public string Path { get; set; }

        public string HashPath { get; set; }
    }
}
