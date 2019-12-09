namespace SharpSharp {
    public sealed class OperationOptions {
        public OperationOptions(bool makeNormalized = false) {
            MakeNormalized = makeNormalized;
        }

        public bool MakeNormalized { get; internal set; }
    }
}
