using System.Collections.Generic;

namespace model.cql.hl7.org
{
    public interface IInstantiationContext
    {
        bool IsInstantiable(TypeParameter parameter, DataType callType);
        DataType Instantiate(TypeParameter parameter);
        IEnumerable<SimpleType> GetSimpleConversionTargets(DataType callType);
        IEnumerable<IntervalType> GetIntervalConversionTargets(DataType callType);
        IEnumerable<ListType> GetListConversionTargets(DataType callType);
    }
}