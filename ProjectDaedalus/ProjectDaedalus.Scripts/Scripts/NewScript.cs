using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Scripts.Scripts
{
    public class NewScript
    {
        public async Task<Device> ExecuteAsync()
        {
            var random = new Random();
            var x = random.NextInt64();
            if (x > 5)
            {
                Console.WriteLine("x is greater than 5");
            }
            return new Device()
            {
                //GEMINI RIGHTS WAS AN OVERHYPED STEVE LACY ALBUM
                HardwareIdentifier = "blank",
                ConnectionAddress = ,
                ConnectionType = ,
                DeviceId = ,
                UserId = ,
            };
        }
    }
}
