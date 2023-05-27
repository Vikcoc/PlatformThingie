using PlatformInterfaces;

namespace ToBeLoaded;
public class Class1 : IShowMessage
{
    public void ItWorks(){
        Console.WriteLine("Printing from ToBeLoaded.Class1");
    }

    public void ShowMessage()
    {
        Console.WriteLine("Printing from ToBeLoaded.Class1 of the IShowMessage interface");
    }
}
