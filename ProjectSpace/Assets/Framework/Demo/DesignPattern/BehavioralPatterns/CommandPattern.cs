using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Demo.DesignPattern.BehavioralPatterns
{
    /// <summary>
    /// 命令模式(行为型)
    /// </summary>
    public class CommandPattern : MonoBehaviour
    {
        void Start()
        {
            var television = new Television();
            var radio = new Radio();
            var windMatchine = new WindMatchine();
            
            //打开电视命令
            var turnTeleOnCmd = new TurnOn(television);
            var deviceBtn = new DeviceButton(turnTeleOnCmd);
            deviceBtn.Press();

            var turnTeleOffCmd = new TurnOff(television);
            turnTeleOffCmd.Execute();
            turnTeleOffCmd.Undo();
        }
    }

    /// <summary>
    /// 硬件按钮
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

    #region 设备

    /// <summary>
    /// 硬件接口
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// 打开
        /// </summary>
        void On();

        /// <summary>
        /// 关闭
        /// </summary>
        void Off();
    }

    /// <summary>
    /// 音量接口
    /// </summary>
    public interface IVolume
    {
        /// <summary>
        /// 加声音
        /// </summary>
        void VolumeUp();

        /// <summary>
        /// 减低声音
        /// </summary>
        void VolumeDown();
    }

    /// <summary>
    /// 电器设备接口
    /// 具备开关和音量控制
    /// </summary>
    public interface IElectronicDevice : IDevice, IVolume
    {

    }

    /// <summary>
    /// 电视机
    /// </summary>
    public class Television : IElectronicDevice
    {
        public void On()
        {
            Debug.LogError("电视机打开");
        }

        public void Off()
        {
            Debug.LogError("电视机关闭");
        }

        public void VolumeUp()
        {
            Debug.LogError("电视机加声音");
        }

        public void VolumeDown()
        {
            Debug.LogError("电视机减低声音");
        }
    }

    /// <summary>
    /// 收音机
    /// </summary>
    public class Radio : IElectronicDevice
    {
        protected int Volume = 0;

        public void On()
        {
            Debug.Log("收音机打开");
        }

        public void Off()
        {
            Debug.Log("收音机关闭");
        }

        public void VolumeUp()
        {
            ++Volume;
            Debug.Log("收音机音量向上调节至 " + Volume);
        }

        public void VolumeDown()
        {
            if (Volume > 0)
                --Volume;
            Debug.Log("收音机音量向下调节至 " + Volume);
        }
    }

    /// <summary>
    /// 风扇
    /// </summary>
    public class WindMatchine : IDevice
    {
        public void On()
        {
            Debug.LogError("风扇打开");
        }

        public void Off()
        {
            Debug.LogError("风扇关闭");
        }
    }

    #endregion

    #region 命令

    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Execute();

        /// <summary>
        /// 撤销
        /// </summary>
        void Undo();
    }

    /// <summary>
    /// 关闭所有命令
    /// </summary>
    public class TurnItAllOff : ICommand
    {
        /// <summary>
        /// 所有硬件
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