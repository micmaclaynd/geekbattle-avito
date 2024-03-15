using Microsoft.Extensions.Configuration;
using Shared.Configuration.Exceptions;

namespace Shared.Configuration.Extensions {
    public static class GetSectionOrThrowExtension {
        public static ValueType GetSectionOrThrow<ValueType>(this IConfiguration configuration, string key) {
            var value = configuration.GetSection(key);
            if (value == null) throw new MissFieldException(key);
            return value.GetOrThrow<ValueType>();
        }
    }
}