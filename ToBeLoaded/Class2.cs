using PlatformInterfaces;

namespace ToBeLoaded;
public class Class2 : IPlatformAbstractStatic
{
    public static void ShowStaticMessage()
    {
        Console.WriteLine("Printing from ToBeLoaded.Class2 of the IShowMessage interface static method.");
    }

    public void ShowMessage()
    {
        Console.WriteLine("Printing from ToBeLoaded.Class2 of the IShowMessage interface");
    }
}
