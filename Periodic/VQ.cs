namespace Periodic
{
    public class VQ<T>
    {
        public T V { get; set; }
        public Quality Q { get; set; }
    }

    public enum Quality
    {
        Ok,
        Suspect,
        Interpolated
    }
}
