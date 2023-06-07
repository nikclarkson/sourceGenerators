using System;
using System.Collections.Generic;
using System.Linq;

namespace NorthDallas.Generators
{
    public class StringClassModel : IEquatable<StringClassModel?>
    {
        public StringClassModel(string namespaceName, string className, IEnumerable<string> propertyNames)
        {
            NamespaceName = namespaceName;
            ClassName = className;
            PropertyNames = propertyNames;
        }

        public string NamespaceName { get; }
        public string ClassName { get; }
        public IEnumerable<string> PropertyNames { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as StringClassModel);
        }

        public bool Equals(StringClassModel? model)
        {
            return model is not null &&
                   NamespaceName == model.NamespaceName &&
                   ClassName == model.ClassName &&
                   PropertyNames.SequenceEqual(model.PropertyNames);
        }

        public override int GetHashCode()
        {
            int hashCode = 41921911;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamespaceName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClassName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<string>>.Default.GetHashCode(PropertyNames);
            return hashCode;
        }

    }
}

