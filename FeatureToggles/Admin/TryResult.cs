namespace FeatureToggles.Admin
{
    public class TryResult 
    {
        public virtual bool Success { get; private set; }
        public string Detail { get; private set; }

        public static TryResult Failed(string error)
        {
            return new TryResult { Success = false, Detail = error };
        }

        public static TryResult Succeeded(string detail = null)
        {
            return new TryResult { Success = true, Detail = detail };
        }
    }
}