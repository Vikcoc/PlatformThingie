namespace DefaultsAndStuff
{
    public class SampleService
    {
        public int ModuloToInterval(int value, int start, int end)
        {
            if (end <= start)
                return 0;

            if (start < value && value < end)
                return value;

            var mod = (value - start) % (end - start);
            return mod + start;
        }
    }
}
