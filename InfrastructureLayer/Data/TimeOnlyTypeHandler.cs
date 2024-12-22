using Dapper;
using System.Data;

namespace INFL.Data
{
    public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.Value = value.ToTimeSpan(); // تحويل إلى TimeSpan
            parameter.DbType = DbType.Time; // تحديد نوع DB
        }

        public override TimeOnly Parse(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return TimeOnly.FromTimeSpan(timeSpan);
            }

            throw new DataException($"Cannot convert {value.GetType()} to TimeOnly");
        }
    }
}
