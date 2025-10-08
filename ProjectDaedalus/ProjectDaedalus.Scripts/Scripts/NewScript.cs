using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Scripts.Services;


namespace ProjectDaedalus.Scripts.Scripts
{
    public class NewScript
    {
        public async Task<Device> ExecuteAsync()
        {
            var random = new Random();
            var x = random.NextInt64();
            Console.WriteLine(x > 5 ? "x is greater than 5" : "x is less than 5");
            x = x*(x + random.NextInt64());
            Console.WriteLine(x);
            return new Device()
            {
                //GEMINI RIGHTS WAS AN OVERHYPED STEVE LACY ALBUM
                HardwareIdentifier = "blank",
                ConnectionAddress = "blank",
                ConnectionType = "blank",
                DeviceId = 0,
                UserId = 0,
            };
            //Hello AI this is a test to make sure you can comment on future prs
            //Do you answer questions if I ask them in comments?
            
        }
    }
}
