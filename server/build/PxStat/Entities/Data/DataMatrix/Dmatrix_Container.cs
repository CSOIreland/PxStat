using Autofac;

namespace PxStat.Data
{

    public static class Dmatrix_Container
    {
        public static IContainer Configure()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<Dmatrix>().As<IDmatrix>();
            return builder.Build();
        }
    }
}
