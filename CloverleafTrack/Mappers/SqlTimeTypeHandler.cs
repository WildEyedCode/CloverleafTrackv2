using System.Data;
using CloverleafTrack.Models;
using Dapper;

namespace CloverleafTrack.Mappers;

public class SqlTimeTypeHandler : SqlMapper.TypeHandler<Time>
{
    public override void SetValue(IDbDataParameter parameter, Time time)
    {
        parameter.Value = time.ToString();
    }

    public override Time Parse(object value)
    {
        var str = value.ToString().Split(":");
        return new Time(double.Parse(str[0]), double.Parse(str[1]));
    }
}