using Jogger.Services;

namespace Jogger.Valves
{
    public interface IValveManager
    {
        ActionStatus Initialize(int channelsCount);
        void SetReceiverType(string valveType);
        void SetSensorsState(byte[] ioResult);
        void SetSequencerType(string valveType);
    }
}