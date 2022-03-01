using ILRuntime.Runtime.Enviorment;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using System;

namespace Framework.Runtime
{
    /// <summary>
    /// 初始化CLR绑定，委托跨域注册
    /// </summary>
    public interface IBinding
    {
        Action<AppDomain> InitializeCLRBingding();

        Action<AppDomain> InitializeCLRManualBingding();

        Action<AppDomain> DelegateRegister();

        Action<AppDomain> RegisterCrossBindingAdaptor();
    }
}