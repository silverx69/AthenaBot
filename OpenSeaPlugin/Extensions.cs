namespace OpenSeaPlugin
{
    static class Extensions
    {
        public static string SchemaToSymbol(this string input) {
            switch (input) {
                case "ERC721": return "ETH";
            }
            return input;
        }
    }
}