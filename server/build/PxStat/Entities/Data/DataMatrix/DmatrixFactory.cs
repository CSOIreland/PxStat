using API;
using Autofac;

namespace PxStat.Data
{

    public class DmatrixFactory
    {
        internal IContainer Container { get; set; }
        internal IADO ado { get; set; }

        public DmatrixFactory(IADO ado)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<Dmatrix>().As<IDmatrix>();
            Container = builder.Build();
            this.ado = ado;
        }

        public IDmatrix CreateDmatrix(IUpload_DTO dto)
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                IDmatrix dmatrix = scope.Resolve<IDmatrix>();

                // Get PxDocument from dto
                var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
                var pxDocument = pxManualParser.Parse();

                // Load properties from PxDocument
                dmatrix = new Dmatrix(pxDocument, dto, ado);
                dmatrix.MtrInput = dto.MtrInput;
                return dmatrix;
            }
        }

        public DmatrixFactory()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<Dmatrix>().As<IDmatrix>();
            Container = builder.Build();
        }

        public IDmatrix CreateDmatrix()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                IDmatrix dmatrix = scope.Resolve<IDmatrix>();


                // Load properties from PxDocument
                dmatrix = new Dmatrix();
                return dmatrix;
            }
        }
    }
}
