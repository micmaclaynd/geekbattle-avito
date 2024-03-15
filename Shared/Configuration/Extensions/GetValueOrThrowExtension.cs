using Microsoft.Extensions.Configuration;
using Shared.Configuration.Exceptions;

namespace Shared.Configuration.Extensions {
    public static class GetValueOrThrowExtension {
        public static string GetOrThrow(this IConfiguration configuration, string key) {
            var value = configuration.GetValue<string>(key);
            return value ?? throw new MissFieldException(key);
        }

        public static ValueType GetOrThrow<ValueType>(this IConfiguration configuration, string key) {
            var value = configuration.GetValue<ValueType>(key);
            return value ?? throw new MissFieldException(key);
        }
    }
}