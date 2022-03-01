using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Demo.DesignPattern.BehavioralPatterns
{
    /// <summary>
    /// 责任链模式(行为型)
    /// </summary>
    public class ChainOfResponsibility : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var stateBoss = new StateBoss();
            var vicePresident = new VicePresident();
            var president = new President();

            stateBoss.SetSuccessor(vicePresident);
            president.SetSuccessor(vicePresident);

            var contract = new Contract("铁路项目", "贯穿纽约到芝加哥", 90000000);
            stateBoss.HandleRequest(contract);
        }
    }

    /// <summary>
    /// 请求处理者抽象类
    /// </summary>
    public abstract class RequestHandler
    {
        /// <summary>
        /// 继任者
        /// </summary>
        protected RequestHandler Successor;

        /// <summary>
        /// 继任者
        /// 这里也可以使用构造去传递赋值，视情况而定
        /// </summary>
        /// <param name="successor"></param>
        public void SetSuccessor(RequestHandler successor)
        {
            Successor = successor;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="request"></param>
        public abstract void HandleRequest(Contract request);
    }

    /// <summary>
    /// 州长
    /// </summary>
    class StateBoss : RequestHandler
    {
        public override void HandleRequest(Contract request)
        {
            if (request.NeedMoney <= 100000000)
            {
                Debug.LogError($"{GetType().Name}签订{request.ContractName}合同:{request.ContractDesc}");
            }
            else
            {
                Successor?.HandleRequest(request);
            }
        }
    }

    /// <summary>
    /// 副总统
    /// </summary>
    class VicePresident : RequestHandler
    {
        public override void HandleRequest(Contract request)
        {
            if (request.NeedMoney <= 200000000)
            {
                Debug.LogError($"{GetType().Name}签订{request.ContractName}合同:{request.ContractDesc}");
            }
            else
            {
                Successor?.HandleRequest(request);
            }
        }
    }

    /// <summary>
    /// 总统
    /// </summary>
    class President : RequestHandler
    {
        public override void HandleRequest(Contract request)
        {
            Debug.LogError($"{GetType().Name}签订{request.ContractName}合同:{request.ContractDesc}");
        }
    }

    public class Contract
    {
        /// <summary>
        /// 合同名
        /// </summary>
        public readonly string ContractName;

        /// <summary>
        /// 合同描述
        /// </summary>
        public readonly string ContractDesc;

        /// <summary>
        /// 需要金钱
        /// </summary>
        public readonly long NeedMoney;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="money"></param>
        public Contract(string name, string desc, long money)
        {
            ContractName = name;
            ContractDesc = desc;
            NeedMoney = money;
        }
    }
}