namespace AosComDevice
{
    public class ComDeviceBaseModel
    {
        public string Serial;
        public string SubSerial;
        public int Firmware;

        public ComDeviceBaseModel()
        {
            Serial = string.Empty;
            SubSerial = string.Empty;
            Firmware = 0;
        }

        public bool HasInfo()
        {
            if(Serial != string.Empty && SubSerial != string.Empty && Firmware != 0)
            {
                return true;
            }
            return false;
        }
    }
}
