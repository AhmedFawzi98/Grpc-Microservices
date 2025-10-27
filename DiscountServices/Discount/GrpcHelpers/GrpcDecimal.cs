namespace Grpc.CustomTypes;

public partial class GrpcDecimal 
{
    private const decimal NanoFactor = 1_000_000_000m;

    public GrpcDecimal(long units, int nanos) 
    {
        Units = units;
        Nanos = nanos;
    }

    public static implicit operator decimal(GrpcDecimal grpcDecimal)
    {
        return grpcDecimal.Units + (grpcDecimal.Nanos / NanoFactor);
    }

    public static implicit operator GrpcDecimal(decimal value)
    {
        var units = decimal.ToInt64(value);
        var nanos = decimal.ToInt32(
                        decimal.Round((value - units) * NanoFactor, MidpointRounding.AwayFromZero)
                    );

        return new GrpcDecimal(units, nanos);
    }
}
