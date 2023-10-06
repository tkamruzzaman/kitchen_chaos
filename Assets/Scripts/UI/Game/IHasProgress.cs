using System;

public interface IHasProgress 
{
    public event EventHandler<OnProgessChangedEventArgs> OnProgessChanged;
    public class OnProgessChangedEventArgs : EventArgs
    {
        public float progessNormalized;
    }
}
