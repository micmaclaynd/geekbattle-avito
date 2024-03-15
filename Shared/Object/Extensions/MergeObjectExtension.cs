namespace Shared.Object.Extensions {
    public static class MergeObjectExtension {
        public static FirstType Merge<FirstType, SecondType>(this FirstType source, SecondType target) {
            if (source == null || target == null) return source;

            foreach (var sourceProperty in source.GetType().GetProperties()) {
                try {
                    var targetProperty = target.GetType().GetProperty(sourceProperty.Name);

                    if (targetProperty != null && targetProperty.PropertyType.IsPublic) {
                        var targetValue = targetProperty.GetValue(target);
                        var sourceValue = sourceProperty.GetValue(source);

                        if (sourceProperty.PropertyType == typeof(string) || sourceProperty.PropertyType.IsPrimitive) {
                            sourceProperty.SetValue(source, targetValue);
                        } else {
                            if (sourceValue == null) {
                                sourceProperty.SetValue(source, Activator.CreateInstance(sourceProperty.PropertyType));
                                sourceValue = sourceProperty.GetValue(source);
                            }
                            sourceValue.Merge(targetValue);
                        }
                    }
                } catch { continue; }
            }

            return source;
        }

        public static SecondType ReverseMerge<FirstType, SecondType>(this FirstType source, SecondType target) {
            return target.Merge(source);
        }
    }
}