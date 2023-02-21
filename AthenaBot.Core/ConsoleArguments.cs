using System.Reflection;

namespace AthenaBot
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConsoleArgumentAttribute : Attribute
    {
        public string Name {
            get;
            private set;
        }

        public List<string> Aliases {
            get;
            private set;
        }

        public ConsoleArgumentAttribute() {
            Aliases = new List<string>();
        }

        public ConsoleArgumentAttribute(PropertyInfo property)
            : this() {
            Name = property.Name.ToLower();
        }

        public ConsoleArgumentAttribute(string name, IEnumerable<string> aliases)
            : this() {
            Name = name;
            Aliases.AddRange(aliases);
        }

        public ConsoleArgumentAttribute(string name, params string[] aliases) {
            Name = name;
            Aliases = aliases.ToList();
        }
    }

    public static class ConsoleArguments
    {
        const BindingFlags reflectFlags =
            BindingFlags.Public |
            BindingFlags.Instance;

        public static T Parse<T>(string[] args) where T : new() {
            if (args == null || args.Length == 0)
                return default;

            T result = new T();

            int i = 0;
            var properties = GetPropertyInfo<T>();

            while (i < args.Length) {
                string arg = args[i].ToLower();
                foreach (var pair in properties) {
                    if (arg == $"--{pair.Value.Name}" || pair.Value.Aliases.Contains(a => arg == $"-{a}")) {
                        object value = null;
                        bool success = true;

                        var typeCode = Type.GetTypeCode(pair.Key.PropertyType);
                        if (typeCode == TypeCode.Boolean)
                            value = true;
                        else
                            value = TryConvertArgument(typeCode, args[++i], out success);

                        if (success)
                            pair.Key.SetValue(result, value);

                        break;
                    }
                }
                i++;
            }

            return result;
        }

        private static Dictionary<PropertyInfo, ConsoleArgumentAttribute> GetPropertyInfo<T>() where T : new() {
            var ret = new Dictionary<PropertyInfo, ConsoleArgumentAttribute>();
            var properties = typeof(T).GetProperties(reflectFlags);

            foreach (var property in properties) {
                ret.Add(
                    property,
                    property.GetCustomAttribute<ConsoleArgumentAttribute>() ?? new ConsoleArgumentAttribute(property));
            }
            return ret;
        }

        private static object TryConvertArgument(TypeCode propertyType, string argument, out bool success) {
            success = false;
            object value = null;
            switch (propertyType) {
                case TypeCode.String: {
                    success = true;
                    value = argument;
                    break;
                }
                case TypeCode.Char: {
                    success = char.TryParse(argument, out char tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.SByte: {
                    success = sbyte.TryParse(argument, out sbyte tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Byte: {
                    success = byte.TryParse(argument, out byte tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Int16: {
                    success = short.TryParse(argument, out short tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.UInt16: {
                    success = ushort.TryParse(argument, out ushort tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Single: {
                    success = float.TryParse(argument, out float tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Int32: {
                    success = int.TryParse(argument, out int tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.UInt32: {
                    success = uint.TryParse(argument, out uint tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Double: {
                    success = double.TryParse(argument, out double tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Int64: {
                    success = long.TryParse(argument, out long tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.UInt64: {
                    success = ulong.TryParse(argument, out ulong tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.Decimal: {
                    success = decimal.TryParse(argument, out decimal tmp);
                    value = tmp;
                    break;
                }
                case TypeCode.DateTime: {
                    success = DateTime.TryParse(argument, out DateTime tmp);
                    value = tmp;
                    break;
                }
            }

            return value;
        }
    }
}
