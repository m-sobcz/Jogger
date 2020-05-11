namespace Jogger.Drivers
{
    public interface IDriver
    {
        void Close();
        bool Initialize();
        string Receive();
        string Send();
        void SetSendData(byte[] data, byte id, byte dataLengthCode, ulong accessMask);
        bool WakeUp();
    }
}