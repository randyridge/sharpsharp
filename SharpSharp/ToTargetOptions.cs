using System;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp {
	public sealed class ToTargetOptions {
		public ToTargetOptions(Target target, Action<OutputInfo>? callback) {
			Target = Guard.NotNull(target, nameof(target));
			Callback = callback;
		}

		public Action<OutputInfo>? Callback { get; }

		public Target Target { get; }
	}
}
