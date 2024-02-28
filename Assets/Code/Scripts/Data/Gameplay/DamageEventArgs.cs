namespace Tulip.Data.Gameplay
{
    public readonly struct DamageEventArgs
    {
        public readonly float Amount;
        public readonly IHealth Source;
        public readonly IHealth Target;

        public DamageEventArgs(float amount, IHealth source, IHealth target)
        {
            Amount = amount;
            Source = source;
            Target = target;
        }

        public override string ToString() => $"{Source} damaged {Target} for {Amount}";
    }
}
