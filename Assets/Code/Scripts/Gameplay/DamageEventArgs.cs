namespace Tulip.Gameplay
{
    public readonly struct DamageEventArgs
    {
        public readonly float Amount;
        public readonly Health Source;
        public readonly Health Target;

        public DamageEventArgs(float amount, Health source, Health target)
        {
            Amount = amount;
            Source = source;
            Target = target;
        }

        public override string ToString() => $"{Source} damaged {Target} for {Amount}";
    }
}
