using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Demo.DesignPattern.BehavioralPatterns
{
    /// <summary>
    /// ������ģʽ(��Ϊ��)
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

            var contract = new Contract("��·��Ŀ", "�ᴩŦԼ��֥�Ӹ�", 90000000);
            stateBoss.HandleRequest(contract);
        }
    }

    /// <summary>
    /// �������߳�����
    /// </summary>
    public abstract class RequestHandler
    {
        /// <summary>
        /// ������
        /// </summary>
        protected RequestHandler Successor;

        /// <summary>
        /// ������
        /// ����Ҳ����ʹ�ù���ȥ���ݸ�ֵ�����������
        /// </summary>
        /// <param name="successor"></param>
        public void SetSuccessor(RequestHandler successor)
        {
            Successor = successor;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="request"></param>
        public abstract void HandleRequest(Contract request);
    }

    /// <summary>
    /// �ݳ�
    /// </summary>
    class StateBoss : RequestHandler
    {
        public override void HandleRequest(Contract request)
        {
            if (request.NeedMoney <= 100000000)
            {
                Debug.LogError($"{GetType().Name}ǩ��{request.ContractName}��ͬ:{request.ContractDesc}");
            }
            else
            {
                Successor?.HandleRequest(request);
            }
        }
    }

    /// <summary>
    /// ����ͳ
    /// </summary>
    class VicePresident : RequestHandler
    {
        public override void HandleRequest(Contract request)
        {
            if (request.NeedMoney <= 200000000)
            {
                Debug.LogError($"{GetType().Name}ǩ��{request.ContractName}��ͬ:{request.ContractDesc}");
            }
            else
            {
                Successor?.HandleRequest(request);
            }
        }
    }

    /// <summary>
    /// ��ͳ
    /// </summary>
    class President : RequestHandler
    {
        public override void HandleRequest(Contract request)
        {
            Debug.LogError($"{GetType().Name}ǩ��{request.ContractName}��ͬ:{request.ContractDesc}");
        }
    }

    public class Contract
    {
        /// <summary>
        /// ��ͬ��
        /// </summary>
        public readonly string ContractName;

        /// <summary>
        /// ��ͬ����
        /// </summary>
        public readonly string ContractDesc;

        /// <summary>
        /// ��Ҫ��Ǯ
        /// </summary>
        public readonly long NeedMoney;

        /// <summary>
        /// ���캯��
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