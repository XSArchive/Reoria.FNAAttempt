using Reoria.Server.Application;
using Reoria.Server.Application.Services;

using var server = new ServerApplication(args).Start<GameService>().Start<GameService>().Start<GameService>();