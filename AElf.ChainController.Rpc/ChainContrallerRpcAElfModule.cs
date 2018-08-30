﻿using AElf.Common.Module;
using AElf.Configuration.Config.RPC;
using Autofac;

namespace AElf.ChainController.Rpc
{
    public class ChainContrallerRpcAElfModule:IAElfModlule
    {
        public void Init(ContainerBuilder builder)
        {
            builder.RegisterModule(new ChainControllerRpcAutofacModule());
        }

        public void Run(ILifetimeScope scope)
        {

        }
    }
}