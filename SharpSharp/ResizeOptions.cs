using NetVips;

namespace SharpSharp {
    public sealed class ResizeOptions {
        // TODO: background
        public ResizeOptions(int? width = -1, int? height = -1, Fit fit = Fit.Cover, CoverBehavior behavior = CoverBehavior.Center, string kernel = Enums.Kernel.Lanczos3, bool withoutEnlargement = false, bool fastShrinkOnLoad = true) {
            // TODO: this
            //Guard.MinimumExclusive(width, 0, nameof(width));
            //Guard.MinimumExclusive(height, 0, nameof(height));
            Width = width ?? -1;
            Height = height ?? -1;
            Fit = fit;
            Behavior = behavior;
            Kernel = kernel;
            WithoutEnlargement = withoutEnlargement;
            FastShrinkOnLoad = fastShrinkOnLoad;
        }

        public CoverBehavior Behavior { get; }

        public bool FastShrinkOnLoad { get; }

        public Fit Fit { get; }

        public int Height { get; }

        public string Kernel { get; }

        public int Width { get; }

        public bool WithoutEnlargement { get; }
    }
}
