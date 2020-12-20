namespace SharpSharp {
	public struct ChannelStats {
		public ChannelStats(int min, int max, double sum, double squaresSum, double mean, double standardDeviation, int minX, int minY, int maxX, int maxY) {
			Min = min;
			Max = max;
			Sum = sum;
			SquaresSum = squaresSum;
			Mean = mean;
			StandardDeviation = standardDeviation;
			MinX = minX;
			MinY = minY;
			MaxX = maxX;
			MaxY = maxY;
		}

		public int Min { get; }

		public int Max { get; }

		public double Sum { get; }

		public double SquaresSum { get; }

		public double Mean { get; }

		public double StandardDeviation { get; }

		public int MinX { get; }

		public int MinY { get; }

		public int MaxX { get; }

		public int MaxY { get; }
	}
}
