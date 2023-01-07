namespace OpenSeaPlugin
{
    static class Extensions
    {
        public static string SchemaToSymbol(this string input) {
            return input switch {
                "ERC721" => "ETH",
                _ => input,
            };
        }
    }
}