using System.Data;
using CloverleafTrack.Models;
using Dapper;

namespace CloverleafTrack.Mappers;

public class SqlDistanceTypeHandler : SqlMapper.TypeHandler<Distance>
{
    public override void SetValue(IDbDataParameter parameter, Distance time)
    {
        parameter.Value = time.ToString();
    }

    public override Distance Parse(object value)
    {
        var str = value.ToString().Split("-");
        return new Distance(double.Parse(str[0]), double.Parse(str[1]));
    }
}