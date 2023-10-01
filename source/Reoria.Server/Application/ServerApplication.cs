using Microsoft.Extensions.Configuration;
using Reoria.Application;

namespace Reoria.Server.Application;

public class ServerApplication : ApplicationCore
{
    public ServerApplication(string[] args) : base(args)
    {
    }

    protected override IConfigurationBuilder CreateConfiguration()
    {
        return base.CreateConfiguration()
            .AddJsonFile("appsettings.server.json", optional: true, reloadOnChange: true);
    }
}
