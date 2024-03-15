using Microsoft.Extensions.Configuration;
using Shared.Configuration.Exceptions;

namespace Shared.Configuration.Extensions {
    public static class GetOrThrowExtension {
        public static string GetOrThrow(this IConfiguration configuration) {
            var value = configuration.Get<string>();
            return value ?? throw new BadCastException();
        }

        public static ValueType GetOrThrow<ValueType>(this IConfiguration configuration) {
            var value = configuration.Get<ValueType>();
            return value ?? throw new BadCastException();
        }
    }
}