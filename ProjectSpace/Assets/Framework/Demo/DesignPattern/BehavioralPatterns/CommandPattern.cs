using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Demo.DesignPattern.BehavioralPatterns
{
    /// <summary>
    /// ����ģʽ(��Ϊ��)
    /// </summary>
    public class CommandPattern : MonoBehaviour
    {
        void Start()
        {
            var television = new Television();
            var radio = new Radio();
            var windMatchine = new WindMatchine();
            
            //�򿪵�������
            var turnTeleOnCmd = new TurnOn(television);
            var deviceBtn = new DeviceButton(turnTeleOnCmd);
            deviceBtn.Press();

            var turnTeleOffCmd = new TurnOff(television);
            turnTeleOffCmd.Execute();
            turnTeleOffCmd.Undo();
        }
    }

    /// <summary>
    /// Ӳ����ť
    /// </summary>
    public class DeviceButton
    {
        private readonly ICommand _cmd;

        public DeviceButton(ICommand cmd)
        {
            _cmd = cmd;
        }

        public void Press()
        {
            // actually the invoker (device button) has no idea what it does
            _cmd.Execute();
        }

        public void PressUndo()
        {
            _cmd.Undo();
        }
    }

    #region �豸

    /// <summary>
    /// Ӳ���ӿ�
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// ��
        /// </summary>
        void On();

        /// <summary>
        /// �ر�
        /// </summary>
        void Off();
    }

    /// <summary>
    /// �����ӿ�
    /// </summary>
    public interface IVolume
    {
        /// <summary>
        /// ������
        /// </summary>
        void VolumeUp();

        /// <summary>
        /// ��������
        /// </summary>
        void VolumeDown();
    }

    /// <summary>
    /// �����豸�ӿ�
    /// �߱����غ���������
    /// </summary>
    public interface IElectronicDevice : IDevice, IVolume
    {

    }

    /// <summary>
    /// ���ӻ�
    /// </summary>
    public class Television : IElectronicDevice
    {
        public void On()
        {
            Debug.LogError("���ӻ���");
        }

        public void Off()
        {
            Debug.LogError("���ӻ��ر�");
        }

        public void VolumeUp()
        {
            Debug.LogError("���ӻ�������");
        }

        public void VolumeDown()
        {
            Debug.LogError("���ӻ���������");
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    public class Radio : IElectronicDevice
    {
        protected int Volume = 0;

        public void On()
        {
            Debug.Log("��������");
        }

        public void Off()
        {
            Debug.Log("�������ر�");
        }

        public void VolumeUp()
        {
            ++Volume;
            Debug.Log("�������������ϵ����� " + Volume);
        }

        public void VolumeDown()
        {
            if (Volume > 0)
                --Volume;
            Debug.Log("�������������µ����� " + Volume);
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public class WindMatchine : IDevice
    {
        public void On()
        {
            Debug.LogError("���ȴ�");
        }

        public void Off()
        {
            Debug.LogError("���ȹر�");
        }
    }

    #endregion

    #region ����

    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// ִ��
        /// </summary>
        void Execute();

        /// <summary>
        /// ����
        /// </summary>
        void Undo();
    }

    /// <summary>
    /// �ر���������
    /// </summary>
    public class TurnItAllOff : ICommand
    {
        /// <summary>
        /// ����Ӳ��
        /// </summary>
        private readonly List<IDevice> _allDevices;

        public TurnItAllOff(List<IDevice> devices)
        {
            _allDevices = devices;
        }

        public void Execute()
        {
            foreach (IDevice device in _allDevices)
            {
                device.Off();
            }
        }

        public void Undo()
        {
            foreach (IDevice device in _allDevices)
            {
                device.On();
            }
        }
    }

    public class TurnOn : ICommand
    {
        private readonly IDevice _device;
        public TurnOn(IDevice device)
        {
            _device = device;
        }

        public void Execute()
        {
            _device.On();
        }

        public void Undo()
        {
            _device.Off();
        }
    }

    public class TurnOff : ICommand
    {
        private readonly IDevice _device;
        public TurnOff(IDevice device)
        {
            _device = device;
        }

        public void Execute()
        {
            _device.Off();
        }

        public void Undo()
        {
            _device.On();
        }
    }

    public class TurnVolumeUp : ICommand
    {
        private readonly IVolume _device;

        public TurnVolumeUp(IVolume device)
        {
            _device = device;
        }

        public void Execute()
        {
            _device.VolumeUp();
        }

        public void Undo()
        {
            _device.VolumeDown();
        }
    }

    public class TurnVolumeDown : ICommand
    {
        private readonly IVolume _device;

        public TurnVolumeDown(IVolume device)
        {
            _device = device;
        }

        public void Execute()
        {
            _device.VolumeDown();
        }

        public void Undo()
        {
            _device.VolumeUp();
        }
    }

    #endregion
}