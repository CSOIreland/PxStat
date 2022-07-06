using Autofac;

namespace PxStat.Data
{

    public class DSpecFactory
    {
        public IContainer Container { get; set; }

        public DSpecFactory()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<Dspec>().As<IDspec>();
            Container = builder.Build();

        }

        public IDspec CreateDspec()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                IDspec dspec = scope.Resolve<IDspec>();


                return dspec;
            }
        }
    }

}
